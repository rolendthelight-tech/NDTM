using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Xml;


namespace AT.Toolbox.Gradients
{
  public class Gradient
  {
    protected Color[] m_color_table;
    private double m_DynMax = 0;
    private double m_DynMin = -90.3;

    protected List<GradientPoint> m_points = new List<GradientPoint>();
    public string Name;
    private bool UnacceptedChanges;

    public Gradient()
    {
    }

    public Gradient(Gradient sc)
    {
      Name = sc.Name;

      foreach (GradientPoint point in sc.Points)
        m_points.Add(point.Clone());

      if (null != sc.m_color_table)
        m_color_table = (Color[])sc.m_color_table.Clone();
    }

    public double DynRange
    {
      get { return DynMax - DynMin; }
    }

    public ColorBlend Blend
    {
      get
      {
        ColorBlend ret_val = new ColorBlend(m_points.Count);
        Color[] colors = new Color[m_points.Count];
        float[] positions = new float[m_points.Count];

        int i = 0;

        m_points.Sort(ComparePoints);

        foreach (GradientPoint pt in m_points)
        {
          colors[i] = pt.Color;
          positions[i++] = (float)pt.dB / 255;
        }

        ret_val.Colors = colors;
        ret_val.Positions = positions;

        return ret_val;
      }
    }

    public List<GradientPoint> Points
    {
      get { return m_points; }
    }

    public double DynMax
    {
      get { return m_DynMax; }
      set
      {
        double old = m_DynMax;
        m_DynMax = value;
        UnacceptedChanges = true;
      }
    }

    public double DynMin
    {
      get { return m_DynMin; }
      set
      {
        double old = m_DynMin;
        m_DynMin = value;

        UnacceptedChanges = true;
      }
    }

    public bool Changed
    {
      get { return UnacceptedChanges; }
    }

    public static Gradient FromXml(XmlNode nod)
    {
      if (null == nod.Attributes["name"] || 0 == nod.ChildNodes.Count)
        return null;

      Gradient sc = new Gradient();
      sc.Name = nod.Attributes["name"].Value;

      foreach (XmlNode nod2 in nod.ChildNodes)
      {
        GradientPoint cp = GradientPoint.FromXml(nod2);
        if (null != cp)
          sc.Points.Add(cp);
      }

      if (0 == sc.Points.Count)
        return null;

      return sc;
    }

    public void Clear()
    {
      Clear(true);
    }

    public void Clear(bool add_points)
    {
      m_points.Clear();

      if (add_points)
      {
        m_points.Add(new GradientPoint(0, 0, 0, 0));
        m_points.Add(new GradientPoint(255, 255, 255, 255));
      }
    }

    public void RecalcTable()
    {
      if (m_points.Count == 0)
        return;

      m_color_table = new Color[256];

      m_points.Sort(ComparePoints);

      int left_index = 0;

      float db_distance = 0;
      float R_distance = 0;
      float G_distance = 0;
      float B_distance = 0;

      for (int i = 0; i < 256; i++)
      {
        if (m_points[left_index + 1].dB < i || db_distance == 0)
        {
          if (db_distance != 0)
            left_index++;

          db_distance = m_points[left_index + 1].dB - m_points[left_index].dB;
          R_distance = (m_points[left_index + 1].R - m_points[left_index].R) / db_distance;
          G_distance = (m_points[left_index + 1].G - m_points[left_index].G) / db_distance;
          B_distance = (m_points[left_index + 1].B - m_points[left_index].B) / db_distance;
        }

        byte spectra_R = (byte)(R_distance * (i - m_points[left_index].dB) + m_points[left_index].R);
        byte spectra_G = (byte)(G_distance * (i - m_points[left_index].dB) + m_points[left_index].G);
        byte spectra_B = (byte)(B_distance * (i - m_points[left_index].dB) + m_points[left_index].B);

        m_color_table[i] = Color.FromArgb(spectra_R, spectra_G, spectra_B);
      }

      UnacceptedChanges = true;
    }

    public Color SpectraColor(double spectra_val)
    {
      if (m_points.Count == 0)
        return Color.Transparent;

      if (null == m_color_table)
        RecalcTable();

      double new_val = spectra_val;

      if (new_val > 1)
        new_val = Math.Log(new_val, 10) * 20; // Math.Log(x) * 25
      else
        new_val = 0;

      double x_db = new_val - 90.3;

      new_val = 255.0 * (x_db - DynMin) / DynRange;

      if (new_val > 255)
        new_val = 255;
      else if (new_val < 0)
        new_val = 0;

      return m_color_table[(int)new_val];
    }

    protected static int ComparePoints(GradientPoint pt1, GradientPoint pt2)
    {
      return pt1.dB.CompareTo(pt2.dB);
    }

    public void SaveXML(XmlNode nod)
    {
      XmlNode nod2 = nod.OwnerDocument.CreateElement("SCHEME");
      nod2.Attributes.Append(nod.OwnerDocument.CreateAttribute("name"));
      nod2.Attributes["name"].Value = Name;

      foreach (GradientPoint point in m_points)
        point.SaveXML(nod2);

      nod.AppendChild(nod2);
    }

    public override string ToString()
    {
      return Name;
    }

    public void AcceptChanges()
    {
      UnacceptedChanges = false;
    }
  }
}
