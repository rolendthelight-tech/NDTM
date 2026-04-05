using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Gradients
{
  public static class HLSRoutines
  {
    public const int MaxHLS = 240;
    public const int MaxRGB = 255;
    public const int UndefinedHLS = (MaxHLS * 2 / 3);

    public static HLS GetHLS(Color color)
    {
      HLS ret_val = new HLS();

      byte cMax, cMin; /* max and min RGB values */
      int Rdelta, Gdelta, Bdelta; /* intermediate value: % of spread from max */

      // calculate lightness
      cMax = Math.Max(Math.Max(color.R, color.G), color.B);
      cMin = Math.Min(Math.Min(color.R, color.G), color.B);
      ret_val.L = (((cMax + cMin) * MaxHLS) + MaxRGB) / (2 * MaxRGB);

      if (cMax == cMin)
      {
        /* r=g=b --> achromatic case */
        ret_val.S = 0; /* saturation */
        ret_val.H = UndefinedHLS; /* hue */
      }
      else
      {
        /* chromatic case */
        /* saturation */
        if (ret_val.L <= (MaxHLS / 2))
          ret_val.S = (((cMax - cMin) * MaxHLS) + ((cMax + cMin) / 2)) / (cMax + cMin);
        else
        {
          ret_val.S = (((cMax - cMin) * MaxHLS) + ((2 * MaxRGB - cMax - cMin) / 2)) /
                      (2 * MaxRGB - cMax - cMin);
        }

        /* hue */
        Rdelta = (((cMax - color.R) * (MaxHLS / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
        Gdelta = (((cMax - color.G) * (MaxHLS / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
        Bdelta = (((cMax - color.B) * (MaxHLS / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);

        if (color.R == cMax)
          ret_val.H = Bdelta - Gdelta;
        else if (color.G == cMax)
          ret_val.H = (MaxHLS / 3) + Rdelta - Bdelta;
        else /* B == cMax */
          ret_val.H = ((2 * MaxHLS) / 3) + Gdelta - Rdelta;

        if (ret_val.H < 0)
          ret_val.H += MaxHLS;

        if (ret_val.H > MaxHLS)
          ret_val.H -= MaxHLS;
      }

      return ret_val;
    }

    public static Color GetRGB(HLS hls)
    {
      int R;
      int G;
      int B;

      if (hls.S == 0)
      {
        R = G = B = (hls.L * MaxRGB) / MaxHLS;

        if (hls.H != UndefinedHLS)
          return Color.Transparent;
      }
      else
      {
        /* chromatic case */
        /* set up magic numbers */

        int Magic1, Magic2;

        if (hls.L <= (MaxHLS / 2))
          Magic2 = (hls.L * (MaxHLS + hls.S) + (MaxHLS / 2)) / MaxHLS;
        else
          Magic2 = hls.L + hls.S - ((hls.L * hls.S) + (MaxHLS / 2)) / MaxHLS;

        Magic1 = 2 * hls.L - Magic2;

        /* get RGB, change units from HLSMAX to RGBMAX */
        R = (HueToRGB(Magic1, Magic2, hls.H + (MaxHLS / 3)) * MaxRGB + (MaxHLS / 2)) / MaxHLS;
        G = (HueToRGB(Magic1, Magic2, hls.H) * MaxRGB + (MaxHLS / 2)) / MaxHLS;
        B = (HueToRGB(Magic1, Magic2, hls.H - (MaxHLS / 3)) * MaxRGB + (MaxHLS / 2)) / MaxHLS;
      }

      if (R > 255)
        R = 255;
      if (G > 255)
        G = 255;
      if (B > 255)
        B = 255;

      if (R < 0)
        R = 0;
      if (G < 0)
        G = 0;
      if (B < 0)
        B = 0;

      return Color.FromArgb(R, G, B);
    }

    static int HueToRGB(int n1, int n2, int hue)
    {
      /* range check: note values passed add/subtract thirds of range */
      if (hue < 0)
        hue += MaxHLS;

      if (hue > MaxHLS)
        hue -= MaxHLS;

      /* return r,g, or b value from this tridrant */
      if (hue < (MaxHLS / 6))
        return (n1 + (((n2 - n1) * hue + (MaxHLS / 12)) / (MaxHLS / 6)));

      if (hue < (MaxHLS / 2))
        return (n2);

      if (hue < ((MaxHLS * 2) / 3))
        return (n1 + (((n2 - n1) * (((MaxHLS * 2) / 3) - hue) + (MaxHLS / 12)) / (MaxHLS / 6)));
      else
        return (n1);
    }
  }
}
