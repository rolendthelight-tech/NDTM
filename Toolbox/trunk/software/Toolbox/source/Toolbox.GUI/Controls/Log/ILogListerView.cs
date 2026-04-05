using System.ComponentModel;
using Toolbox.Application.Services;

namespace Toolbox.GUI.Controls.Log
{
  public interface ILogListerView : ISynchronizeInvoke, IComponent
  {
    BindingList<LogEntry> DataSource { get; set; }

    InfoLevel Treshold { get; }
  }
}