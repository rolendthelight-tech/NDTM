using System;
using System.Linq;

namespace AT.Toolbox.MSSQL.Browsing
{
  public static class VersionHelper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(VersionHelper).Name);

    const int VersionPartCount = 4;

    public static string GetNextVersion(string version, int position)
    {
      if (position >= VersionPartCount)
      {
        throw new ArgumentException();
      }
      
      int[] points = new int[VersionPartCount];

      int i = 0;
      foreach (string currentPoint in version.Split('.'))
      {
        points[i] = int.Parse(currentPoint);
        i++;
      }

      points[position]++;
      return string.Join(".", (from p in points select p.ToString()).ToArray());
    }

    /// <summary>
    /// Сравнение номеров версий формате Х.Х.Х.Х
    /// </summary>
    /// <returns>1: первая версия старше; -1: вторая версия старше; 0: версии одинаковы</returns>
    /// <exception cref="ArgumentException">Неправильный формат версии</exception>
    public static int CompareVersions(string firstVersion, string secondVersion)
    {
      firstVersion = string.IsNullOrEmpty(firstVersion) ? "0.0.0.0" : firstVersion;
      secondVersion = string.IsNullOrEmpty(secondVersion) ? "0.0.0.0" : secondVersion;

      if (firstVersion == secondVersion)
      {
        return 0;
      }

      int[] firstPoints = new int[VersionPartCount];
      int[] secondPoints = new int[VersionPartCount];

      try
      {
        string [] firstVersionParts = firstVersion.Split('.');

        if (firstVersionParts.Length != VersionPartCount)
        {
          throw new Exception();
        }
        for (int i = 0; i < VersionPartCount; i++)
        {
          firstPoints[i] = int.Parse(firstVersionParts[i]);
        }

        string[] secondVersionParts = secondVersion.Split('.');

        if (secondVersionParts.Length != VersionPartCount)
        {
          throw new Exception();
        }
        for (int i = 0; i < VersionPartCount; i++)
        {
          secondPoints[i] = int.Parse(secondVersionParts[i]);
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("CompareVersions({0}, {1}): exception", firstVersion, secondVersion), ex);
        throw new ArgumentException(Properties.Resources.SQL_INCORRECT_VERSION);
      }

      for (int i = 0; i < VersionPartCount - 1; i++)
      {
        int ret = firstPoints[i].CompareTo(secondPoints[i]);
        if (ret != 0)
        {
          return ret;
        }
      }
      return 0;
    }
  }
}
