#region

using System;

#endregion

namespace Toolbox.Imaging
{
  /// <summary>
  ///   Цвет в представлении Hue/Light/Saturation
  /// </summary>
  public struct HLS : IEquatable<HLS>
  {
    /// <summary>
    ///   Hue
    /// </summary>
    public int H { get; set; }

    /// <summary>
    ///   Light
    /// </summary>
    public int L { get; set; }

    /// <summary>
    ///   Saturation
    /// </summary>
    public int S { get; set; }

    #region IEquatable<HLS> Members

    /// <summary>
    ///   Сравнение по значениям компонент
    /// </summary>
    /// <param name = "Other">Цвет в пространстве HLS</param>
    /// <returns>
    ///   <code>true</code>, если все значения компонент равны
    /// </returns>
    public bool Equals( HLS Other )
    {
      return Other.H == H && Other.L == L && Other.S == S;
    }

    #endregion

    /// <summary>
    ///   Сравнение по типу, ссылке и значениям компонент
    /// </summary>
    /// <param name = "Obj">Объект для сравнения</param>
    /// <returns><code>true</code>, если все значения компонент равны</returns>
    public override bool Equals( object Obj )
    {
      if( ReferenceEquals( null, Obj ) )
        return false;
      return Obj.GetType( ) == typeof( HLS ) && Equals( (HLS)Obj );
    }

    /// <summary>
    ///   Стандартное получение хэша
    /// </summary>
    /// <returns>Хэш-код объекта</returns>
    public override int GetHashCode( )
    {
      unchecked
      {
        int result = H;
        result = ( result * 397 ) ^ L;
        result = ( result * 397 ) ^ S;
        return result;
      }
    }

    /// <summary>
    ///   Оператор равенства
    /// </summary>
    /// <param name = "Left">Цвет в пространстве HLS</param>
    /// <param name = "Right">Цвет в пространстве HLS</param>
    /// <returns><code>true</code>, если цвета равны</returns>
    public static bool operator ==( HLS Left,
                                    HLS Right )
    {
      return Left.Equals( Right );
    }

    /// <summary>
    ///   Оператор неравенства
    /// </summary>
    /// <param name = "Left">Цвет в пространстве HLS</param>
    /// <param name = "Right">Цвет в пространстве HLS</param>
    /// <returns><code>true</code>, если цвета не равны</returns>
    public static bool operator !=( HLS Left,
                                    HLS Right )
    {
      return !Left.Equals( Right );
    }
  }
}