using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using AT.Toolbox.Log;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает выполнение операций по привязке данных на сервере приложения
  /// </summary>
  public interface IServiceRecord : IPackable, IDisposable
  {
    /// <summary>
    /// Обновляет данные в БД
    /// </summary>
    /// <param name="result"></param>
    void UpdateData(BindingResult result);
  }

  /// <summary>
  /// Интерфейс, необходимый для возможности подключения одного приложения к разным базам
  /// </summary>
  public interface ICustomRecordBinderProvider
  {
    IRecordBinder GetRecordBinder(Dictionary<BindingAction, string> procedureActions);
  }

  [Serializable]
  //[DataContract]
  public sealed class BindingResult : IPackable
  {
    Dictionary<string, object> m_fields = new Dictionary<string, object>();
    InfoCollection m_messages = new InfoCollection();

    public BindingResult(BindingAction action)
    {
      this.BindingAction = action;
    }

    public BindingResult() : this(BindingAction.Execute) { }

    public InfoCollection Messages
    {
      get { return m_messages; }
    }

    [DataMember]
    public Dictionary<string, object> Fields
    {
      get { return m_fields; }
    }

    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public bool RebindRequired { get; set; }

    [DataMember]
    public BindingAction BindingAction { get; private set; }

    internal void BindMessages(InfoEventArgs e)
    {
      m_messages = e.Messages;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      bool first = true;
      foreach (KeyValuePair<string, object> kv in m_fields)
      {
        if (first)
        {
          first = false;
        }
        else
        {
          sb.Append(", ");
        }
        sb.Append(kv.Key + " -> " + kv.Value);
      }

      return "Action: " + this.BindingAction + ", Success: " + this.Success
        + ", Field values: " + sb.ToString();
    }

    #region IPackable Members

    byte[] IPackable.Pack()
    {
      Dictionary<string, object> fields = new Dictionary<string, object>(m_fields);

      fields.Add("#_ok", this.Success);
      fields.Add("#_act", (byte)this.BindingAction);
      fields.Add("#_msgs", m_messages.Count);
      fields.Add("#_reb", this.RebindRequired);

      for (int i = 0; i < m_messages.Count; i++)
      {
        fields.Add("#_msg" + i, m_messages[i]);
      }
      return PackHelper.PackFieldValues(fields);
    }

    bool IPackable.Unpack(byte[] data)
    {
      try
      {
        List<string> skip = new List<string>() { "#_ok", "#_act", "#_msgs", "#_reb" };
        Dictionary<string, object> fields = PackHelper.UnpackFieldValues(data);

        this.Success = (bool)fields["#_ok"];
        this.BindingAction = (BindingAction)fields["#_act"];
        this.RebindRequired = (bool)fields["#_reb"];

        int messageCount = (int)fields["#_msgs"];
        for (int i = 0; i < messageCount; i++)
        {
          m_messages.Add((Info)fields["#_msg" + i]);
          skip.Add("#_msg" + i);
        }

        foreach (KeyValuePair<string, object> kv in fields)
        {
          if (skip.Contains(kv.Key))
            continue;

          m_fields.Add(kv.Key, kv.Value);
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

    #endregion
  }


  public class RecordServiceClientStub
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(RecordServiceClientStub).Name);

    public BindingResult GetResult(IServiceRecord record, BindingAction action)
    {
      ICustomRecordBinderProvider customiser = record as ICustomRecordBinderProvider;

      using (IRecordBinder bdr = RecordManager.GetRecordBinder(new Dictionary<BindingAction, string>(), customiser))
      {
        if (bdr == null)
        {
          return null;
        }
        BindingResult result = new BindingResult(action);
        bdr.BeginTransaction(IsolationLevel.RepeatableRead);
        try
        {
          record.UpdateData(result);
          if (result.Success)
          {
            bdr.CommitTransaction();
          }
          else
          {
            bdr.RollbackTransaction();
          }
          return result;
        }
        catch (Exception ex)
        {
          bdr.RollbackTransaction();
          throw;
        }
      }
    }
  }
}
