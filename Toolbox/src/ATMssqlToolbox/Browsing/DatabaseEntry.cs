using System.Drawing;
using System.Linq.Expressions;
using System;
using System.ComponentModel;
using AT.Toolbox.Constants;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AT.Toolbox.MSSQL
{
  [DataContract]
  public class DatabaseEntry : INotifyPropertyChanged
  {
    private bool used;
    private string m_version;

    public DatabaseEntry()
    {
      PropertyChanged += delegate { };
    }

    [DataMember]
    [DefaultValue("")]
    public string Name { get; set; }

    [DataMember]
    [DefaultValue(0)]
    public int Supported { get; set; }

    [DataMember]
    [DefaultValue("0.0.0.0")]
    [ReadOnly(true)]
    public string Version 
    {
      get { return m_version; }
      set
      {
        m_version = value;
        InvokePropertyChanged(() => this.Version);
      }
    }

    public string TestResults { get; set; }

    public bool Used
    {
      get
      {
        return used;
      }
      set
      {
        if (used != value)
        {
          used = value;
          InvokePropertyChanged(() => this.Used);
        }
      }
    }

    public Image SupportedPic
    {
      get
      {
        if (Supported < 0 || Supported > (int)Support.Full)
          return Properties.Resources.p_16_error;

        Support s = (Support)Supported;

        switch (s)
        {
          case Support.Full:
            return Properties.Resources.p_16_ok;
          case Support.Partial:
            return Properties.Resources.p_16_warning;
          case Support.Unknown:
            return Properties.Resources.p_16_help;
          case Support.None:
            return Properties.Resources.p_16_error;
          default:
            return Properties.Resources.p_16_error;
        }
      }
    }

    private void InvokePropertyChanged<T>(Expression<Func<T>> property)
    {
      if (this.PropertyChanged != null)
      {
        MemberExpression expression = (MemberExpression)property.Body;
        this.PropertyChanged(this, new PropertyChangedEventArgs(expression.Member.Name));
      }
    }

    public void ApplySupportInfo(SupportInfo support_info)
    {
      this.Version = support_info.Version;
      this.Supported = (int)support_info.Supported;
      this.TestResults = support_info.TestResults;
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}