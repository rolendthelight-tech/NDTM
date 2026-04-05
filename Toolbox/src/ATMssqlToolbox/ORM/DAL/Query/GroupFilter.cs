using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [Serializable]
  public class GroupFilter : FilterBase, IList<FilterBase>, ICollection
  {
    public GroupFilter(bool conjunction)
    {
      this.Conjunction = conjunction;
    }
    
    public bool Conjunction { get; set; }

    private List<FilterBase> nestedFilters = new List<FilterBase>();

    public override QueryObject Query
    {
      get
      {
        return base.Query;
      }
      set
      {
        base.Query = value;
        foreach (FilterBase filter in this.nestedFilters)
        {
          filter.Query = value;
        }
      }
    }

    public override bool Enabled
    {
      get
      {
        return this.nestedFilters.Count > 0;
      }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }
    
    public int Count
    {
      get
      {
        return nestedFilters.Count;
      }
    }

    public FilterBase this[int i]
    {
      get
      {
        return nestedFilters[i];
      }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }
        nestedFilters[i] = value;
        nestedFilters[i].Query = this.Query;
      }
    }

    public void Add(FilterBase filter)
    {
      if (filter == null)
      {
        throw new ArgumentNullException("filter");
      }
      filter.Query = this.Query;
      nestedFilters.Add(filter);
    }

    public void Insert(int index, FilterBase filter)
    {
      if (filter == null)
      {
        throw new ArgumentNullException("filter");
      }
      filter.Query = this.Query;
      nestedFilters.Insert(index, filter);
    }

    public bool Remove(FilterBase filter)
    {
      return nestedFilters.Remove(filter);
    }

    public void RemoveAt(int index)
    {
      nestedFilters.RemoveAt(index);
    }

    public void Clear()
    {
      nestedFilters.Clear();
    }

    public bool Contains(FilterBase filter)
    {
      return nestedFilters.Contains(filter);
    }

    public int IndexOf(FilterBase filter)
    {
      return nestedFilters.IndexOf(filter);
    }

    public override void DoBuildFilter(IQueryBuilder builder)
    {
      if (this.nestedFilters.Count > 0)
      {
        builder.StartBlock();
        for (int i = 0; i < this.nestedFilters.Count; i++)
        {
          if (i > 0)
          {
            builder.BuildSeparator(this.Conjunction);
          }
          this.nestedFilters[i].BuildFilter(builder);
        }
        builder.EndBlock();
      }
    }

    public override FilterBase Clone(QueryObject newQuery)
    {
      GroupFilter ret = base.Clone(newQuery) as GroupFilter;

      if (!ReferenceEquals(ret.nestedFilters, this.nestedFilters))
      {
        foreach (FilterBase flt in this.nestedFilters)
        {
          ret.nestedFilters.Add(flt.Clone(newQuery));
        }
      }

      return ret;
    }

    #region IEnumerable Members

    public IEnumerator<FilterBase> GetEnumerator()
    {
      return nestedFilters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return nestedFilters.GetEnumerator();
    }

    #endregion

    #region ICollection Members

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotSupportedException();
    }

    void ICollection<FilterBase>.CopyTo(FilterBase[] array, int arrayIndex)
    {
      throw new NotSupportedException();
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)nestedFilters).SyncRoot; }
    }

    #endregion
  }
}
