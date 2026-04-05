using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System;

namespace AT.Toolbox.Misc
{
  //TODO: Локализация

  /// <summary>
  /// Служебная задача для ничегонеделания
  /// </summary>
  public class StubWork : IBackgroundWork
  {
    #region IBackgroundWork Members

    public bool CloseOnFinish
    {
      get { return true; }
    }

    public bool IsMarquee
    {
      get { return false; }
    }

    public bool CanCancel
    {
      get { return true; }
    }

    public Bitmap Icon
    {
      get { return null; }
    }

    public string Name
    {
      get { return "Doing Absoultely Nothing"; }
    }

    public float Weight
    {
      get { return 1; }
    }

    public object Result
    {
      get { throw new NotImplementedException( ); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Run(BackgroundWorker worker)
    {
      PropertyChanged += delegate { };
      AppSwitchablePool.RegisterThread(Thread.CurrentThread);

      if (null == worker)
        return;

      worker.ReportProgress(0, "Starting to do nothing ... ");
      Thread.Sleep(1000);
      worker.ReportProgress(25, "Doing nothing ... ");
      Thread.Sleep(1000);
      worker.ReportProgress(50, "Still doing nothing ... ");
      Thread.Sleep(1000);
      worker.ReportProgress(75, "Finishing doing nothing ... ");
      Thread.Sleep(1000);
      worker.ReportProgress(100, "Finished doing nothing ... ");
    }

    #endregion
  }
}