using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Синхронизированное множество с учётом количества вхождений элементов.
	/// </summary>
	/// <typeparam name="TKey">Тип элементов.</typeparam>
	public class SynchronizedCountSet<TKey> : CountSet<TKey>, ICountSet<TKey>
	{
		[CanBeNull] private object m_sync_root;

		/// <summary>
		/// Создаёт синхронизированное множество с учётом количества вхождений элементов, которое содержит элементы, скопированные из заданного множества.
		/// </summary>
		/// <param name="source">Множество для копирования.</param>
		/// <param name="comparer">Компаратор.</param>
		[Pure]
		public SynchronizedCountSet([NotNull] CountSet<TKey> source, [CanBeNull] IEqualityComparer<TKey> comparer = null)
			: base(source: source, comparer: comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
		}

		/// <summary>
		/// Создаёт синхронизированное множество с учётом количества вхождений элементов.
		/// </summary>
		/// <param name="capacity">Начальная вместимость.</param>
		/// <param name="comparer">Компаратор.</param>
		[Pure]
		public SynchronizedCountSet(int capacity = 0, [CanBeNull] IEqualityComparer<TKey> comparer = null)
			: base(capacity: capacity, comparer: comparer)
		{
		}

		#region Overrides of CountSet<TKey>

		public override int TotalCount
		{
			get
			{
				lock (this.SyncRoot)
					return base.TotalCount;
			}
		}

		public override void Increment(TKey key)
		{
			lock (this.SyncRoot)
				base.Increment(key);
		}

		public override void Increment(TKey key, int count)
		{
			lock (this.SyncRoot)
				base.Increment(key, count);
		}

		public override void Decrement(TKey key)
		{
			lock (this.SyncRoot)
				base.Decrement(key);
		}

		public override void Decrement(TKey key, int count)
		{
			lock (this.SyncRoot)
				base.Decrement(key, count);
		}

		public override bool TryIncrement(TKey key)
		{
			lock (this.SyncRoot)
				return base.TryIncrement(key);
		}

		public override bool TryIncrement(TKey key, int count)
		{
			lock (this.SyncRoot)
				return base.TryIncrement(key, count);
		}

		public override bool TryDecrement(TKey key)
		{
			lock (this.SyncRoot)
				return base.TryDecrement(key);
		}

		public override bool TryDecrement(TKey key, int count)
		{
			lock (this.SyncRoot)
				return base.TryDecrement(key, count);
		}

		public override int this[TKey key]
		{
			get
			{
				lock (this.SyncRoot)
					return base[key];
			}
		}

		public override bool Contains(TKey key)
		{
			lock (this.SyncRoot)
				return base.Contains(key);
		}

		public override void CopyTo(Array array, int index)
		{
			lock (this.SyncRoot)
				base.CopyTo(array, index);
		}

		public override IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override int Count
		{
			get
			{
				lock (this.SyncRoot)
					return base.Count;
			}
		}

		public override object SyncRoot
		{
			get { return base.SyncRoot ?? this.m_sync_root ?? CreateSyncRoot(); }
		}

		public override bool IsSynchronized
		{
			get { return true; }
		}

		[NotNull]
		public override IDictionary<TKey, int> ToDictionary()
		{
			lock (this.SyncRoot)
				return base.ToDictionary();
		}

		#endregion

		[NotNull]
		private object CreateSyncRoot()
		{
			if (this.m_sync_root != null)
			{
				return this.m_sync_root;
			}
			else
			{
				var sync_root = new object();
				return Interlocked.CompareExchange(ref this.m_sync_root, sync_root, null);
			}
		}
	}
}