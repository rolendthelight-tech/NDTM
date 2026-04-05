using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	public class HashTableSetWrapper<T> : ISet<T>
	{
		[NotNull] private readonly Hashtable m_hashtable;

		public HashTableSetWrapper(int capacity, [NotNull] IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");

			this.m_hashtable = new Hashtable(capacity, new EqualityComparerWrapper(equalityComparer));
		}

		#region Члены ISet<T>

		public bool Add([NotNull] T item)
		{
			if (item == null) throw new ArgumentNullException("item");
			try
			{
				this.m_hashtable.Add(item, null);
				return true;
			}
			catch (ArgumentException ex)
			{
				if (ex.GetType() != typeof (ArgumentException))
					throw;
				else
					return false;
			}
		}

		public void UnionWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Члены ICollection<T>

		void ICollection<T>.Add([NotNull] T item)
		{
			if (item == null) throw new ArgumentNullException("item");

			this.m_hashtable.Add(item, null);
		}

		public void Clear()
		{
			this.m_hashtable.Clear();
		}

		public bool Contains([NotNull] T item)
		{
			if (item == null) throw new ArgumentNullException("item");

			return this.m_hashtable.Contains(item);
		}

		public void CopyTo([NotNull] T[] array, int arrayIndex)
		{
			if (array == null) throw new ArgumentNullException("array");

			this.m_hashtable.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.m_hashtable.Count; }
		}

		public bool IsReadOnly
		{
			get { return this.m_hashtable.IsReadOnly; }
		}

		public bool Remove([NotNull] T item)
		{
			if (item == null) throw new ArgumentNullException("item");

			int decrease;
			lock (this.m_hashtable.SyncRoot)
			{
				var enumerator =this.m_hashtable.GetEnumerator();
				var old_count = this.m_hashtable.Count;
				this.m_hashtable.Remove(item);
				var new_count = this.m_hashtable.Count;
				enumerator.MoveNext();
				decrease = old_count - new_count;
			}
			if (decrease == 0)
			{
				return false;
			}
			else if (decrease == 1)
			{
				return true;
			}
			else
			{
				throw new ApplicationException("Удалено несколько ключей");
			}
		}

		#endregion

		#region Члены IEnumerable<T>

		public IEnumerator<T> GetEnumerator()
		{
			return this.m_hashtable.Keys.Cast<T>().GetEnumerator();
		}

		#endregion

		#region Члены IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region Члены IReadOnlySet<T>

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			throw new NotImplementedException();
		}

		#endregion

		private class EqualityComparerWrapper : EqualityComparer<T>
		{
			[NotNull]
			private readonly IEqualityComparer<T> m_equality_comparer;

			[Pure]
			public EqualityComparerWrapper([NotNull] IEqualityComparer<T> equalityComparer)
			{
				if (equalityComparer == null) throw new ArgumentNullException("equalityComparer");

				this.m_equality_comparer = equalityComparer;
			}

			public override bool Equals(T x, T y)
			{
				return this.m_equality_comparer.Equals(x, y);
			}

			public override int GetHashCode(T obj)
			{
				return this.m_equality_comparer.GetHashCode(obj);
			}
		}
	}
}
