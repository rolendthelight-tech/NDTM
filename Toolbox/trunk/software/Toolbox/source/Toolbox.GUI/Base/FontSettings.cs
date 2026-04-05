using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Toolbox.Configuration;
using System.Collections.ObjectModel;
using System.Drawing;
using Toolbox.Application.Services;
using Toolbox.GUI.Properties;

namespace Toolbox.GUI.Base
{
  [DataContract]
  public class FontSettings : ConfigurationSection
  {
	  [NotNull] private static readonly Dictionary<string, FontSetupExtensions.FontSetupInfo> _font_setup = new Dictionary<string, FontSetupExtensions.FontSetupInfo>();

    private const string DefaultFontID = "DefaultFont";
    private static readonly object _lock = new object();
    private FontList m_fonts;
    public static event EventHandler<EventArgs> FontSettingsChanged;

    /// <summary>
    /// Настройки шрифтов
    /// </summary>
    [DataMember]
    public Collection<FontInfo> Fonts
    {
      get
      {
        if (m_fonts != null)
          return m_fonts;

        lock (_lock)
        {
          return m_fonts ?? (m_fonts = new FontList());
        }
      }
    }

    /// <summary>
    /// Шрифты, которые можно настроить
    /// </summary>
    [NotNull]
    public static Dictionary<string, FontSetupExtensions.FontSetupInfo> FontSetup
    {
      get { return _font_setup; }
    }

	  [CanBeNull]
	  public Font DefaultFont
    {
      get
      {
        var f = Fonts.SingleOrDefault(a => a.ID == DefaultFontID);
        return f == null ? null : f.Font;
      }
    }

    public static FontSettings Instance
    {
      get
      {
        var s = AppManager.Configurator.GetSection<FontSettings>();
        if (s.DefaultFont == null)
          s.AddDefaultFont();
        return s;
      }
    }

    private void AddDefaultFont()
    {
      if (DefaultFont == null)
      {
        var f = new Font("Times New Roman", 10);
        FontSetup.Add(DefaultFontID, Resources.DEFAULT, f);
        m_fonts.Add(new FontInfo(DefaultFontID) {Font = f});
      }
    }

    public void InitFonts()
    {
      lock (_lock)
      {
        AddDefaultFont();
        if (!_font_setup.ContainsKey(DefaultFontID))
          _font_setup.Add(DefaultFontID, new FontSetupExtensions.FontSetupInfo {Caption = Resources.DEFAULT, DefaultFont = DefaultFont});

        var removee = new HashSet<string>(this.Fonts.Select(f => f.ID)
          .Where(id => !_font_setup.ContainsKey(id)));

        foreach (var rm in removee)
          m_fonts.Remove(rm);


        foreach (var kv in _font_setup)
        {
          if (!m_fonts.Contains(kv.Key))
            m_fonts.Add(new FontInfo(kv.Key)
            {
              Font = kv.Value.DefaultFont,
              Color = Color.Black
            });
        }
      }
    }

    private class FontList : KeyedCollection<string, FontInfo>
    {
      protected override string GetKeyForItem([NotNull] FontInfo item)
      {
	      if (item == null) throw new ArgumentNullException("item");

				return item.ID;
      }
    }

    public override void ApplySettings()
    {
      base.ApplySettings();

      if (FontSettingsChanged != null)
      {
        FontSettingsChanged(this, EventArgs.Empty);
      }
    }

	  [CanBeNull]
	  public Font this[string id]
    {
      get
      {
        var f = Fonts.SingleOrDefault(a => a.ID == id);

        return f != null ? f.Font : DefaultFont;
      }
    }
  }

  [DataContract]
  [KnownType(typeof(System.Drawing.FontStyle))]
  [KnownType(typeof(System.Drawing.GraphicsUnit))]
  public class FontInfo
  {
    public FontInfo([NotNull] string id)
    {
	    if (id == null) throw new ArgumentNullException("id");
			if (string.IsNullOrEmpty(id)) throw new ArgumentException("Empty", "id");

      this.ID = id;
    }

    [DataMember]
    public string ID { get; private set; }

    [DataMember]
    public Font Font { get; set; }

    [DataMember]
    public Color Color { get; set; }
  }

  public static class FontSetupExtensions
  {
    [Obsolete]
    public static void Add([NotNull] this IDictionary<string, FontSetupInfo> dic, string id, string caption)
    {
	    if (dic == null) throw new ArgumentNullException("dic");

			dic.Add(id, new FontSetupInfo { Caption = caption, DefaultFont = new Font("Times New Roman", 10) });
    }

	  public static void Add([NotNull] this IDictionary<string, FontSetupInfo> dic, string id, string caption, Font defaultFont)
	  {
		  if (dic == null) throw new ArgumentNullException("dic");

			dic.Add(id, new FontSetupInfo { Caption = caption, DefaultFont = defaultFont });
	  }

	  public class FontSetupInfo
    {
      public string Caption { get; set; }

      public Font DefaultFont { get; set; }

      public override string ToString()
      {
        return this.Caption ?? base.ToString();
      }
    }
  }
}
