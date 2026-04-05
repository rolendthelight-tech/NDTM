using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.Constants;
using AT.Toolbox.MSSQL.Properties;

namespace AT.Toolbox.MSSQL
{
  /// <summary>
  /// Класс для хранения информации о пригодности с комментариями
  /// </summary>
  public class SupportInfo
  {
    private List<string> output = new List<string>();
    private Support supported = Support.Full;
    private bool outputIsClear = true;

    public Support Supported
    {
      get { return supported; }
    }

    /// <summary>
    /// Информация в виде массива строк
    /// </summary>
    public string[] TestResultsEx
    {
      get { return output.ToArray(); }
    }

    /// <summary>
    /// Информация в виде строки
    /// </summary>
    public string TestResults
    {
      get
      {
        string ret = string.Join(Environment.NewLine, this.TestResultsEx);

        switch (this.supported)
        {
          case Support.Full:
            ret = Resources.STATUS_DESCRIPTION_FULL + Environment.NewLine + ret;
            break;
          case Support.Partial:
            ret = Resources.STATUS_DESCRIPTION_PARTIAL + Environment.NewLine + ret;
            break;
          case Support.Unknown:
            ret = Resources.STATUS_DESCRIPTION_UNKNOWN + Environment.NewLine + ret;
            break;
          case Support.None:
            ret = Resources.STATUS_DESCRIPTION_NONE + Environment.NewLine + ret;
            break;
        }

        return ret;
      }
    }

    public string Version { get; set; }

    /// <summary>
    /// Добавить информацию
    /// </summary>
    /// <param name="support">Устанавливаемый уровень пригодности; может только уменьшаться</param>
    /// <param name="versionInfo">Сообщение</param>
    public void UpdateSupported(Support support, string versionInfo)
    {
      if (support < this.supported)
      {
        this.supported = support;
      }
      if (!string.IsNullOrEmpty(versionInfo))
      {
        if (this.outputIsClear && support != Support.Unknown)
        {
          this.output.Add(Resources.DB_BROWSER_SUPPORT_PROBLEMS);
        }
        this.output.Add(versionInfo);
        this.outputIsClear = false;
      }
    }

    public void UpdateDatabaseInfo(string name, string version, Guid objectGuid)
    {
      this.Version = version;
      this.output.Clear();
      this.output.Add(Resources.DB_INFO);
      this.output.Add(string.Format(Resources.DB_NAME, name));
      this.output.Add(string.Format(Resources.DB_VERSION, version));
      if (objectGuid != Guid.Empty)
      {
        this.output.Add(string.Format(Resources.DB_GUID, objectGuid));
      }
      this.outputIsClear = true;
    }
  }
}