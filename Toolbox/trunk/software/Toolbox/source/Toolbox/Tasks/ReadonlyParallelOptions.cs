using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Toolbox.Tasks
{
	/// <summary>
	/// Хранит параметры параллелизма.
	/// </summary>
	public class ReadonlyParallelOptions : Tuple<CancellationToken, int?, TaskScheduler>
	{
		/// <summary>
		/// Копирует параметры параллелизма.
		/// </summary>
		/// <param name="source">Исходные параметры параллелизма.</param>
		public ReadonlyParallelOptions([NotNull] ReadonlyParallelOptions source)
			: this(cancellationToken: source.CancellationToken, maxDegreeOfParallelism: source.MaxDegreeOfParallelism, taskScheduler: source.TaskScheduler)
		{
			if (source == null) throw new ArgumentNullException("source");
		}

		/// <summary>
		/// Преобразует параметры параллелизма.
		/// </summary>
		/// <param name="source">Исходные параметры параллелизма.</param>
		public ReadonlyParallelOptions([NotNull] System.Threading.Tasks.ParallelOptions source)
			: this(cancellationToken: source.CancellationToken, maxDegreeOfParallelism: source.MaxDegreeOfParallelism < 0 ? (int?)null : source.MaxDegreeOfParallelism, taskScheduler: source.TaskScheduler)
		{
			if (source == null) throw new ArgumentNullException("source");
		}

		/// <summary>
		/// Создаёт параметры параллелизма.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены.</param>
		/// <param name="maxDegreeOfParallelism">Максимальная степень параллельности. (<c><value>null</value></c> если не ограничено.)</param>
		/// <param name="taskScheduler">Планировщик заданий.</param>
		public ReadonlyParallelOptions(CancellationToken cancellationToken, int? maxDegreeOfParallelism, [NotNull] TaskScheduler taskScheduler)
			: base(cancellationToken, maxDegreeOfParallelism, taskScheduler)
		{
			if (maxDegreeOfParallelism < 0) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism", maxDegreeOfParallelism, "< 0");
			if (taskScheduler == null) throw new ArgumentNullException("taskScheduler");
		}

		/// <summary>
		/// Токен отмены.
		/// </summary>
		public CancellationToken CancellationToken
		{
			get { return this.Item1; }
		}

		/// <summary>
		/// Максимальная степень параллельности. (<c><value>null</value></c> если не ограничено.)
		/// </summary>
		public int? MaxDegreeOfParallelism
		{
			get { return this.Item2; }
		}

		/// <summary>
		/// Планировщик заданий.
		/// </summary>
		[NotNull]
		public TaskScheduler TaskScheduler
		{
			get { return this.Item3; }
		}

		/// <summary>
		/// Преобразует параметры параллелизма из <see cref="T:System.Threading.Tasks.ParallelOptions"/>.
		/// </summary>
		/// <param name="value">Исходные параметры параллелизма.</param>
		/// <returns>Преобразованные параметры параллелизма.</returns>
		[CanBeNull]
		public static implicit operator ReadonlyParallelOptions([CanBeNull] System.Threading.Tasks.ParallelOptions value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return new ReadonlyParallelOptions(value);
			}
		}

		/// <summary>
		/// Преобразует параметры параллелизма в <see cref="T:System.Threading.Tasks.ParallelOptions"/>.
		/// </summary>
		/// <param name="value">Исходные параметры параллелизма.</param>
		/// <returns>Преобразованные параметры параллелизма.</returns>
		[CanBeNull]
		public static explicit operator System.Threading.Tasks.ParallelOptions([CanBeNull] ReadonlyParallelOptions value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return new ParallelOptions()
				{
					CancellationToken = value.CancellationToken,
					MaxDegreeOfParallelism = value.MaxDegreeOfParallelism == null ? -1 : value.MaxDegreeOfParallelism.Value,
					TaskScheduler = value.TaskScheduler,
				};
			}
		}
	}
}