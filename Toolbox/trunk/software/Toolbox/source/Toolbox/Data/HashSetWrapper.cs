using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	public class HashSetWrapper<T> : ISet<T>
	{
		[NotNull] private readonly HashSet<T> m_source;

		[Pure]
		public HashSetWrapper([NotNull] HashSet<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			this.m_source = source;
		}

		[NotNull]
		public static implicit operator HashSetWrapper<T>([NotNull] HashSet<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return new HashSetWrapper<T>(source);
		}

		#region Implementation of IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return this.m_source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection<T>

		void ICollection<T>.Add(T item)
		{
			((ICollection<T>)this.m_source).Add(item);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			this.m_source.UnionWith(other);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			this.m_source.IntersectWith(other);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			this.m_source.ExceptWith(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			this.m_source.SymmetricExceptWith(other);
		}

		public bool Add(T item)
		{
			return this.m_source.Add(item);
		}

		public void Clear()
		{
			this.m_source.Clear();
		}

		public bool Contains(T item)
		{
			return this.m_source.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_source.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return this.m_source.Remove(item);
		}

		int ICollection<T>.Count
		{
			get { return ((ICollection<T>) this.m_source).Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<T>) this.m_source).IsReadOnly; }
		}

		#endregion

		#region Implementation of IReadOnlyCollection<out T>

		int IReadOnlyCollection<T>.Count
		{
			get { return ((ICollection<T>) this).Count; }
		}

		#endregion

		#region Implementation of IReadOnlySet<T>

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return this.m_source.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return this.m_source.IsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return this.m_source.IsProperSupersetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return this.m_source.IsProperSubsetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return this.m_source.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return this.m_source.SetEquals(other);
		}

		#endregion
	}
}
