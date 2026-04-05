using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System;
using Toolbox.Application.Services;
using Toolbox.GUI.Base;
using Toolbox.GUI.Properties;

namespace Toolbox.GUI.Misc
{
  //TODO: Локализация

  /// <summary>
  /// Служебная задача для загрузки файла протокола
  /// </summary>
	[PercentNotification]
	internal class LoadLogFileWork : CancelableRunBase
  {
    /// <summary>
    /// Содерживое файла протокола
    /// </summary>
    public List<string> strings = new List<string>();

    override public void Run()
    {
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

			//  worker.ReportProgress(counter, "Loading…");

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
			//      worker.ReportProgress(counter, "Loading…");
      //    }
      //  }
      //}
    }
  }
}