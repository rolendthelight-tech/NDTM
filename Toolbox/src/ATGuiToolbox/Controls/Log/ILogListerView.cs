using System.ComponentModel;
using AT.Toolbox;

namespace AT.Toolbox.Controls
{
  public interface ILogListerView : ISynchronizeInvoke, IComponent
  {
    BindingList<LogEntry> DataSource { get; set; }

    InfoLevel Treshold { get; }
  }
}