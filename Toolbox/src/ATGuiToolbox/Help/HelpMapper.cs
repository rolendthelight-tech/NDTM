using System;
using System.ComponentModel;
using System.Drawing;

namespace AT.Toolbox.Help
{
  [ToolboxBitmap("_16_help"),
   Description("Maps The Control To The Help Topic")]
  public partial class HelpMapper : Component, ISupportInitialize
  {
    private HelpEntry[] m_help_map;
    public string MainTopic { get; set; }

    public HelpMapper()
    {
      InitializeComponent();
    }

    public HelpMapper(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public HelpEntry[] HelpMap
    {
      get
      {
        if (null == m_help_map)
          m_help_map = new HelpEntry[] {};

        return m_help_map;
      }
      set { m_help_map = value; }
    }

    public void BeginInit()
    {
    }

    public void EndInit()
    {
      ApplicationHelp.AppendMap(this);
    }
  }

  [TypeConverter(typeof (ExpandableObjectConverter))]
  public class HelpEntry
  {
    public Component Target { get; set; }
    public String HelpLink { get; set; }
  }
}