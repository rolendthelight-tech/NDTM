using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System;
using Toolbox.Application;
using Toolbox.Application.Services;
using Toolbox.GUI.Base;

namespace Toolbox.GUI.Misc
{
  //TODO: Локализация

  /// <summary>
  /// Служебная задача для ничегонеделания
  /// </summary>
	[PercentNotification]
	public class StubWork : CancelableRunBase
  {
    #region IBackgroundWork Members

    override public void Run()
    {
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      this.ReportProgress(0, "Starting to do nothing…");
      Thread.Sleep(1000);
			this.ReportProgress(25, "Doing nothing…");
      Thread.Sleep(1000);
			this.ReportProgress(50, "Still doing nothing…");
      Thread.Sleep(1000);
			this.ReportProgress(75, "Finishing doing nothing…");
      Thread.Sleep(1000);
			this.ReportProgress(100, "Finished doing nothing…");
    }

    #endregion
  }
}