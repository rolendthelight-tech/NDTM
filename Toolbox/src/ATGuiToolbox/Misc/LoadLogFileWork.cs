using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using AT.Toolbox.Properties;
using System;

namespace AT.Toolbox.Misc
{
  //TODO: Локализация

  /// <summary>
  /// Служебная задача для загрузки файла протокола
  /// </summary>
  internal class LoadLogFileWork : IBackgroundWork
  {
    /// <summary>
    /// Содерживое файла протокола
    /// </summary>
    public List<string> strings = new List<string>();

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
      get { return Resources.p_48_view_log; }
    }

    public string Name
    {
      get { return "Loading Log File"; }
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
      //AppInstance.RegisterThread(Thread.CurrentThread);

      //if (string.IsNullOrEmpty(Logger.Preferences.LogFilePath))
      //{
      //  worker.ReportProgress(100, "");
      //  return;
      //}


      //FileStream fs = new FileStream(Path.Combine(Application.StartupPath, 
      //                               Logger.Preferences.LogFilePath),
      //                               FileMode.Open,
      //                               FileAccess.Read,
      //                               FileShare.ReadWrite);

      //using (StreamReader reader = new StreamReader(fs))
      //{
      //  int counter = 0;

      //  worker.ReportProgress(counter, "Loading...");

      //  while (true)
      //  {
      //    string line = reader.ReadLine();

      //    if (line == null)
      //      break;

      //    if (line.Length == 0)
      //      continue;

      //    strings.Add(line);

      //    counter++;

      //    if (counter > 100)
      //    {
      //      counter = 0;
      //      worker.ReportProgress(counter, "Loading...");
      //    }
      //  }
      //}
    }

    #endregion
  }
}