#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Toolbox.Properties;

#endregion

namespace Toolbox.Imaging
{
  ///<summary>
  ///  Линейный цветовой градиент
  ///</summary>
  [DataContract( Name = "Gradient" )]
  public class Gradient
  {
    /// <summary>
    ///   Таблица цветов
    /// </summary>
    protected Color[] m_color_table;

    /// <summary>
    ///   Размер таблицы цветов
    /// </summary>
    private int m_max_position;

    /// <summary>
    ///   Список точек
    /// </summary>
    [DataMember( Name = "Points", IsRequired = false )] private List<GradientPoint> m_points = new List<GradientPoint>( );

    /// <summary>
    ///   Конструктор по умолчанию
    /// </summary>
    public Gradient( ) {}

    /// <summary>
    ///   Конструктор-копирование
    /// </summary>
    public Gradient( Gradient Src )
    {
      if( null == Src )
        throw new ArgumentNullException( @"Src" );

      if( null == Src.Points || 0 == Src.Points.Count )
        throw new ArgumentException( Resources.MSG_CANNOT_CLONE_EMPTY_GRADIENT, @"Src" );

      foreach( GradientPoint point in Src.Points )
        Points.Add( point.Clone( ) );

      if( null != Src.m_color_table )
        m_color_table = (Color[])Src.m_color_table.Clone( );
    }

    #region Public Methods ------------------------------------------------------------------------------------------------------

    /// <summary>
    ///   Очистка с добавлением точек (0, 0, 0, 0) и (255, 255, 255, 255)
    /// </summary>
    public void Clear( )
    {
      Clear( true );
    }

    ///<summary>
    ///  Очистка
    ///</summary>
    ///<param name = "AddPoints"><code>true</code>, если нужно добавлять (0, 0, 0, 0) и (255, 255, 255, 255)</param>
    public void Clear( bool AddPoints )
    {
      m_points.Clear( );

      if( !AddPoints )
        return;

      m_points.Add( new GradientPoint( 0, 0, 0, 0 ) );
      m_points.Add( new GradientPoint( 255, 255, 255, 255 ) );
    }

    /// <summary>
    ///   Пересчёт таблицв цветов
    /// </summary>
    public void RecalcTable( )
    {
      if( m_points.Count == 0 )
        return;

      m_points.Sort( ComparePoints );
      m_max_position = m_points[m_points.Count - 1].Position;

      m_color_table = new Color[m_max_position];

      int left_index = 0;

      float db_distance = 0;
      float r_distance = 0;
      float g_distance = 0;
      float b_distance = 0;

      for( int i = 0; i < m_max_position; i++ )
      {
        if( m_points[left_index + 1].Position < i || db_distance == 0 )
        {
          if( db_distance != 0 )
            left_index++;

          db_distance = m_points[left_index + 1].Position - m_points[left_index].Position;
          r_distance = ( m_points[left_index + 1].R - m_points[left_index].R ) / db_distance;
          g_distance = ( m_points[left_index + 1].G - m_points[left_index].G ) / db_distance;
          b_distance = ( m_points[left_index + 1].B - m_points[left_index].B ) / db_distance;
        }

        byte spectra_r =
                (byte)
                ( r_distance * ( i - m_points[left_index].Position ) + m_points[left_index].R );
        byte spectra_g =
                (byte)
                ( g_distance * ( i - m_points[left_index].Position ) + m_points[left_index].G );
        byte spectra_b =
                (byte)
                ( b_distance * ( i - m_points[left_index].Position ) + m_points[left_index].B );

        m_color_table[i] = Color.FromArgb( spectra_r, spectra_g, spectra_b );
      }
    }

    #endregion

    /// <summary>
    ///   Список точек
    /// </summary>
    public List<GradientPoint> Points
    {
      get { return m_points; }
    }

    /// <summary>
    ///   Blend для отрисовки
    /// </summary>
    public ColorBlend Blend
    {
      get
      {
        ColorBlend ret_val = new ColorBlend( m_points.Count );
        Color[] colors = new Color[m_points.Count];

        float[] positions = new float[m_points.Count];

        if( null == m_color_table )
          RecalcTable( );

        int i = 0;

        m_points.Sort( ComparePoints );

        foreach( GradientPoint pt in m_points )
        {
          colors[i] = pt.RGBColor;
          positions[i++] = (float)pt.Position / m_max_position;
        }

        ret_val.Colors = colors;
        ret_val.Positions = positions;

        return ret_val;
      }
    }

    /// <summary>
    ///   Размер таблицы цветов
    /// </summary>
    public int MaxPosition
    {
      get { return m_max_position; }
    }

    /// <summary>
    ///   Таблица цветов
    /// </summary>
    public Color[] GetColorTable( )
    {
      return m_color_table;
    }

    /// <summary>
    ///   Сравнение позиций точек для сортировки
    /// </summary>
    /// <param name = "Pt1">Точка 1</param>
    /// <param name = "Pt2">Точка 2</param>
    /// <returns><see>Int32.CompareTo</see>></returns>
    private static int ComparePoints( GradientPoint Pt1,
                                      GradientPoint Pt2 )
    {
      return Pt1.Position.CompareTo( Pt2.Position );
    }
  }
}