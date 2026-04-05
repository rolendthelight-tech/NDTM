using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace AT.Toolbox
{
  public class NestedBindingList<TType, TParent> : BindingList<TType>
    where TType : class, INestedObject<TParent>
    where TParent: class, ICommandHistoryOwner
  {
    private readonly TParent m_parent;

    public NestedBindingList(TParent parent)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");

      m_parent = parent;
    }

    public NestedBindingList(TParent parent, IList<TType> items)
      : base(items)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");

      m_parent = parent;
    }

    public TParent Parent
    {
      get { return m_parent; }
    }

    protected override object AddNewCore()
    {
      var ret = base.AddNewCore();

      INestedObject<TParent> item = ret as INestedObject<TParent>;

      if (item != null)
        item.Parent = m_parent;

      return ret;
    }

    protected override void ClearItems()
    {
      var history = m_parent.History;
      if (history != null)
      {
        history.AddCommand(new ClearCommand<TType>
        {
          Items = this.Items.ToList(),
          List = this
        });
      }      foreach (var item in this.Items)
        item.Parent = null;

      base.ClearItems();
    }

    protected override void InsertItem(int index, TType item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      var history = m_parent.History;
      if (history != null)
      {
        history.AddCommand(new ItemAddCommand
        {
          Index = index,
          Item = item,
          List = this
        });
      }

      item.Parent = m_parent;
      base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
      var item = this.Items[index];

      var history = m_parent.History;
      if (history != null)
      {
        history.AddCommand(new ItemRemoveCommand
        {
          Index = index,
          Item = item,
          List = this
        });
      }

      item.Parent = null;

      base.RemoveItem(index);
    }

    protected override void SetItem(int index, TType item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      var history = m_parent.History;

      if (history != null)
      {
        history.AddCommand(new ItemReplaceCommand
        {
          Index = index,
          OldItem = this[index],
          NewItem = item,
          List = this
        });
      }

      item.Parent = m_parent;
      base.SetItem(index, item);
    }
  }

  public interface INestedObject<TParent>
    where TParent : class
  {
    TParent Parent { get; set; }
  }

  class PropertyChangedCommand : ICommand
  {
    public IPropertyTracker Item { get; set; }

    public string PropertyName { get; set; }

    public object OldValue { get; set; }

    public object NewValue { get; set; }

    #region ICommand Members

    public bool TrackingRequired
    {
      get
      {
        if (this.Item == null || string.IsNullOrEmpty(this.PropertyName))
          return false;

        if (this.OldValue == null)
          return this.NewValue != null;

        return !this.OldValue.Equals(this.NewValue);
      }
    }

    public void Undo()
    {
      if (this.Item == null || string.IsNullOrEmpty(this.PropertyName))
        return;

      this.Item[PropertyName] = this.OldValue;
      this.Item.RaisePropertyChanged(this.PropertyName);
    }

    public void Redo()
    {
      if (this.Item == null || string.IsNullOrEmpty(this.PropertyName))
        return;

      this.Item[PropertyName] = this.NewValue;
      this.Item.RaisePropertyChanged(this.PropertyName);
    }

    public override string ToString()
    {
      List<string> list = new List<string>();

      list.Add("Property change command");
      list.Add("Item: " + this.Item);
      list.Add("Property: " + this.PropertyName);
      list.Add("Old value: " + this.OldValue);
      list.Add("New value: " + this.NewValue);

      return string.Join("; ", list.ToArray());
    }

    #endregion
  }

  public class ItemAddCommand : ICommand
  {
    public object Item { get; set; }

    public IBindingList List { get; set; }

    public int Index { get; set; }

    #region ICommand Members

    public bool TrackingRequired
    {
      get { return this.List != null && this.Item != null; }
    }

    public void Undo()
    {
      this.List.Remove(this.Item);
    }

    public void Redo()
    {
      this.List.Insert(this.Index, this.Item);
    }

    public override string ToString()
    {
      List<string> list = new List<string>();

      list.Add("Add item command");
      list.Add("Item: " + this.Item);
      list.Add("List: " + this.List);
      list.Add("Index: " + this.Index);

      return string.Join("; ", list.ToArray());
    }

    #endregion
  }

  public class ItemRemoveCommand : ICommand
  {
    public object Item { get; set; }

    public IBindingList List { get; set; }

    public int Index { get; set; }

    #region ICommand Members

    public bool TrackingRequired
    {
      get { return this.List != null && this.Item != null; }
    }

    public void Undo()
    {
      this.List.Insert(this.Index, this.Item);
    }

    public void Redo()
    {
      this.List.Remove(this.Item);
    }

    public override string ToString()
    {
      List<string> list = new List<string>();

      list.Add("Remove item command");
      list.Add("Item: " + this.Item);
      list.Add("List: " + this.List);
      list.Add("Index: " + this.Index);

      return string.Join("; ", list.ToArray());
    }

    #endregion
  }

  class ItemReplaceCommand : ICommand
  {
    public object OldItem { get; set; }

    public object NewItem { get; set; }

    public IBindingList List { get; set; }

    public int Index { get; set; }

    #region ICommand Members

    public bool TrackingRequired
    {
      get { return this.OldItem != null && this.NewItem != null && this.List != null; }
    }

    public void Undo()
    {
      this.List[this.Index] = this.OldItem;
    }

    public void Redo()
    {
      this.List[this.Index] = this.NewItem;
    }

    public override string ToString()
    {
      List<string> list = new List<string>();

      list.Add("Add item command");
      list.Add("List: " + this.List);
      list.Add("Old item: " + this.OldItem);
      list.Add("New item: " + this.NewItem);
      list.Add("Index: " + this.Index);

      return string.Join("; ", list.ToArray());
    }

    #endregion
  }

  public class ClearCommand<TType> : ICommand
  {
    public IBindingList List { get; set; }

    public IList<TType> Items { get; set; }

    #region ICommand Members

    public bool TrackingRequired
    {
      get { return this.List != null && this.Items != null && this.Items.Count > 0; }
    }

    public void Undo()
    {
      foreach (var item in this.Items)
      {
        this.List.Add(item);
      }
    }

    public void Redo()
    {
      this.List.Clear();
    }

    #endregion
  }
}
