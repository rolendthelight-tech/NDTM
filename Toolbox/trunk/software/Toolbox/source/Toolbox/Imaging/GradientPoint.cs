#region

using System.Drawing;
using System.Runtime.Serialization;

#endregion

namespace Toolbox.Imaging
{
  /// <summary>
  ///   Точка градиента
  /// </summary>
  [DataContract( Name = "GradientPoint" )]
  public class GradientPoint
  {
    /// <summary>
    ///   Конструктор
    /// </summary>
    /// <param name = "NewPosition">Положение на градиенте</param>
    /// <param name = "NewR">Красный компонент</param>
    /// <param name = "NewG"> Зелёный компонент</param>
    /// <param name = "NewB"> Синий компонент</param>
    public GradientPoint( int NewPosition,
                          byte NewR,
                          byte NewG,
                          byte NewB )
    {
      Position = NewPosition;
      R = NewR;
      G = NewG;
      B = NewB;
    }

    /// <summary>
    ///   Конструктор
    /// </summary>
    /// <param name = "NewPosition">Положение на градиенте</param>
    /// <param name = "NewColor">Цвет</param>
    public GradientPoint( int NewPosition,
                          Color NewColor )
    {
      Position = NewPosition;
      R = NewColor.R;
      G = NewColor.G;
      B = NewColor.B;
    }

    /// <summary>
    ///   Синий компонент
    /// </summary>
    [DataMember( IsRequired = false, Name = "B" )]
    public byte B { get; set; }

    /// <summary>
    ///   Зелёный компонент
    /// </summary>
    [DataMember( IsRequired = false, Name = "G" )]
    public byte G { get; set; }

    /// <summary>
    ///   Положение на градиенте
    /// </summary>
    [DataMember( IsRequired = false, Name = "Position" )]
    public int Position { get; set; }

    /// <summary>
    ///   Красный компонент
    /// </summary>
    [DataMember( IsRequired = false, Name = "R" )]
    public byte R { get; set; }

    ///<summary>
    ///  Цвет точки в системе координат RGB
    ///</summary>
    public Color RGBColor
    {
      get { return Color.FromArgb( R, G, B ); }
      set
      {
        R = value.R;
        G = value.G;
        B = value.B;
      }
    }

    ///<summary>
    ///  Цвет точки в системе координат HLS
    ///</summary>
    public HLS HLSColor
    {
      get { return HLSRoutines.GetHLS( RGBColor ); }
      set
      {
        Color c = HLSRoutines.GetRGB( value );

        if( c != Color.Transparent )
          RGBColor = c;
      }
    }

    /// <summary>
    ///   Создание копии точки
    /// </summary>
    /// <returns>Копия точки</returns>
    public GradientPoint Clone( )
    {
      return new GradientPoint( Position, R, G, B );
    }
  }
}