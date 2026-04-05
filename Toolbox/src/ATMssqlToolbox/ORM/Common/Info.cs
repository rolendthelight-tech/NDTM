using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace AT.Toolbox.MSSQL
{
  [Serializable]
  public class Info
  {
    public Info(string message, InfoLevel state)
    {
      this.Message = message;
      this.InfoLevel = state;
      this.Description = "";
    }

    public Info(Exception ex)
    {
      this.Message = ex.Message;
      this.InfoLevel = InfoLevel.Error;
      this.Description = ex.ToString();
    }

    public string Message { get; private set; }

    public string Description { get; private set; }

    public InfoLevel InfoLevel { get; private set; }
  }

  /// <summary>
  /// Тип сообщения
  /// </summary>
  public enum InfoLevel
  {
    Info,
    Warning,
    Error
  }

  [Serializable]
  public class InfoCollection : IEnumerable<Info>
  {
    private List<Info> innerList = new List<Info>();

    public void Add(Info info)
    {
      innerList.Add(info);
    }

    public List<TOutput> ConvertAll<TOutput>(Converter<Info, TOutput> converter)
    {
      return innerList.ConvertAll<TOutput>(converter);
    }

    public Info this[int index]
    {
      get { return innerList[index]; }
    }

    public int Count
    {
      get { return innerList.Count; }
    }

    #region IEnumerable<Info> Members

    public IEnumerator<Info> GetEnumerator()
    {
      return innerList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return innerList.GetEnumerator();
    }

    #endregion
  }

  public delegate void InfoEventHandler(object sender, InfoEventArgs e);

  /// <summary>
  /// Коллекция сообщений, которую можно использовать для оповещения
  /// </summary>
  [Serializable]
  public class InfoEventArgs : EventArgs
  {
    private InfoCollection m_messages = new InfoCollection();

    public InfoEventArgs() { }

    internal InfoEventArgs(IEnumerable<Info> source)
    {
      foreach (Info info in source)
      {
        m_messages.Add(info);
      }
    }

    public InfoCollection Messages
    {
      get { return m_messages; }
    }
  }
}
