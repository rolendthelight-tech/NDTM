using System;
using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Toolbox.Application.Services;
using Toolbox.Configuration;
using System.Runtime.Serialization;
using Toolbox.Log;

namespace Toolbox.GUI.Controls.Log
{
	//TODO: Локализация

    /// <summary>
    /// Настройки отображения протокола
    /// </summary>
  [DataContract]
  public class LogListerSettings : ConfigurationSection
  {
    bool m_show_debug;

    /// <summary>
    /// Показывать ли кнопку отладочных сообщений и отладочные сообщения
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
    //[DefaultValue(typeof(Color), "Orange")]
    [DefaultValue(typeof(Color), "MediumBlue")]
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

    public override bool Validate([NotNull] InfoBuffer WarningsAndErrors)
    {
	    if (WarningsAndErrors == null) throw new ArgumentNullException("WarningsAndErrors");

			if (0 == RecentLogCount)
        WarningsAndErrors.Add(link:this.ToString(),
					message: "Количество отображаемых сообщений не задано, последние сообщения будут отображаться некорректно", state: InfoLevel.Warning, details: null);

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