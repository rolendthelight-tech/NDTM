using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	/// <summary>
	/// Множество с учётом количества вхождений элементов.
	/// </summary>
	/// <typeparam name="TKey">Тип элементов.</typeparam>
	[DebuggerDisplay("Count = {Count}, TotalCount = {TotalCount}")]
	public class CountSet<TKey> : ICountSet<TKey>
	{
		[NotNull] private readonly Dictionary<TKey, int> m_dict;
		private int m_total_count;

		/// <summary>
		/// Создаёт множество с учётом количества вхождений элементов, которое содержит элементы, скопированные из заданного множества.
		/// </summary>
		/// <param name="source">Множество для копирования.</param>
		/// <param name="comparer">Компаратор.</param>
		[Pure]
		public CountSet([NotNull] CountSet<TKey> source, [CanBeNull] IEqualityComparer<TKey> comparer = null)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (source.m_dict == null) throw new ArgumentException("m_dict is null", "source");
			if (source.m_total_count < 0) throw new ArgumentException("m_total_count is negative", "source");

			this.m_dict = new Dictionary<TKey, int>(source.m_dict, comparer);
			this.m_total_count = source.m_total_count;
		}

		/// <summary>
		/// Создаёт множество с учётом количества вхождений элементов.
		/// </summary>
		/// <param name="capacity">Начальная вместимость.</param>
		/// <param name="comparer">Компаратор.</param>
		[Pure]
		public CountSet(int capacity = 0, [CanBeNull] IEqualityComparer<TKey> comparer = null)
		{
			this.m_dict = new Dictionary<TKey, int>(capacity, comparer);
			this.m_total_count = 0;
		}

		#region Implementation of IEnumerable

		public virtual IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
		{
			return this.m_dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Члены ICountSet<TKey>

		public virtual int TotalCount
		{
			get { return this.m_total_count; }
		}

		public virtual void Increment(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			Increment(key, 1);
		}

		public virtual void Increment(TKey key, int count)
		{
			checked
			{
				if (key == null) throw new ArgumentNullException("key");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");

				if (this.m_total_count < 0)
					throw new ApplicationException("TotalCount is negative");

				int value;
				if (this.m_dict.TryGetValue(key, out value))
				{
					if (value <= 0)
						throw new ApplicationException("count is negative");
					else
					{
						value += count;
						this.m_total_count += count;
						try
						{
							this.m_dict[key] = value;
						}
						catch
						{
							this.m_total_count -= count;
							throw;
						}
					}
				}
				else
				{
					value = count;
					this.m_total_count += count;
					try
					{
						this.m_dict.Add(key, value);
					}
					catch
					{
						this.m_total_count -= count;
						throw;
					}
				}
			}
		}

		public virtual void Decrement(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			Decrement(key, 1);
		}

		public virtual void Decrement(TKey key, int count)
		{
			checked
			{
				if (key == null) throw new ArgumentNullException("key");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");

				if (this.m_total_count < 0)
					throw new ApplicationException("TotalCount is negative");

				int value;
				if (this.m_dict.TryGetValue(key, out value))
				{
					if (value <= 0)
						throw new ApplicationException("count is negative");
					else
					{
						value -= count;
						this.m_total_count -= count;
						try
						{
							if (value == 0)
							{
								if (!this.m_dict.Remove(key))
									throw new ApplicationException("key not removed");
							}
							else
							{
								this.m_dict[key] = value;
							}
						}
						catch
						{
							this.m_total_count += count;
							throw;
						}
					}
				}
				else
				{
					throw new InvalidOperationException("key not found");
				}
			}
		}

		public virtual bool TryIncrement(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return TryIncrement(key, 1);
		}

		public virtual bool TryIncrement(TKey key, int count)
		{
			checked
			{
				if (key == null) throw new ArgumentNullException("key");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");

				if (this.m_total_count < 0)
					throw new ApplicationException("TotalCount is negative");

				int value;
				if (this.m_dict.TryGetValue(key, out value))
				{
					if (value <= 0)
						throw new ApplicationException("count is negative");
					else
					{
						value += count;
						this.m_total_count += count;
						try
						{
							this.m_dict[key] = value;
						}
						catch
						{
							this.m_total_count -= count;
							throw;
						}
						return true;
					}
				}
				else
				{
					value = count;
					this.m_total_count += count;
					try
					{
						this.m_dict.Add(key, value);
					}
					catch
					{
						this.m_total_count -= count;
						throw;
					}
					return true;
				}
			}
		}

		public virtual bool TryDecrement(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return TryDecrement(key, 1);
		}

		public virtual bool TryDecrement(TKey key, int count)
		{
			checked
			{
				if (key == null) throw new ArgumentNullException("key");
				if (count < 0) throw new ArgumentOutOfRangeException("count", count, "< 0");

				if (this.m_total_count < 0)
					throw new ApplicationException("TotalCount is negative");

				int value;
				if (this.m_dict.TryGetValue(key, out value))
				{
					if (value <= 0)
						throw new ApplicationException("count is negative");
					else
					{
						value -= count;
						this.m_total_count -= count;
						try
						{
							if (value == 0)
							{
								if (!this.m_dict.Remove(key))
									throw new ApplicationException("key not removed");
								else
									return true;
							}
							else
							{
								this.m_dict[key] = value;
								return true;
							}
						}
						catch
						{
							this.m_total_count += count;
							throw;
						}
					}
				}
				else
				{
					return false;
				}
			}
		}

		public virtual int this[TKey key]
		{
			get
			{
				if (key == null) throw new ArgumentNullException("key");

				int value;
				if (this.m_dict.TryGetValue(key, out value))
				{
					if (value <= 0)
						throw new ApplicationException("count is negative");
					else
					{
						return value;
					}
				}
				else
				{
					return 0;
				}
			}
		}

		public virtual bool Contains(TKey key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return this.m_dict.ContainsKey(key);
		}

		[NotNull]
		public virtual IDictionary<TKey, int> ToDictionary()
	  {
		  return new ReadOnlyDictionary<TKey, int>(new Dictionary<TKey, int>(this.m_dict));
	  }

		#endregion

		#region Implementation of ICollection

		public virtual void CopyTo(Array array, int index)
		{
			if (array == null) throw new ArgumentNullException("array");

			((ICollection)this.m_dict).CopyTo(array, index);
		}

		public virtual int Count
		{
			get { return ((ICollection)this.m_dict).Count; }
		}

		public virtual object SyncRoot
		{
			get { return ((ICollection) this.m_dict).SyncRoot; }
		}

		public virtual bool IsSynchronized
		{
			get { return ((ICollection) this.m_dict).IsSynchronized; }
		}

		#endregion
	}
}
