using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AT.Toolbox;
using DevExpress.Utils;

namespace ATHelpEditor
{
  [ATConfigurationComposite(Name = "TextBlockStyle")]
  public class TextBlockStyle
  {
    [ATConfigurationAttribute(Name = "Name", Default = "")]
    public string Name { get; set; }

    [ATConfigurationAttribute(Name = "Color", Default = "")]
    public Color Color { get; set; }

    [ATConfigurationAttribute(Name = "Font", Default = "Tahoma")]
    public string FontName { get; set; }

    [ATConfigurationAttribute(Name = "Alignment", Default = "Default")]
    public HorzAlignment Alignment { get; set; }

    [ATConfigurationAttribute(Name = "Bold", Default = "false")]
    public bool Bold { get; set; }

    [ATConfigurationAttribute(Name = "Underline", Default = "false")]
    public bool Underline { get; set; }

    [ATConfigurationAttribute(Name = "Italic", Default = "false")]
    public bool Italic { get; set; }

    [ATConfigurationAttribute(Name = "FontSize", Default = "4")]
    public int FontSize { get; set; }

    public override string ToString()
    {
      return this.Name ?? base.ToString();
    }
  }
}
