#region

using System;
using System.Drawing;

#endregion

namespace Toolbox.Imaging
{
  /// <summary>
  ///   Работа с цветовой системой координат HLS
  /// </summary>
  public static class HLSRoutines
  {
    #region Constants -----------------------------------------------------------------------------------------------------------

    ///<summary>
    ///  Максимальное значение в системе координат HLS
    ///</summary>
    public const int MaxHLS = 240;

    ///<summary>
    ///  Максимальное значение в системе координат RGB
    ///</summary>
    public const int MaxRGB = 255;

    ///<summary>
    ///  Неопределённое значение в системе координат HLS
    ///</summary>
    public const int UndefinedHLS = ( MaxHLS * 2 / 3 );

    #endregion

    #region Public Methods ------------------------------------------------------------------------------------------------------

    /// <summary>
    ///   Получение цвета в HLS из цвета в RGB
    /// </summary>
    /// <param name = "Cl">Цвет в RGB</param>
    /// <returns>Цвет в HLS</returns>
    public static HLS GetHLS( Color Cl )
    {
      HLS ret_val = new HLS( );

      // calculate lightness
      byte c_max = Math.Max( Math.Max( Cl.R, Cl.G ), Cl.B );
      byte c_min = Math.Min( Math.Min( Cl.R, Cl.G ), Cl.B );

      ret_val.L = ( ( ( c_max + c_min ) * MaxHLS ) + MaxRGB ) / ( 2 * MaxRGB );

      if( c_max == c_min )
      {
        /* r=g=b --> achromatic case */
        ret_val.S = 0; /* saturation */
        ret_val.H = UndefinedHLS; /* hue */
      }
      else
      {
        /* chromatic case */
        /* saturation */
        if( ret_val.L <= ( MaxHLS / 2 ) )
        {
          ret_val.S = ( ( ( c_max - c_min ) * MaxHLS ) + ( ( c_max + c_min ) / 2 ) )
                      / ( c_max + c_min );
        }
        else
        {
          ret_val.S = ( ( ( c_max - c_min ) * MaxHLS ) + ( ( 2 * MaxRGB - c_max - c_min ) / 2 ) ) /
                      ( 2 * MaxRGB - c_max - c_min );
        }

        /* hue */
        int r_delta = ( ( ( c_max - Cl.R ) * ( MaxHLS / 6 ) ) + ( ( c_max - c_min ) / 2 ) )
                      / ( c_max - c_min );
        /* intermediate value: % of spread from max */
        int g_delta = ( ( ( c_max - Cl.G ) * ( MaxHLS / 6 ) ) + ( ( c_max - c_min ) / 2 ) )
                      / ( c_max - c_min );
        /* intermediate value: % of spread from max */
        int b_delta = ( ( ( c_max - Cl.B ) * ( MaxHLS / 6 ) ) + ( ( c_max - c_min ) / 2 ) )
                      / ( c_max - c_min );
        /* intermediate value: % of spread from max */

        if( Cl.R == c_max )
          ret_val.H = b_delta - g_delta;
        else if( Cl.G == c_max )
          ret_val.H = ( MaxHLS / 3 ) + r_delta - b_delta;
        else /* B == cMax */
          ret_val.H = ( ( 2 * MaxHLS ) / 3 ) + g_delta - r_delta;

        if( ret_val.H < 0 )
          ret_val.H += MaxHLS;

        if( ret_val.H > MaxHLS )
          ret_val.H -= MaxHLS;
      }

      return ret_val;
    }

    /// <summary>
    ///   Получение цвета в RGB из цвета в HLS
    /// </summary>
    /// <param name = "Hls">Цвет в HLS</param>
    /// <returns>Цвет в RGB</returns>
    public static Color GetRGB( HLS Hls )
    {
      int r;
      int g;
      int b;

      if( Hls.S == 0 )
      {
        r = g = b = ( Hls.L * MaxRGB ) / MaxHLS;

        if( Hls.H != UndefinedHLS )
          return Color.Transparent;
      }
      else
      {
        /* chromatic case */
        /* set up magic numbers */

        int magic2;

        if( Hls.L <= ( MaxHLS / 2 ) )
          magic2 = ( Hls.L * ( MaxHLS + Hls.S ) + ( MaxHLS / 2 ) ) / MaxHLS;
        else
          magic2 = Hls.L + Hls.S - ( ( Hls.L * Hls.S ) + ( MaxHLS / 2 ) ) / MaxHLS;

        int magic1 = 2 * Hls.L - magic2;

        /* get RGB, change units from HLSMAX to RGBMAX */
        r = ( HueToRGB( magic1, magic2, Hls.H + ( MaxHLS / 3 ) ) * MaxRGB + ( MaxHLS / 2 ) )
            / MaxHLS;
        g = ( HueToRGB( magic1, magic2, Hls.H ) * MaxRGB + ( MaxHLS / 2 ) ) / MaxHLS;
        b = ( HueToRGB( magic1, magic2, Hls.H - ( MaxHLS / 3 ) ) * MaxRGB + ( MaxHLS / 2 ) )
            / MaxHLS;
      }

      if( r > 255 )
        r = 255;

      if( g > 255 )
        g = 255;

      if( b > 255 )
        b = 255;

      if( r < 0 )
        r = 0;

      if( g < 0 )
        g = 0;

      if( b < 0 )
        b = 0;

      return Color.FromArgb( r, g, b );
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    /// <summary>
    ///   Перевод из оттенка (Hue) в компонену цвета
    /// </summary>
    /// <param name = "N1">число 1</param>
    /// <param name = "N2">число 2</param>
    /// <param name = "Hue">Hue</param>
    /// <returns></returns>
    private static int HueToRGB( int N1,
                                 int N2,
                                 int Hue )
    {
      /* range check: values passed add/subtract thirds of range */
      if( Hue < 0 )
        Hue += MaxHLS;

      if( Hue > MaxHLS )
        Hue -= MaxHLS;

      /* return r,g, or b value from this tridrant */
      if( Hue < ( MaxHLS / 6 ) )
        return ( N1 + ( ( ( N2 - N1 ) * Hue + ( MaxHLS / 12 ) ) / ( MaxHLS / 6 ) ) );

      if( Hue < ( MaxHLS / 2 ) )
        return ( N2 );

      if( Hue < ( ( MaxHLS * 2 ) / 3 ) )
      {
        return ( N1
                 +
                 ( ( ( N2 - N1 ) * ( ( ( MaxHLS * 2 ) / 3 ) - Hue ) + ( MaxHLS / 12 ) )
                   / ( MaxHLS / 6 ) ) );
      }

      return ( N1 );
    }

    #endregion
  }
}