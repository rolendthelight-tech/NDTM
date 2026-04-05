using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace AT.Toolbox.MSSQL
{
  [Serializable]
  public class ObjectTextStore
  {
    public ObjectTextStore() { }

    public ObjectTextStore(object value, string text)
    {
      this.Value = value;
      this.Text = text;
    }

    public object Value { get; set; }

    public string Text { get; set; }

    public override string ToString()
    {
      return this.Text;
    }
  }
}
