using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AT.Toolbox.Log;

namespace ATBaseProject
{
  public static class LayoutSave
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LayoutSave).Name);

    public static string GetPath(Type type)
    {
      string path = Path.Combine(Application.UserAppDataPath, "Layouts");
      path = Path.Combine(path, type.Name) + "_layout.xml";

      if (!Directory.Exists(Path.GetDirectoryName(path)))
        Directory.CreateDirectory(Path.GetDirectoryName(path));

      return path;
    }

    public static void Clear()
    {
      try
      {
        string path = Path.Combine(Application.UserAppDataPath, "Layouts");

        Directory.Delete(path, true);
      }
      catch (Exception ex)
      {
        Log.Error("Clear(): exception", ex);
      }
    }
  }
}