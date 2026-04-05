using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace AT.Toolbox.Controls
{
  using Base;
  using Dialogs;
  using Log;
  using Misc;
  using Properties;
  using Settings;

  using System.Runtime.Serialization;

  //TODO: Локализация

    /// <summary>
    /// Настройки отображения протокола
    /// </summary>
  [DataContract]
  public class LogListerSettings : ConfigurationSection
  {
    bool m_show_debug;

    /// <summary>
    /// Показывать или нет кнопку отладочных сообщений и отладочные сообщения
    /// </summary>
    [DataMember]
    public bool ShowDebug
    {
      get { return m_show_debug; }
      set { m_show_debug = value; }
    }

    /// <summary>
    /// Цвет сообщений об ошибках
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Red")]
    public Color ErrorColor { get; set; }

    /// <summary>
    /// Цвет предупреждений
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Orange")]
    public Color WarningColor { get; set; }

    /// <summary>
    /// Цвет обычных сообщений
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Black")]
    public Color InfoColor { get; set; }

    /// <summary>
    /// Цвет отладочных сообщений
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Gray")]
    public Color DebugColor { get; set; }

    /// <summary>
    /// Прокрутка списка вниз
    /// </summary>
    [DataMember]
    [DefaultValue(true)]
    public bool ScrollDown { get; set; }

    /// <summary>
    /// Цвет отладочных сообщений
    /// </summary>
    [DataMember]
    [DefaultValue(1000)]
    public int RecentLogCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [DataMember]
    [DefaultValue(InfoLevel.Info)]
    public InfoLevel DefaultLevel { get; set; }

    public override bool Validate(InfoBuffer WarningsAndErrors)
    {
      if (0 == RecentLogCount)
        WarningsAndErrors.Add(this.ToString(),
          "Количество отображаемых сообщений не задано, последние сообщения будут отображаться некорректно", InfoLevel.Warning);

      return true;
    }

    /// <summary>
    /// Настройки 
    /// </summary>
    public static LogListerSettings Preferences
    {
      get
      {
        return AppManager.Configurator.GetSection<LogListerSettings>();
      }
      set { AppManager.Configurator.SaveSection(value); }
    }
  }
}