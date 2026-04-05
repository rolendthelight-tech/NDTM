using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Data
{
	public class ListWrapper<T> : IList<T>, IReadOnlyList<T>
	{
		[NotNull] private readonly List<T> m_source;

		[Pure]
		public ListWrapper([NotNull] List<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			this.m_source = source;
		}

		[NotNull]
		public static implicit operator ListWrapper<T>([NotNull] List<T> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return new ListWrapper<T>(source);
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

		#region Члены IList<T>

		public int IndexOf(T item)
		{
			return this.m_source.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.m_source.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.m_source.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return this.m_source[index]; }
			set { this.m_source[index] = value; }
		}

		#endregion
	}
}
