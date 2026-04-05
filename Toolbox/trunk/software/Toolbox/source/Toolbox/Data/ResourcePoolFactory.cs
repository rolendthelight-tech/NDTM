using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using log4net;
using Toolbox.Common;
using Toolbox.Extensions;
using Toolbox.Tasks;

namespace Toolbox.Data
{
	/// <summary>
	/// Создатель пула ресурсов.
	/// </summary>
	/// <typeparam name="TResource">Тип ресурса.</typeparam>
	public class ResourcePoolFactory<TResource> : IResourcePoolFactory<TResource>
				where TResource : class, IDisposable
	{
		[NotNull] private static readonly IResourcePoolFactory<TResource> _resource_pool_factory = new ResourcePoolFactory<TResource>();

		[NotNull]
		public static IResourcePoolFactory<TResource> Get()
		{
			return _resource_pool_factory.ThrowIfNull("_resource_pool_factory");
		}

		#region Члены IResourcePoolFactory<TResource>

		[NotNull]
		public virtual IResourcePool<TResource> CreatePool([NotNull] Func<CancellationToken, TResource> resourceFactory, int? maxResourceCount, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount, [NotNull] ReadonlyParallelOptions parallelOptions)
		{
			if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
			if (readyResourceCount == null) throw new ArgumentNullException("readyResourceCount");
			if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");

			parallelOptions.CancellationToken.ThrowIfCancellationRequested();
			return new Pool(resourceFactory, maxResourceCount, readyResourceCount, parallelOptions);
		}

		#endregion

		protected interface IPool
		{
			void Reserve([NotNull] Pool.IAcquirer acquirer, int count);
			int TryReserve([NotNull] Pool.IAcquirer acquirer, int count);
			void Unreserve([NotNull] Pool.IAcquirer acquirer, int count);
			void StartAcquire([NotNull] Pool.IAcquirer acquirer);
			void EndAcquire([NotNull] Pool.IAcquirer acquirer);

			[NotNull]
			TResource Acquire([NotNull] Pool.IAcquirer acquirer, CancellationToken cancel);

			void Release([NotNull] Pool.IAcquirer acquirer, [NotNull] TResource resource);

			void ThrowIfLittered([NotNull] Pool.IAcquirer acquirer);
		}

		protected class Pool : IResourcePool<TResource>, IPool
		{
			[NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof (Pool));

			[NotNull] private readonly Func<CancellationToken, TResource> m_resource_factory;
			[NotNull] private readonly Action<TResource> m_resource_unfactory;
			private readonly int? m_max_resource_count;
			[NotNull] private readonly IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> m_ready_resource_count;
			[NotNull] private readonly ReadonlyParallelOptions m_parallel_options;
			[NotNull] private readonly HashSet<TResource> m_collection = new HashSet<TResource>(ReferenceComparer.Get());
			[NotNull] private readonly Queue<TResource> m_collection_ready = new Queue<TResource>();
			[NotNull] private readonly object m_lock = new object();
			[NotNull] private readonly ReaderWriterLockSlim m_lock_dispose = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			[NotNull] private readonly DisposeChecker m_dispose_checker;
			[NotNull] private readonly HashSet<ICreateDestroyTask<ICancellableTask>> m_tasks = new HashSet<ICreateDestroyTask<ICancellableTask>>(ReferenceComparer.Get());
			[NotNull] private readonly ICountSet<IAcquirer> m_resource_reservation = new CountSet<IAcquirer>(comparer: ReferenceComparer.Get());
			[NotNull] private readonly ICountSet<IAcquirer> m_resource_acquired = new CountSet<IAcquirer>(comparer: ReferenceComparer.Get());
			[NotNull] private readonly HashSet<IAcquirer> m_acquirers_actual = new HashSet<IAcquirer>(comparer: ReferenceComparer.Get());
			[NotNull] private int m_resource_count;
			[NotNull] private IntTuple m_create_destroy_correlation;
			[NotNull] private IntTuple m_create_destroy_correlation_task_list;
			[NotNull] private bool m_ended;

			/// <summary>
			/// Создаёт пул ресурсов.
			/// </summary>
			/// <param name="resourceFactory">Метод создания ресурса.</param>
			/// <param name="maxResourceCount">Вместимость пула.</param>
			/// <param name="readyResourceCount">Диапазон количества выделенных ресурсов, готовых к получению.</param>
			/// <param name="parallelOptions">Настройки параллелизма выделения и освобождения ресурсов.</param>
			public Pool([NotNull] Func<CancellationToken, TResource> resourceFactory, int? maxResourceCount,
				[NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount,
				[NotNull] ReadonlyParallelOptions parallelOptions)
			{
				if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
				if (readyResourceCount == null) throw new ArgumentNullException("readyResourceCount");
				if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
				if (maxResourceCount < 0) throw new ArgumentOutOfRangeException("maxResourceCount", maxResourceCount, "< 0");
				if (readyResourceCount.Left != null && readyResourceCount.Left.CompareTo(0) < 0)
					throw new ArgumentOutOfRangeException("readyResourceCount", readyResourceCount, "readyResourceCount.Left < 0");
				if (readyResourceCount.Right != null && readyResourceCount.Right.CompareTo(0) < 0)
					throw new ArgumentOutOfRangeException("readyResourceCount", readyResourceCount, "readyResourceCount.Right < 0");
				if (readyResourceCount.Left != null && readyResourceCount.Right != null && readyResourceCount.Right.CompareTo(readyResourceCount.Left) < 0)
					throw new ArgumentException("readyResourceCount.Right < readyResourceCount.Left", "readyResourceCount");

				parallelOptions.CancellationToken.ThrowIfCancellationRequested();
				this.m_resource_factory = resourceFactory;
				this.m_resource_unfactory = ResourceUnfactory;
				this.m_max_resource_count = maxResourceCount;
				this.m_ready_resource_count = readyResourceCount;
				this.m_parallel_options = parallelOptions;
				this.m_resource_count = 0;
				this.m_create_destroy_correlation = new IntTuple();
				this.m_create_destroy_correlation_task_list = new IntTuple();
				this.m_ended = false;
				this.m_dispose_checker = DisposeChecker.Create(this).ThrowIfNull("m_dispose_checker");
				try
				{
					parallelOptions.CancellationToken.ThrowIfCancellationRequested();
					CreateNecessaryResources();
					Thread.MemoryBarrier();
				}
				catch
				{
					this.m_dispose_checker.Dispose();
					throw;
				}
			}

			#region Члены IResourcePool<TResource>

			[NotNull]
			public virtual IResourceAcquirer<TReadonlyResource> CreateAcquirer<TReadonlyResource>([NotNull] Func<TResource, TReadonlyResource> selector, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> reserveResourceCount, CancellationToken cancel)
			{
				if (selector == null) throw new ArgumentNullException("selector");
				if (reserveResourceCount == null) throw new ArgumentNullException("reserveResourceCount");
				ThrowIfDisposed();
				ThrowIfEnded();

				cancel.ThrowIfCancellationRequested();
				return new Acquirer<TReadonlyResource>((IPool)this, selector, reserveResourceCount, cancel);
			}

			public virtual void EndCreate()
			{
				ThrowIfDisposed();
				ThrowIfEnded();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						ThrowIfEnded();
						this.m_ended = true;
					}
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public virtual bool Ended
			{
				get
				{
					ThrowIfDisposed();

					if (this.m_ended)
						return true;

					this.m_lock_dispose.EnterReadLock();
					try
					{
						lock (this.m_lock)
						{
							ThrowIfDisposed();
							return this.m_ended;
						}
					}
					finally
					{
						this.m_lock_dispose.ExitReadLock();
					}
				}
			}

			protected void ThrowIfEnded()
			{
				ThrowIfDisposed();
				if (this.m_ended)
					throw new ResourcePoolEndedException<TResource>("Создание запрашивателей ресурсов уже прекращено", this);
			}

			#endregion

			private void ResourceUnfactory([NotNull] TResource resource)
			{
				if (resource == null) throw new ArgumentNullException("resource");

				resource.Dispose();
			}

			private bool IsLockEntered()
			{
				return IsEntered(this.m_lock);
			}

			private bool IsLockDisposeEntered()
			{
				return /*this.m_lock_dispose.IsReadLockHeld В Acquire может быть вызов задачи ||*/ this.m_lock_dispose.IsUpgradeableReadLockHeld /*|| this.m_lock_dispose.IsWriteLockHeld В Dispose может быть вызов задачи*/;
			}

			private static bool IsEntered([NotNull] object obj)
			{
				if (obj == null) throw new ArgumentNullException("obj");

				//return Monitor.IsEntered(); // .NET 4.5
				if (_monitor_is_entered_net_4_5 != null)
					return IsEntered_NET_4_5(obj);
				else
					return IsEntered_NET_4_0(obj);
			}

			private static bool IsEntered_NET_4_0([NotNull] object obj)
			{
				if (obj == null) throw new ArgumentNullException("obj");

				try
				{
					Monitor.Pulse(obj); // Костыль до перехода на .NET 4.5
					return true;
				}
				catch (SynchronizationLockException)
				{
					return false;
				}
			}

			private static bool IsEntered_NET_4_5([NotNull] object obj)
			{
				if (obj == null) throw new ArgumentNullException("obj");

				//return Monitor.IsEntered(); // .NET 4.5
				if (_monitor_is_entered_net_4_5 != null)
					return _monitor_is_entered_net_4_5(obj);
				else
					throw new ApplicationException("Ошибка вызова метода проверки");
			}

			[CanBeNull]
			private static readonly Func<object, bool> _monitor_is_entered_net_4_5 = Get_Monitor_IsEntered();

			[CanBeNull]
			private static Func<object, bool> Get_Monitor_IsEntered()
			{
				var type = typeof (Monitor);
				var mi = type.GetMethod("IsEntered", new Type[] { typeof(object) });
				if (mi == null)
				{
					return null;
				}
				else
				{
					if (mi.ReturnType != typeof (bool))
						return null;
					else
					{
						return obj => (bool) mi.Invoke(null, new object[] { obj });
					}
				}
			}

			[Conditional("DEBUG_FULL")]
			//[Debuggable(false, false)]
			[DebuggerStepThrough]
			private void ThrowIfLocked()
			{
				if (IsLockEntered())
					throw new InvalidOperationException("Поток владеет блокировкой");
				if (IsLockDisposeEntered())
					throw new InvalidOperationException("Поток владеет блокировкой освобождения");
			}

			[NotNull]
			private TResource Create(CancellationToken cancel)
			{
				ThrowIfDisposed();

				ThrowIfLocked();

				Increment();
				try
				{
					var item = this.m_resource_factory(cancel).ThrowIfNull("item");
					return item;
				}
				catch
				{
					Decrement();
					throw;
				}
			}

			private void Destroy([NotNull] TResource item)
			{
				if (item == null) throw new ArgumentNullException("item");
				ThrowIfDisposed();
				DestroyImpl(item);
			}

			private void DestroyImpl([NotNull] TResource item)
			{
				if (item == null) throw new ArgumentNullException("item");

				ThrowIfLocked();

				try
				{
					this.m_resource_unfactory(item);
				}
				finally
				{
					Decrement();
				}
			}

			[NotNull]
			private TResource CreateAndReady(CancellationToken cancel)
			{
				ThrowIfDisposed();

				cancel.ThrowIfCancellationRequested();
				var resource = Create(cancel);
				resource = resource.ThrowIfNull("resource");
				try
				{
					cancel.ThrowIfCancellationRequested();
					ThrowIfDisposed();

					lock (this.m_lock)
					{
						ThrowIfDisposed();
						if (!this.m_collection.Add(resource))
							throw new ApplicationException("Элемент не добавился в коллекцию");
						this.m_collection_ready.Enqueue(resource);
						return resource;
					}
				}
				catch
				{
					DestroyImpl(resource);
					throw;
				}
			}

			[NotNull]
			private TResource CreateAndReadyWork([NotNull] object cancel)
			{
				if (cancel == null) throw new ArgumentNullException("cancel");

				return CreateAndReady((CancellationToken) cancel);
			}

			private void DestroyAndReady([NotNull] DestroyTaskParameters parameters)
			{
				if (parameters == null) throw new ArgumentNullException("parameters");
				ThrowIfDisposed();
				DestroyAndReadyImpl(parameters);
			}

			private void DestroyAndReadyImpl([NotNull] DestroyTaskParameters parameters)
			{
				if (parameters == null) throw new ArgumentNullException("parameters");

				parameters.Cancel.ThrowIfCancellationRequested();
				TResource resource = null;
				try
				{
					lock (this.m_lock)
					{
						Thread.MemoryBarrier();
						parameters.Cancel.ThrowIfCancellationRequested();
						resource = this.m_collection_ready.Dequeue();
						resource = resource.ThrowIfNull("resource");
						try
						{
							parameters.Cancel.ThrowIfCancellationRequested();
							this.m_collection.CheckedRemove(resource);
							try
							{
								parameters.Cancel.ThrowIfCancellationRequested();
							}
							catch
							{
								this.m_collection.CheckedAdd(resource);
								throw;
							}
						}
						catch
						{
							this.m_collection_ready.Enqueue(resource);
							resource = null;
							throw;
						}
					}
				}
				finally
				{
					if (resource != null)
						DestroyImpl(resource);
				}
			}

			private void DestroyAndReadyWork([NotNull] object parameters)
			{
				if (parameters == null) throw new ArgumentNullException("parameters");

				DestroyAndReady((DestroyTaskParameters)parameters);
			}

			private void DestroyAndReadyImplWork([NotNull] object parameters)
			{
				if (parameters == null) throw new ArgumentNullException("parameters");

				DestroyAndReadyImpl((DestroyTaskParameters)parameters);
			}

			private void DestroyWaitAndReadyImplWork([NotNull] Task<TResource> task)
			{
				if (task == null) throw new ArgumentNullException("task");

				if (task.IsCompleted)
				{
					task.Wait();
					DestroyAndReadyImpl(new DestroyTaskParameters(CancellationToken.None, task.Result));
				}
			}

			private void Increment()
			{
				ThrowIfDisposed();

				lock (this.m_lock)
				{
					ThrowIfDisposed();

					if (this.m_max_resource_count != null)
					{
						var max = this.m_max_resource_count.Value;

						if (this.m_resource_count >= max)
							throw new OverflowException("Превышение количества выделенных ресурсов");
					}
					++this.m_resource_count;
				}
			}

			private void Decrement()
			{
				//ThrowIfDisposed();

				lock (this.m_lock)
				{
					//ThrowIfDisposed();
					Thread.MemoryBarrier();

					if (this.m_resource_count <= 0)
						throw new ApplicationException("Ошибка подсчёта ресурсов");

					--this.m_resource_count;
				}
			}

			private int GetOptimalReadyCount()
			{
				var count = 1; // TODO

				if (this.m_ready_resource_count.Left != null)
				{
					count = Math.Max(count, this.m_ready_resource_count.Left.Value);
				}
				if (this.m_ready_resource_count.Right != null)
				{
					count = Math.Min(count, this.m_ready_resource_count.Right.Value);
				}
				if (count < 0)
					throw new ApplicationException("Ошибка подсчёта ресурсов");

				return count;
			}

			#region Члены IPool

			public void Reserve([NotNull] IAcquirer acquirer, int count)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");
				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();

						if (!this.m_acquirers_actual.Contains(acquirer))
						{
							throw new ApplicationException("Запрашиватель закончил запрос");
						}

						if (this.m_max_resource_count != null)
						{
							var max = this.m_max_resource_count.Value;

							if (this.m_resource_reservation.TotalCount + count > max)
								throw new OverflowException("Превышение количества зарезервированных ресурсов");
						}
						this.m_resource_reservation.Increment(acquirer, count);
					}

					CreateNecessaryResources();
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public int TryReserve([NotNull] IAcquirer acquirer, int count)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");
				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();

						if (!this.m_acquirers_actual.Contains(acquirer))
						{
							throw new ApplicationException("Запрашиватель закончил запрос");
						}

						if (this.m_max_resource_count != null)
						{
							var max = this.m_max_resource_count.Value;

							if (this.m_resource_reservation.TotalCount + count > max)
							{
								count = max - this.m_resource_reservation.TotalCount;
								if (count < 0) throw new ApplicationException("Ошибка подсчёта ресурсов");
							}
						}
						this.m_resource_reservation.Increment(acquirer, count);
					}

					CreateNecessaryResources();

					return count;
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public void Unreserve([NotNull] IAcquirer acquirer, int count)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");
				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();

						if (this.m_resource_reservation.TotalCount - count < 0)
							throw new ApplicationException("Ошибка подсчёта зарезервированных ресурсов");
						else
							this.m_resource_reservation.Decrement(acquirer, count);
					}

					ReleaseUnnecessaryResources();
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public void StartAcquire([NotNull] IAcquirer acquirer)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				ThrowIfDisposed();
				ThrowIfEnded();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						ThrowIfEnded();
						this.m_acquirers_actual.CheckedAdd(acquirer);
					}
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public void EndAcquire([NotNull] IAcquirer acquirer)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						if (this.m_resource_reservation[acquirer] != 0)
						{
							throw new ApplicationException("Не все резервирования ресурсов запрашивателя сняты");
						}
						if (this.m_resource_acquired[acquirer] != 0)
						{
							throw new ApplicationException("Не все ресурсы запрашивателем возвращены");
						}
						this.m_acquirers_actual.CheckedRemove(acquirer);
					}
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			#endregion

			private void WaitOne([NotNull] Func<bool> pre_predicate, [NotNull] Func<ICreateDestroyTask<ICancellableTask>, bool> predicate, CancellationToken cancel)
			{
				if (pre_predicate == null) throw new ArgumentNullException("pre_predicate");
				if (predicate == null) throw new ArgumentNullException("predicate");

				cancel.ThrowIfCancellationRequested();
				ICreateDestroyTask<ICancellableTask>[] cd_tasks;
				Task[] tasks;
				lock (this.m_lock)
				{
					cancel.ThrowIfCancellationRequested();
					Thread.MemoryBarrier();
					if (pre_predicate())
						return;
					cd_tasks = this.m_tasks.Where(predicate).ToArray();
					if (cd_tasks.Length == 0)
						throw new ApplicationException("Нечего ждать");
					tasks = cd_tasks.Select(task => task.Task.Task).ToArray();
				}
				cancel.ThrowIfCancellationRequested();

				cancel.ThrowIfCancellationRequested();
				{
					var i = Task.WaitAny(tasks, cancel);
					cancel.ThrowIfCancellationRequested();
					var cd_task = cd_tasks[i];
					var task = cd_task.Task.Task;
					if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.Canceled)
						throw new ApplicationException("Ошибка ожидания задачи");
					cancel.ThrowIfCancellationRequested();
					try
					{
						task.Wait(cancel);
					}
					catch (ObjectDisposedException ex)
					{
						_log.Debug("WaitOne(): ObjectDisposedException", ex);
					}
					cancel.ThrowIfCancellationRequested();
					var create_destroy_correlation = cd_task.IsCreate ? new IntTuple(1, 0) : new IntTuple(0, 1);
					cancel.ThrowIfCancellationRequested();
					lock (this.m_lock)
					{
						Thread.MemoryBarrier();
						if (this.m_tasks.Remove(cd_task))
						{
							this.m_create_destroy_correlation_task_list -= create_destroy_correlation;
						}
					}
					task.Dispose();
					cd_task.Task.CancellationToken.Dispose();
				}
			}

			private void WaitCreateOne(CancellationToken cancel)
			{
				WaitOne(() => this.m_collection_ready.Count > 0, task => task.IsCreate, cancel);
			}

			/*
			private void WaitDestroyOne(CancellationToken cancel)
			{
				WaitOne(,task => !task.IsCreate, cancel);
			}
			*/

			private ISuccessResult<TResource> AcquireReady([NotNull] IAcquirer acquirer, CancellationToken cancel)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				ThrowIfDisposed();

				TResource item;
				bool success;
				lock (this.m_lock)
				{
					ThrowIfDisposed();

					if (!this.m_acquirers_actual.Contains(acquirer))
					{
						throw new ApplicationException("Запрашиватель закончил запрос");
					}

					if (this.m_resource_reservation[acquirer] < (this.m_resource_acquired[acquirer] + 1))
						throw new InvalidOperationException("Запрошен не зарезервированный ресурс");

					success = this.m_collection_ready.Count > 0;
					if (success)
					{
						item = this.m_collection_ready.Dequeue().ThrowIfNull("item");
						this.m_resource_acquired.Increment(acquirer);
					}
					else
						item = null;
				}
				if (success)
				{
					CreateNecessaryResources();
					return SuccessResult<TResource>.Create(item.ThrowIfNull("item"));
				}
				else
				{
					return FailResult<TResource>.Create();
				}
			}

			[NotNull]
			public TResource Acquire([NotNull] IAcquirer acquirer, CancellationToken cancel)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					for (;;)
					{
						var result = AcquireReady(acquirer, cancel);
						if (result.Success)
							return result.Result.ThrowIfNull("Result");
						else
							WaitCreateOne(cancel);
					}
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public void Release([NotNull] IAcquirer acquirer, [NotNull] TResource item)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");
				if (item == null) throw new ArgumentNullException("item");

				ThrowIfDisposed();

				this.m_lock_dispose.EnterReadLock();
				try
				{
					lock (this.m_lock)
					{
						ThrowIfDisposed();

						if (this.m_collection.Contains(item))
						{
							if (this.m_collection_ready.Contains(item))
								throw new InvalidOperationException("Элемент уже был возвращён");

							this.m_collection_ready.Enqueue(item);
							this.m_resource_acquired.Decrement(acquirer);
						}
						else
						{
							throw new InvalidOperationException("Элемент не из этого хранилища");
						}
					}

					ReleaseUnnecessaryResources();
				}
				finally
				{
					this.m_lock_dispose.ExitReadLock();
				}
			}

			public void ThrowIfLittered([NotNull] IAcquirer acquirer)
			{
				if (acquirer == null) throw new ArgumentNullException("acquirer");

				lock (this.m_lock)
				{
					Thread.MemoryBarrier();
					if (this.m_resource_reservation.Contains(acquirer)) throw new ApplicationException("Остался мусор при резервировании ресурсов");
					if (this.m_resource_acquired.Contains(acquirer)) throw new ApplicationException("Остался мусор при выделении ресурсов");
				}
			}

			private void CreateNecessaryResources()
			{
				ThrowIfDisposed();

				lock (this.m_lock)
				{
					ThrowIfDisposed();
					var optimal_count = GetOptimalReadyCount();

					if (this.m_resource_reservation.TotalCount > this.m_resource_count
					    || (((this.m_ready_resource_count.Left != null
					          && this.m_ready_resource_count.Left.Value > this.m_collection_ready.Count)
					         || (optimal_count > this.m_collection_ready.Count))
					        && (this.m_max_resource_count == null
					            || this.m_max_resource_count.Value > this.m_collection.Count)))
					{
						var count = this.m_resource_reservation.TotalCount - this.m_resource_count;
						if (this.m_max_resource_count != null)
						{
							if (count > this.m_max_resource_count.Value)
								throw new ApplicationException("Ошибка резервирования ресурсов");
						}
						if (this.m_ready_resource_count.Left != null)
						{
							optimal_count = Math.Max(this.m_ready_resource_count.Left.Value, optimal_count);
						}
						count = Math.Max(count, optimal_count - this.m_collection_ready.Count);
						if (this.m_max_resource_count != null)
						{
							count = Math.Min(count, this.m_max_resource_count.Value - this.m_resource_count);
						}
						if (count <= 0)
							throw new ApplicationException("Ошибка подсчёта ресурсов");
						else if (this.m_max_resource_count != null)
						{
							if (count + this.m_resource_count > this.m_max_resource_count.Value)
								throw new ApplicationException("Ошибка резервирования или подсчёта ресурсов");
						}
						else
						{
							for (int i = 0; i < count; i++)
							{
								var cancel = new CancellationTokenSource();
								var task = Task<TResource>.Factory.StartNew(this.CreateAndReadyWork, cancel.Token, cancel.Token, TaskCreationOptions.None, this.m_parallel_options.TaskScheduler);
								var cancellable_task = new CreateTask<ICancellableTask<TResource>, TResource>(new CancellableTask<TResource>(task, cancel));
								this.m_tasks.CheckedAdd(cancellable_task);
								this.m_create_destroy_correlation += new IntTuple(1, 0);
								this.m_create_destroy_correlation_task_list += new IntTuple(1, 0);
							}
						}
					}
				}
			}

			private void ReleaseUnnecessaryResources()
			{
				ThrowIfDisposed();

				lock (this.m_lock)
				{
					ThrowIfDisposed();
					if (this.m_resource_reservation.TotalCount < this.m_resource_count
					    && ((this.m_ready_resource_count.Right != null
					         && this.m_ready_resource_count.Right.Value < this.m_collection_ready.Count)
					        || (this.m_ended
					            && this.m_acquirers_actual.Count == 0))
					    && this.m_collection.Count > 0)
					{
						var count = this.m_resource_count - this.m_resource_reservation.TotalCount;
						count = Math.Max(count, 0);
						if ((!(this.m_ended && this.m_acquirers_actual.Count == 0)) && this.m_ready_resource_count.Right != null)
						{
							count = Math.Min(count, Math.Max(this.m_collection_ready.Count - this.m_ready_resource_count.Right.Value, 0));
						}
						if (count < 0)
							throw new ApplicationException("Ошибка подсчёта лишних ресурсов");
						else if (count > this.m_resource_count)
							throw new ApplicationException("Ошибка подсчёта ресурсов");
						else if (this.m_resource_count - count < this.m_resource_reservation.TotalCount)
							throw new ApplicationException("Ошибка резервирования ресурсов");
						else if (count > this.m_collection_ready.Count)
							throw new ApplicationException("Ошибка подсчёта готовых ресурсов");
						else
						{
							for (int i = 0; i < count; i++)
							{
								var cancel = new CancellationTokenSource();
								var task = Task.Factory.StartNew(this.DestroyAndReadyWork, new DestroyTaskParameters(cancel.Token, null), cancel.Token, TaskCreationOptions.None, this.m_parallel_options.TaskScheduler);
								var cancellable_task = new DestroyTask<ICancellableTask, ICancellableTask>(new CancellableTask(task, cancel), null);
								this.m_tasks.CheckedAdd(cancellable_task);
								this.m_create_destroy_correlation += new IntTuple(0, 1);
								this.m_create_destroy_correlation_task_list += new IntTuple(0, 1);
							}
						}
					}
				}
			}

			#region Члены IDisposable

			public virtual void Dispose()
			{
				Thread.MemoryBarrier();
				this.m_lock_dispose.EnterWriteLock();
				try
				{
					int collection_count;
					int collection_ready_count;
					int acquirer_count;
					IntTuple create_destroy_correlation;
					IntTuple create_destroy_correlation_task_list;
					try
					{
						lock (this.m_lock)
						{
							this.m_dispose_checker.Dispose();
							Thread.MemoryBarrier();

							var cancels = this.m_tasks.Where(task => task.IsCreate).Select(task => (Action) task.Task.CancellationToken.Cancel).ToArray();
							cancels.InvokeAll(null);

							{
								var count = this.m_collection_ready.Count;
								for (int i = 0; i < count; i++)
								{
									var cancel = new CancellationTokenSource();
									var task = Task.Factory.StartNew(this.DestroyAndReadyImplWork, new DestroyTaskParameters(cancel.Token, null), cancel.Token, TaskCreationOptions.None, this.m_parallel_options.TaskScheduler);
									var cancellable_task = new DestroyTask<ICancellableTask, ICancellableTask>(new CancellableTask(task, cancel), null);
									this.m_tasks.CheckedAdd(cancellable_task);
									this.m_create_destroy_correlation += new IntTuple(0, 1);
									this.m_create_destroy_correlation_task_list += new IntTuple(0, 1);
								}
							}

							var c_tasks =
								this
									.m_tasks
									.Where(task => task.IsCreate)
									.Cast<ICreateTask<ICancellableTask<TResource>, TResource>>()
									.Select(task => task.Task)
									.Where(task => !this.m_collection_ready.Contains(task.Task.Result.ThrowIfNull("Result")))
									//.Except(this.m_collection_ready)
									.ToArray();

							foreach(var c_task in c_tasks)
							{
								var cancel = new CancellationTokenSource();
								var task = c_task.Task.ContinueWith(this.DestroyWaitAndReadyImplWork, cancel.Token, TaskContinuationOptions.OnlyOnRanToCompletion, this.m_parallel_options.TaskScheduler);
								var cancellable_task = new DestroyTask<ICancellableTask, ICancellableTask>(new CancellableTask(task, cancel), c_task);
								this.m_tasks.CheckedAdd(cancellable_task);
								this.m_create_destroy_correlation += new IntTuple(0, 1);
								this.m_create_destroy_correlation_task_list += new IntTuple(0, 1);
							}


							//TODO: освободить готовые ресурсы, отменить резервирование, аннулировать выделятелей, освободить остальные ресурсы


							collection_count = this.m_collection.Count;
							collection_ready_count = this.m_collection_ready.Count;
							acquirer_count = this.m_acquirers_actual.Count;
							this.m_acquirers_actual.Clear();

							/*destroys = this.m_collection.Select(item => (Action) (() => Destroy(item))).ToArray();
						this.m_collection.Clear();
						this.m_collection_ready.Clear();*/
						}

						/*	destroys.InvokeAll();*/
					}
					finally
					{
						Task[] tasks;
						IEnumerable<Action> t_dispose;
						IEnumerable<Action> ct_dispose;
						lock (this.m_lock)
						{
							Thread.MemoryBarrier();
							tasks = this.m_tasks.Select(task => task.Task.Task).ToArray();
							t_dispose = tasks.Select(task => (Action) task.Dispose);
							ct_dispose = this.m_tasks.Select(task => (Action) task.Task.CancellationToken.Dispose).ToArray();
							create_destroy_correlation = this.m_create_destroy_correlation;
							create_destroy_correlation_task_list = this.m_tasks.Aggregate(this.m_create_destroy_correlation_task_list, (a, task) => a - (task.IsCreate ? new IntTuple(1, 0) : new IntTuple(0, 1)));
							this.m_tasks.Clear();
							this.m_create_destroy_correlation_task_list = new IntTuple();
						}
						try
						{
							Task.WaitAll(tasks);
						}
						finally
						{
							lock (this.m_lock)
							{
								try
								{
									t_dispose.InvokeAll();
								}
								finally
								{
									ct_dispose.InvokeAll();
								}
							}
						}
						lock (this.m_lock)
						{
							Thread.MemoryBarrier();
							if (this.m_collection.Count != 0)
								throw new ApplicationException("Не все ресурсы освобождены");
							if (this.m_collection_ready.Count != 0)
								throw new ApplicationException("Не все готовые ресурсы освобождены");
						}
					}

					lock (this.m_lock)
					{
						Thread.MemoryBarrier();
						if (acquirer_count != 0)
							throw new InvalidOperationException("Не запрашиватели были освобождены");
						if (collection_count != collection_ready_count)
							throw new InvalidOperationException("Не все объекты были возвращены");
						if (this.m_resource_acquired.TotalCount != 0)
							throw new InvalidOperationException("Не все ресурсы были возвращены");
						if (this.m_resource_reservation.TotalCount != 0)
							throw new InvalidOperationException("Не все резервирования ресурсов были сняты");
						if (create_destroy_correlation.Item0 != create_destroy_correlation.Item1)
							throw new ApplicationException("Количество задач создания не равно количеству задач уничтожения");
						if (create_destroy_correlation_task_list != new IntTuple())
							throw new ApplicationException("Количество задач создания в списке не равно количеству задач уничтожения");
					}
					Thread.MemoryBarrier();
				}
				finally
				{
					this.m_lock_dispose.ExitWriteLock();
				}
				lock (this.m_lock)
				{
					this.m_lock_dispose.Dispose();
				}
			}

			protected void ThrowIfDisposed()
			{
				Thread.MemoryBarrier();
				this.m_dispose_checker.ThrowIfDisposed();
			}

			#endregion

			private interface ICancellableTask
			{
				[NotNull]
				Task Task { get; }

				[NotNull]
				CancellationTokenSource CancellationToken { get; }
			}

			private interface ICancellableTask<TResult> : ICancellableTask
			{
				[NotNull]
				new Task<TResult> Task { get; }
			}

			private class CancellableTask : ICancellableTask
			{
				[NotNull] private readonly Task m_task;
				[NotNull] private readonly CancellationTokenSource m_cancellation_token;

				public CancellableTask([NotNull] Task task, [NotNull] CancellationTokenSource cancellationToken)
				{
					if (task == null) throw new ArgumentNullException("task");
					if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

					this.m_task = task;
					this.m_cancellation_token = cancellationToken;
				}

				[NotNull]
				public Task Task
				{
					get { return this.m_task; }
				}

				[NotNull]
				public CancellationTokenSource CancellationToken
				{
					get { return this.m_cancellation_token; }
				}
			}

			private class CancellableTask<TResult> : CancellableTask, ICancellableTask<TResult>
			{
				[NotNull]
				private readonly Task<TResult> m_task;

				public CancellableTask([NotNull] Task<TResult> task, [NotNull] CancellationTokenSource cancellationToken)
					: base(task, cancellationToken)
				{
					if (task == null) throw new ArgumentNullException("task");

					this.m_task = task;
				}

				[NotNull]
				public new Task<TResult> Task
				{
					get { return this.m_task; }
				}
			}

			private interface ICreateDestroyTask<out TCancellableTask>
			where TCancellableTask : ICancellableTask
			{
				[NotNull]
				TCancellableTask Task { get; }

				bool IsCreate { get; }
			}

			private interface IDestroyTask<out TCancellableTask, out TPreviousCancellableTask> : ICreateDestroyTask<TCancellableTask>
				where TCancellableTask : ICancellableTask
				where TPreviousCancellableTask : ICancellableTask
			{
				[CanBeNull]
				TPreviousCancellableTask PreviousTask { get; }
			}

			private interface ICreateTask<out TCancellableTask, out TResult> : ICreateDestroyTask<TCancellableTask>
				where TCancellableTask : ICancellableTask<TResult>
			{
			}

			private abstract class CreateDestroyTaskBase<TCancellableTask> : ICreateDestroyTask<TCancellableTask>
				where TCancellableTask : class, ICancellableTask
			{
				[NotNull]
				private readonly TCancellableTask m_task;

				public CreateDestroyTaskBase([NotNull] TCancellableTask task)
				{
					if (task == null) throw new ArgumentNullException("task");

					this.m_task = task;
				}

				[NotNull]
				public TCancellableTask Task
				{
					get { return this.m_task; }
				}

				public abstract bool IsCreate { get; }
			}

			private class CreateTask<TCancellableTask, TResult> : CreateDestroyTaskBase<TCancellableTask>, ICreateTask<TCancellableTask, TResult>
				where TCancellableTask : class, ICancellableTask<TResult>
			{
				public CreateTask([NotNull] TCancellableTask task)
					: base(task)
				{
				}

				public override bool IsCreate
				{
					get { return true; }
				}
				/*
				[CanBeNull]
				public CancellableTask PreviousTask
				{
					get { return null; }
				}*/
			}

			private class DestroyTaskParameters : Tuple<CancellationToken, TResource>
			{
				public DestroyTaskParameters(CancellationToken cancel, [CanBeNull] TResource resource)
					: base(cancel, resource)
				{
				}

				public CancellationToken Cancel
				{
					get { return this.Item1; }
				}

				[CanBeNull]
				public TResource Resource
				{
					get { return this.Item2; }
				}
			}

			private class DestroyTask<TCancellableTask, TPreviousCancellableTask> : CreateDestroyTaskBase<TCancellableTask>, IDestroyTask<TCancellableTask, TPreviousCancellableTask>
				where TCancellableTask : class, ICancellableTask
				where TPreviousCancellableTask : class, ICancellableTask
			{
				[CanBeNull] private readonly TPreviousCancellableTask m_previous_task;

				public DestroyTask([NotNull] TCancellableTask task, [CanBeNull] TPreviousCancellableTask previousTask) : base(task)
				{
					this.m_previous_task = previousTask;
				}

				public override bool IsCreate
				{
					get { return false; }
				}

				[CanBeNull]
				public TPreviousCancellableTask PreviousTask
				{
					get { return this.m_previous_task; }
				}
			}

			private struct IntTuple : IFormattable, IEquatable<IntTuple>
			{
				private readonly int m_item0;
				private readonly int m_item1;

				public IntTuple(int item0, int item1)
				{
					this.m_item0 = item0;
					this.m_item1 = item1;
				}

				public int Item0
				{
					get
					{
						return this.m_item0;
					}
				}

				public int Item1
				{
					get
					{
						return this.m_item1;
					}
				}

				#region Overrides of ValueType

				public override bool Equals(object obj)
				{
					return obj is IntTuple && this.Equals((IntTuple) obj);
				}

				public override string ToString()
				{
					return this.ToString(null, null);
				}

				#endregion

				#region Equality members

				public bool Equals(IntTuple other)
				{
					return this.m_item0 == other.m_item0 && this.m_item1 == other.m_item1;
				}

				public override int GetHashCode()
				{
					unchecked
					{
						return (this.m_item0*1955079017) + this.m_item1;
					}
				}

				#endregion

				#region Члены IFormattable

				public string ToString(string format, IFormatProvider formatProvider)
				{
					if (format == null)
						return string.Format(formatProvider, "[{0}, {1}]", this.Item0, this.Item1);
					else
						return string.Format(formatProvider, "[{0}, {1}]", this.Item0.ToString(format, formatProvider), this.Item1.ToString(format, formatProvider));
				}

				#endregion

				public static IntTuple operator +(IntTuple value)
				{
					return new IntTuple(+value.Item0, +value.Item1);
				}

				public static IntTuple operator -(IntTuple value)
				{
					return new IntTuple(-value.Item0, -value.Item1);
				}

				public static IntTuple operator +(IntTuple value0, IntTuple value1)
				{
					return new IntTuple(value0.Item0 + value1.Item0, value0.Item1 + value1.Item1);
				}

				public static IntTuple operator -(IntTuple value0, IntTuple value1)
				{
					return new IntTuple(value0.Item0 - value1.Item0, value0.Item1 - value1.Item1);
				}

				public static IntTuple operator *(IntTuple value, int multiplier)
				{
					return new IntTuple(value.Item0*multiplier, value.Item1*multiplier);
				}

				public static IntTuple operator /(IntTuple value, int divisor)
				{
					return new IntTuple(value.Item0/divisor, value.Item1/divisor);
				}

				public static IntTuple operator %(IntTuple value, int divisor)
				{
					return new IntTuple(value.Item0%divisor, value.Item1%divisor);
				}

				public static bool operator ==(IntTuple value0, IntTuple value1)
				{
					return (value0.Item0 == value1.Item0) && (value0.Item1 == value1.Item1);
				}

				public static bool operator !=(IntTuple value0, IntTuple value1)
				{
					return (value0.Item0 != value1.Item0) || (value0.Item1 != value1.Item1);
				}
			}

			public interface IAcquirer : IDisposable
			{
				[NotNull]
				TResource Acquire(CancellationToken cancel);
				void Release([NotNull] TResource resource);
			}

			public interface IAcquirer<out TReadonlyResource> : IAcquirer
			{
				[NotNull]
				TReadonlyResource SelectReadonly([NotNull] TResource resource);
			}

			protected class Acquirer<TReadonlyResource> : IResourceAcquirer<TReadonlyResource>, IAcquirer<TReadonlyResource>
			{
				[NotNull] private readonly IPool m_pool;
				[NotNull] private readonly Func<TResource, TReadonlyResource> m_selector;
				[NotNull] private readonly IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> m_reserve_resource_count;
				[NotNull] private readonly object m_lock = new object();
				[NotNull] private readonly DisposeChecker m_dispose_checker;
				[NotNull] private bool m_ended;
				[NotNull] private int m_reserved;
				[NotNull] private int m_acquired;

				public Acquirer([NotNull] IPool pool, [NotNull] Func<TResource, TReadonlyResource> selector, [NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> reserveResourceCount, CancellationToken cancel)
				{
					if (pool == null) throw new ArgumentNullException("pool");
					if (selector == null) throw new ArgumentNullException("selector");
					if (reserveResourceCount == null) throw new ArgumentNullException("reserveResourceCount");

					cancel.ThrowIfCancellationRequested();
					pool.ThrowIfLittered((IAcquirer) this);
					cancel.ThrowIfCancellationRequested();

					this.m_pool = pool;
					this.m_selector = selector;
					this.m_reserve_resource_count = new UnboundedInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>>(left: reserveResourceCount.Left, right: reserveResourceCount.Right);
					this.m_reserved = 0;
					this.m_acquired = 0;
					this.m_dispose_checker = DisposeChecker.Create(this);
					try
					{
						cancel.ThrowIfCancellationRequested();
						this.m_pool.StartAcquire((IAcquirer) this);
						try
						{
							cancel.ThrowIfCancellationRequested();
							Reserve(false);
						}
						catch
						{
							this.m_pool.EndAcquire((IAcquirer) this);
							throw;
						}
					}
					catch
					{
						this.m_dispose_checker.Dispose();
						throw;
					}
				}

				private void Reserve(bool mandatory)
				{
					if (this.m_ended)
						throw new InvalidOperationException("Выделение ресурсов завершено");

					lock (this.m_lock)
					{
						if (this.m_ended)
							throw new InvalidOperationException("Выделение ресурсов завершено");

						if (mandatory || this.m_reserve_resource_count.Left != null)
						{
							var min = this.m_reserve_resource_count.Left == null ? 0 : this.m_reserve_resource_count.Left.Value - this.m_reserved;
							if (min <= 0 && mandatory && this.m_reserved == this.m_acquired &&
							    (this.m_reserve_resource_count.Right == null || this.m_reserve_resource_count.Right.Value > this.m_reserved))
								min = 1;
							if (min > 0)
							{
								this.m_pool.Reserve((IAcquirer)this, min);
								this.m_reserved += min;
							}
							if (this.m_reserve_resource_count.Right != null && this.m_reserve_resource_count.Right.Value > this.m_reserved)
							{
								var count = this.m_pool.TryReserve((IAcquirer)this, this.m_reserve_resource_count.Right.Value - this.m_reserved);
								if (count < 0)
									throw new ApplicationException("Неверное резервирование");
								else
								{
									this.m_reserved += count;
								}
							}
						}
					}
				}

				private void Unreserve()
				{
					if (!this.m_ended)
						throw new InvalidOperationException("Выделение ресурсов не завершено");

					var count = this.m_reserved - this.m_acquired;
					if (count < 0)
						throw new ApplicationException("Ошибка подсчёта ресурсов");
					this.m_pool.Unreserve((IAcquirer) this, count);
					this.m_reserved -= count;
				}

				#region Члены IAcquirer

				[NotNull]
				public TResource Acquire(CancellationToken cancel)
				{
					ThrowIfDisposed();
					ThrowIfEnded();
					cancel.ThrowIfCancellationRequested();
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						ThrowIfEnded();
						cancel.ThrowIfCancellationRequested();
						if (this.m_reserve_resource_count.Right != null)
						{
							if (this.m_reserve_resource_count.Right.Value < this.m_acquired + 1)
								throw new InvalidOperationException("Превышение допустимого количества ресурсов");
						}
						if (this.m_reserved < this.m_acquired + 1)
						{
							cancel.ThrowIfCancellationRequested();
							this.Reserve(true);
						}
						this.m_acquired++;
					}
					try
					{
						cancel.ThrowIfCancellationRequested();
						var resource = this.m_pool.Acquire((IAcquirer)this, cancel).ThrowIfNull("resource");
						return resource;
					}
					catch
					{
						lock (this.m_lock)
						{
							this.m_acquired--;
						}
						throw;
					}
				}

				public void Release([NotNull] TResource resource)
				{
					if (resource == null) throw new ArgumentNullException("resource");
					ThrowIfDisposed();

					bool ended;
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						ended = this.m_ended;
						if (this.m_acquired <= 0)
							throw new InvalidOperationException("Не было выделено ресурсов");
						if (this.m_reserved <= 0)
							throw new InvalidOperationException("Не было зарезервировано ресурсов");
						this.m_acquired--;
						if (ended)
							this.m_reserved--;
					}
					try
					{
						try
						{
							this.m_pool.Release((IAcquirer) this, resource);
						}
						finally
						{
							if (ended)
								this.m_pool.Unreserve((IAcquirer) this, 1);
						}
					}
					catch
					{
						lock (this.m_lock)
						{
							this.m_acquired++;
							if (ended)
								this.m_reserved++;
						}
						throw;
					}
				}

				[NotNull]
				public TReadonlyResource SelectReadonly([NotNull] TResource resource)
				{
					if (resource == null) throw new ArgumentNullException("resource");
					ThrowIfDisposed();

					return this.m_selector(resource);
				}

				#endregion

				#region Члены IResourceAcquirer<TResource>

				[NotNull]
				public virtual IResourceLink<TReadonlyResource> AcquireLink(CancellationToken cancel)
				{
					ThrowIfDisposed();
					ThrowIfEnded();

					cancel.ThrowIfCancellationRequested();
					return new Link((IAcquirer<TReadonlyResource>)this, cancel);
				}

				[NotNull]
				public virtual IResourcePool<TReferencedResource> CreateReferencePool<TReferencedResource>([NotNull] Func<TReadonlyResource, CancellationToken, TReferencedResource> resourceFactory,
 int? maxResourceCount,
					[NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount,
					[NotNull] ReadonlyParallelOptions parallelOptions)
					where TReferencedResource : class, IDisposable
				{
					if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
					if (readyResourceCount == null) throw new ArgumentNullException("readyResourceCount");
					if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
					ThrowIfDisposed();
					ThrowIfEnded();

					/*if (maxResourceCount.HasValue)
					{
						if (this.m_max_resource_count.HasValue)
							if (maxResourceCount.Value > this.m_max_resource_count.Value)
								throw new ArgumentOutOfRangeException("maxResourceCount", maxResourceCount, "> maxResourceCount базового пула");
					}
					else
					{
						if (this.m_max_resource_count.HasValue)
							maxResourceCount = this.m_max_resource_count;
					}*/

					return new ReferencePool<TReferencedResource>(this, resourceFactory, maxResourceCount, readyResourceCount, parallelOptions);
				}

				public void EndAcquire()
				{
					ThrowIfDisposed();
					ThrowIfEnded();
					lock (this.m_lock)
					{
						ThrowIfDisposed();
						ThrowIfEnded();
						this.m_ended = true;
						Unreserve();
						this.m_pool.EndAcquire((IAcquirer) this);
					}
				}

				public bool Ended
				{
					get
					{
						ThrowIfDisposed();
						return this.m_ended;
					}
				}

				protected void ThrowIfEnded()
				{
					ThrowIfDisposed();
					if (this.m_ended)
						throw new ResourceAcquirerEndedException<TReadonlyResource>("Запрос ресурсов уже прекращён", this);
				}

				#endregion

				#region Члены IDisposable

				public virtual void Dispose()
				{
					lock (this.m_lock)
					{
						this.m_dispose_checker.Dispose();

						var ended = this.m_ended;
						this.m_ended = true;
						Unreserve();
						if (!ended)
							this.m_pool.EndAcquire((IAcquirer) this);

						this.m_pool.ThrowIfLittered((IAcquirer) this);
					}
				}

				protected void ThrowIfDisposed()
				{
					Thread.MemoryBarrier();
					this.m_dispose_checker.ThrowIfDisposed();
				}

				#endregion

				protected class Link : IResourceLink<TReadonlyResource>
				{
					[NotNull] private readonly TResource m_resource;
					[NotNull] private readonly IAcquirer<TReadonlyResource> m_acquirer;
					[NotNull] private readonly DisposeChecker m_dispose_checker;
					[NotNull] private readonly object m_lock = new object();

					public Link([NotNull] IAcquirer<TReadonlyResource> acquirer, CancellationToken cancel)
					{
						if (acquirer == null) throw new ArgumentNullException("acquirer");

						cancel.ThrowIfCancellationRequested();
						this.m_acquirer = acquirer;
						this.m_dispose_checker = DisposeChecker.Create(this);
						try
						{
							cancel.ThrowIfCancellationRequested();
							this.m_resource = this.m_acquirer.Acquire(cancel).ThrowIfNull("item");
						}
						catch
						{
							this.m_dispose_checker.Dispose();
							throw;
						}
						Thread.MemoryBarrier();
					}

					#region Члены IDisposable

					public virtual void Dispose()
					{
						ThrowIfDisposed();
						lock (this.m_lock)
						{
							ThrowIfDisposed();
							this.m_dispose_checker.Dispose();
							this.m_acquirer.Release(this.m_resource);
						}
					}

					protected void ThrowIfDisposed()
					{
						Thread.MemoryBarrier();
						this.m_dispose_checker.ThrowIfDisposed();
					}

					#endregion

					#region Члены IResourceLink<TResource>

					public TReadonlyResource Resource
					{
						get
						{
							ThrowIfDisposed();
							return this.m_acquirer.SelectReadonly(this.m_resource);
						}
					}

					#endregion
				}

				protected class ReferencePool<TReferencedResource> : IResourcePool<TReferencedResource>
					where TReferencedResource : class, IDisposable
				{
					[NotNull] private readonly IResourcePool<Reference> m_pool;
					[NotNull] private readonly IResourceAcquirer<TReadonlyResource> m_base_pool;
					[NotNull] private readonly Func<TReadonlyResource, CancellationToken, TReferencedResource> m_resource_factory;
					[NotNull] private readonly Action<TReferencedResource> m_resource_unfactory;
					[NotNull] private readonly DisposeChecker m_dispose_checker;

					public ReferencePool([NotNull] IResourceAcquirer<TReadonlyResource> basePool, [NotNull] Func<TReadonlyResource, CancellationToken, TReferencedResource> resourceFactory,
 int? maxResourceCount,
						[NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount,
						[NotNull] ReadonlyParallelOptions parallelOptions)
					{
						if (basePool == null) throw new ArgumentNullException("basePool");
						if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
						if (readyResourceCount == null) throw new ArgumentNullException("readyResourceCount");
						if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");

						this.m_base_pool = basePool;
						this.m_resource_factory = resourceFactory;
						this.m_resource_unfactory = ResourceUnfactory;
						this.m_dispose_checker = DisposeChecker.Create(this);
						try
						{
							this.m_pool = new ResourcePoolFactory<Reference>.Pool(ResourceFactory, maxResourceCount, readyResourceCount, parallelOptions);
						}
						catch
						{
							this.m_dispose_checker.Dispose();
							throw;
						}
					}
					private void ResourceUnfactory([NotNull] TReferencedResource resource)
					{
						if (resource == null) throw new ArgumentNullException("resource");

						resource.Dispose();
					}

					[NotNull]
					private Reference ResourceFactory(CancellationToken cancel)
					{
						cancel.ThrowIfCancellationRequested();
						return new Reference(this, cancel);
					}

					private void ResourceUnfactory([NotNull] Reference referencedResource)
					{
						if (referencedResource == null) throw new ArgumentNullException("referencedResource");

						referencedResource.Dispose();
					}

					#region Члены IResourcePool<out TReferencedResource>

					public IResourceAcquirer<TReadonlyResource> CreateAcquirer<TReadonlyResource>([NotNull] Func<TReferencedResource, TReadonlyResource> selector, IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> reserveResourceCount, CancellationToken cancel)
					{
						return new Acquirer<TReadonlyResource>(this, selector, reserveResourceCount, cancel);
					}

					public void EndCreate()
					{
						ThrowIfDisposed();
						this.m_pool.EndCreate();
					}

					public bool Ended
					{
						get
						{
							ThrowIfDisposed();
							return this.m_pool.Ended;
						}
					}

					#endregion

					#region Члены IDisposable
					public void Dispose()
					{
						try
						{
							this.m_dispose_checker.Dispose();
						}
						finally
						{
							this.m_pool.Dispose();
						}
					}

					protected void ThrowIfDisposed()
					{
						Thread.MemoryBarrier();
						this.m_dispose_checker.ThrowIfDisposed();
					}

					#endregion

					protected class Reference : IResourceLink<TReferencedResource>
					{
						[NotNull]
						private readonly ReferencePool<TReferencedResource> m_reference_resource_pool;
						[NotNull]
						private readonly IResourceLink<TReadonlyResource> m_resource_link;
						[NotNull]
						private readonly TReferencedResource m_resource;
						[NotNull]
						private readonly DisposeChecker m_dispose_checker;

						public Reference([NotNull] ReferencePool<TReferencedResource> referenceResourcePool, CancellationToken cancel)
						{
							if (referenceResourcePool == null) throw new ArgumentNullException("referenceResourcePool");

							cancel.ThrowIfCancellationRequested();
							this.m_reference_resource_pool = referenceResourcePool;
							this.m_dispose_checker = DisposeChecker.Create(this);
							try
							{
								cancel.ThrowIfCancellationRequested();
								this.m_resource_link = this.m_reference_resource_pool.m_base_pool.AcquireLink(cancel).ThrowIfNull("resource_link");
								try
								{
									cancel.ThrowIfCancellationRequested();
									this.m_resource = this.m_reference_resource_pool.m_resource_factory(this.m_resource_link.Resource, cancel).ThrowIfNull("resource");
								}
								catch
								{
									this.m_resource_link.Dispose();
									throw;
								}
							}
							catch
							{
								this.m_dispose_checker.Dispose();
								throw;
							}
							Thread.MemoryBarrier();
						}

						#region Члены IResourceLink<TReferencedResource>

						[NotNull]
						public TReferencedResource Resource
						{
							get
							{
								ThrowIfDisposed();
								return this.m_resource;
							}
						}

						#endregion

						#region Члены IDisposable

						public void Dispose()
						{
							Thread.MemoryBarrier();
							try
							{
								this.m_dispose_checker.Dispose();
							}
							finally
							{
								try
								{
									this.m_reference_resource_pool.m_resource_unfactory(this.m_resource);
								}
								finally
								{
									this.m_resource_link.Dispose();
								}
							}
						}

						protected void ThrowIfDisposed()
						{
							Thread.MemoryBarrier();
							this.m_dispose_checker.ThrowIfDisposed();
						}

						#endregion
					}

					protected class Acquirer<TReadonlyResource> : IResourceAcquirer<TReadonlyResource>
					{
						[NotNull] private readonly ReferencePool<TReferencedResource> m_pool;
						[NotNull] private readonly IResourceAcquirer<TReadonlyResource> m_base_acquirer;
						[NotNull] private readonly DisposeChecker m_dispose_checker;

						public Acquirer([NotNull] ReferencePool<TReferencedResource> pool, [NotNull] Func<TReferencedResource, TReadonlyResource> selector, IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> reserveResourceCount, CancellationToken cancel)
						{
							if (pool == null) throw new ArgumentNullException("pool");

							cancel.ThrowIfCancellationRequested();
							this.m_pool = pool;
							this.m_dispose_checker = DisposeChecker.Create(this);
							try
							{
								cancel.ThrowIfCancellationRequested();
								this.m_base_acquirer = this.m_pool.CreateAcquirer(selector, reserveResourceCount, cancel);
							}
							catch
							{
								this.m_dispose_checker.Dispose();
								throw;
							}
						}

						#region Члены IResourceAcquirer<TReferencedResource>

						public virtual IResourceLink<TReadonlyResource> AcquireLink(CancellationToken cancel)
						{
							ThrowIfDisposed();

							cancel.ThrowIfCancellationRequested();
							return new Link(this, cancel);
						}

						public virtual IResourcePool<TReferencedResource2> CreateReferencePool<TReferencedResource2>(
							[NotNull] Func<TReadonlyResource, CancellationToken, TReferencedResource2> resourceFactory,
							int? maxResourceCount,
							[NotNull] IInterval<IEquatableComparableNullable<int>, IEquatableComparableNullable<int>> readyResourceCount,
							[NotNull] ReadonlyParallelOptions parallelOptions)
							where TReferencedResource2 : class, IDisposable
						{
							if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");
							if (readyResourceCount == null) throw new ArgumentNullException("readyResourceCount");
							if (parallelOptions == null) throw new ArgumentNullException("parallelOptions");
							ThrowIfDisposed();

							/*if (maxResourceCount.HasValue)
							{
								if (this.m_pool.m_max_resource_count.HasValue)
									if (maxResourceCount.Value > this.m_pool.m_max_resource_count.Value)
										throw new ArgumentOutOfRangeException("maxResourceCount", maxResourceCount, "> maxResourceCount базового пула");
							}
							else
							{
								if (this.m_pool.m_max_resource_count.HasValue)
									maxResourceCount = this.m_pool.m_max_resource_count;
							}*/

							return new ResourcePoolFactory<TReferencedResource>.Pool.Acquirer<TReadonlyResource>.ReferencePool<TReferencedResource2>(this, resourceFactory, maxResourceCount, readyResourceCount, parallelOptions);
						}

						public void EndAcquire()
						{
							ThrowIfDisposed();
							this.m_base_acquirer.EndAcquire();
						}

						public bool Ended
						{
							get
							{
								ThrowIfDisposed();
								return this.m_base_acquirer.Ended;
							}
						}

						#endregion

						#region Члены IDisposable

						public virtual void Dispose()
						{
							try
							{
								this.m_dispose_checker.Dispose();
							}
							finally
							{
								this.m_base_acquirer.Dispose();
							}
						}

						protected void ThrowIfDisposed()
						{
							Thread.MemoryBarrier();
							this.m_dispose_checker.ThrowIfDisposed();
						}

						#endregion

						protected class Link : IResourceLink<TReadonlyResource>
						{
							[NotNull] private readonly Acquirer<TReadonlyResource> m_acquirer;
							[NotNull] private readonly IResourceLink<TReadonlyResource> m_reference;

							public Link([NotNull] Acquirer<TReadonlyResource> acquirer, CancellationToken cancel)
							{
								if (acquirer == null) throw new ArgumentNullException("acquirer");

								cancel.ThrowIfCancellationRequested();
								this.m_acquirer = acquirer;
								this.m_reference = this.m_acquirer.m_base_acquirer.AcquireLink(cancel);
							}

							#region Члены IResourceLink<TReadonlyResource>

							public TReadonlyResource Resource
							{
								get { return this.m_reference.Resource; }
							}

							#endregion

							#region Члены IDisposable

							public void Dispose()
							{
								this.m_reference.Dispose();
							}

							#endregion
						}
					}
				}
			}
		}
	}
}
