using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridTest
{
  using System.ComponentModel;
  using System.Data;
  using System.Windows.Forms;
  using AT.Toolbox.Log;

  public class ATBindingSource : BindingSource 
  {
    #region Log -----------------------------------------------------------------------------------------------------------------

    public static readonly ILog Log = AT.Toolbox.Log.Log.GetLogger("Binding");

    #endregion

    protected ListChangedEventArgs m_prev_action;
    protected Stack<BindingAction> m_undos = new Stack<BindingAction>();
    private Stack<BindingAction> m_redos = new Stack<BindingAction>();
    public bool InAction = false ;

    public Dictionary<object, List<BindingAction>> ObjectMap = new Dictionary<object, List<BindingAction>>();

    public ATBindingSource(IContainer components)
      : base(components)
    {
    }

    protected override void OnListChanged(ListChangedEventArgs e)
    {
      Log.Debug(@"Event :" + e.ListChangedType + " " + e.NewIndex + " " + e.OldIndex);
      base.OnListChanged(e);
    }

    public override int Add(object value)
    {
      if (!InAction)
      {
        BindingAction ac = new NewItemAction(this, value);
        m_undos.Push(ac);
      }

      return base.Add(value);
    }

    public override object AddNew()
    {
      object o = base.AddNew();

      if (!InAction)
      {
        BindingAction ac = new NewItemAction(this, o);
        m_undos.Push(ac);
      }

      return o;
    }

    public override void Remove(object value)
    {
      if (!InAction)
      {
        BindingAction ac = new DeleteItemAction(this, value);
        m_undos.Push(ac);
      }

      base.Remove(value);
    }

    public override void RemoveAt(int index)
    {
      if (!InAction)
      {
        BindingAction ac = new DeleteItemAction(this, this[index]);
        m_undos.Push(ac);
      }

      base.RemoveAt(index);
    }

    public void Undo()
    {
      if( m_undos.Count == 0 )
        return;

      InAction = true;
      BindingAction ac = m_undos.Pop();
      ac.Undo();
      m_redos.Push(ac);
      InAction = false;
    }

    public void Redo()
    {
      if (m_redos.Count == 0)
        return;

      InAction = true;
      BindingAction ac = m_redos.Pop();
      ac.Redo();
      m_undos.Push(ac);
      InAction = false;
    }

    public void Remap(object OldValue, object NewValue)
    {
      if (!ObjectMap.ContainsKey(OldValue))
        return;

      foreach (BindingAction ac in ObjectMap[OldValue])
        ac.Value = NewValue;

      if (!ObjectMap.ContainsKey(NewValue))
        ObjectMap.Add(NewValue, new List<BindingAction>());

      foreach (BindingAction ac in ObjectMap[OldValue])
        ObjectMap[NewValue].Add(ac);

      ObjectMap[OldValue].Clear();
      ObjectMap.Remove(OldValue);
    }
  }

  public  class DeleteItemAction : BindingAction
  {
    public DeleteItemAction(ATBindingSource Parent, object Value)
      : base(Parent, Value)
    {
      ATBindingSource.Log.Debug("DeleteItemAction created");

      if (m_value is DataRowView)
      {
        DataRowView v = m_value as DataRowView;
        m_value_data = v.Row.ItemArray;
      }
    }

    public override string DisplayName
    {
      get { return "Delete Row"; }
    }

    public override void Undo()
    {
      ATBindingSource.Log.Debug("DeleteItemAction::UNDO()");

      if (m_value is DataRowView)
      {
        DataRowView new_rw = m_parent.AddNew() as DataRowView;
        new_rw.Row.ItemArray = m_value_data;
        m_parent.Remap(m_value, new_rw);
      }
    }

    public override void Redo()
    {
      ATBindingSource.Log.Debug("DeleteItemAction::REDO()");
      m_parent.Remove(m_value);
    }
  }

  public abstract class BindingAction
  {
    protected object m_value;
    protected object[] m_value_data;

    protected ATBindingSource m_parent;

    public abstract string DisplayName {get;}

    public object Value
    {
      get { return m_value; }
      set
      {
        m_value = value;

        if (m_value is DataRowView)
        {
          DataRowView v = m_value as DataRowView;
          m_value_data = v.Row.ItemArray;
        }
      }
    }

    protected BindingAction( ATBindingSource Parent, object Value )
    {
      m_parent = Parent;
      m_value = Value;

      if( !Parent.ObjectMap.ContainsKey(Value))
        Parent.ObjectMap.Add(Value, new List<BindingAction>());

      Parent.ObjectMap[Value].Add(this);
    }

    public abstract void Undo();

    public abstract void Redo();
  }

  class NewItemAction : BindingAction
  {
    public NewItemAction(ATBindingSource Parent, object Value)
      : base(Parent, Value)
    {
      ATBindingSource.Log.Debug("NewItemAction created");

      if (m_value is DataRowView)
      {
        DataRowView v = m_value as DataRowView;
        m_value_data = v.Row.ItemArray;
      }
    }

    public override string DisplayName
    {
      get { return "Add Row"; }
    }

    public override void Undo()
    {
      ATBindingSource.Log.Debug("NewItemAction::UNDO()");
      m_parent.Remove(m_value);
    }

    public override void Redo()
    {

      ATBindingSource.Log.Debug("NewItemAction::REDO()");

      if( m_value is DataRowView )
      {
        DataRowView new_rw = m_parent.AddNew() as DataRowView;
        new_rw.Row.ItemArray = m_value_data;

        m_parent.Remap(m_value, new_rw);
      }
    }
  }

}
