using AT.Toolbox.Properties;
using DevExpress.LookAndFeel;

namespace AT.Toolbox.Base
{
  public static class DrawingModeConverter
  {
    public static void Parse(DefaultLookAndFeel feel, string val)
    {
      if (val == Resources.DRAWING_OFFICE_2003)
      {
        feel.LookAndFeel.Style = LookAndFeelStyle.Office2003;
        feel.LookAndFeel.UseWindowsXPTheme = true;
        return;
      }

      if (val == Resources.DRAWING_SKIN_CARAMEL)
      {
        feel.LookAndFeel.Style = LookAndFeelStyle.Skin;
        feel.LookAndFeel.SetSkinStyle("Caramel");
        feel.LookAndFeel.UseWindowsXPTheme = true;
        return;
      }

      if (val == Resources.DRAWING_SKIN_BLUE)
      {
        feel.LookAndFeel.Style = LookAndFeelStyle.Skin;
        feel.LookAndFeel.SetSkinStyle("Blue");
        feel.LookAndFeel.UseWindowsXPTheme = false;
        return;
      }

      if (val == Resources.DRAWING_SKIN_ASPHALT)
      {
        feel.LookAndFeel.Style = LookAndFeelStyle.Skin;
        feel.LookAndFeel.SkinName = "The Asphalt World";
        feel.LookAndFeel.UseWindowsXPTheme = false;
        return;
      }

      feel.LookAndFeel.Style = LookAndFeelStyle.Style3D;
      feel.LookAndFeel.UseWindowsXPTheme = true;
      return;
    }

    public static string ToString(DefaultLookAndFeel feel)
    {
      return "";
    }
  }
}