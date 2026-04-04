using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

using AT.Toolbox.Properties;


namespace AT.Toolbox.Gradients
{
  public class Gradients
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Gradients).Name);

    protected static List<Gradient> m_common_schemes;
    protected static List<Gradient> m_custom_schemes;

    public static Gradient BlackAndWhite
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BW;
        sc.Clear();
        return sc;
      }
    }

    public static Gradient BlackAndWhiteInv
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BW_INV;
        sc.Points.Add(new GradientPoint(0, 255, 255, 255));
        sc.Points.Add(new GradientPoint(255, 0, 0, 0));
        return sc;
      }
    }

    public static Gradient CoolEdit
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_COOL;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(128, 128, 0, 128));
        sc.Points.Add(new GradientPoint(192, 255, 0, 0));
        sc.Points.Add(new GradientPoint(240, 255, 255, 0));
        sc.Points.Add(new GradientPoint(255, 255, 255, 255));
        return sc;
      }
    }

    public static Gradient RedYellow
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_RED_YELLOW;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(192, 255, 0, 0));
        sc.Points.Add(new GradientPoint(240, 255, 255, 0));
        sc.Points.Add(new GradientPoint(255, 255, 255, 255));
        return sc;
      }
    }

    public static Gradient Search2
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_SEARCH_2;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(177, 6, 6, 6));
        sc.Points.Add(new GradientPoint(196, 242, 242, 242));
        sc.Points.Add(new GradientPoint(214, 17, 17, 17));
        sc.Points.Add(new GradientPoint(255, 0, 0, 0));
        return sc;
      }
    }

    public static Gradient Search
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_SEARCH;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(119, 6, 6, 6));
        sc.Points.Add(new GradientPoint(138, 242, 242, 242));
        sc.Points.Add(new GradientPoint(156, 17, 17, 17));
        sc.Points.Add(new GradientPoint(255, 0, 0, 0));
        return sc;
      }
    }

    public static Gradient BlueSelect
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BLUE_SELECTION;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(213, 4, 0, 79));
        sc.Points.Add(new GradientPoint(255, 255, 255, 255));
        return sc;
      }
    }

    public static Gradient BlueInv
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BLUE_INVERSE;
        sc.Points.Add(new GradientPoint(0, 255, 255, 255));
        sc.Points.Add(new GradientPoint(133, 32, 0, 255));
        sc.Points.Add(new GradientPoint(255, 0, 0, 0));
        return sc;
      }
    }

    public static Gradient Blue2
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BLUE_2;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(159, 61, 0, 128));
        sc.Points.Add(new GradientPoint(255, 255, 255, 255));
        return sc;
      }
    }

    public static Gradient Blue
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_BLUE;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(143, 41, 0, 128));
        sc.Points.Add(new GradientPoint(255, 255, 255, 255));
        return sc;
      }
    }

    public static Gradient Topographic
    {
      get
      {
        Gradient sc = new Gradient();
        sc.Name = Resources.STR_COLOR_SCHEME_TOPO;
        sc.Points.Add(new GradientPoint(0, 0, 0, 0));
        sc.Points.Add(new GradientPoint(21, 255, 0, 0));
        sc.Points.Add(new GradientPoint(38, 0, 0, 0));
        sc.Points.Add(new GradientPoint(57, 255, 128, 0));
        sc.Points.Add(new GradientPoint(76, 0, 0, 0));
        sc.Points.Add(new GradientPoint(93, 255, 255, 0));
        sc.Points.Add(new GradientPoint(109, 0, 0, 0));
        sc.Points.Add(new GradientPoint(130, 0, 255, 0));
        sc.Points.Add(new GradientPoint(147, 1, 1, 1));
        sc.Points.Add(new GradientPoint(165, 0, 255, 255));
        sc.Points.Add(new GradientPoint(183, 2, 2, 2));
        sc.Points.Add(new GradientPoint(201, 0, 0, 255));
        sc.Points.Add(new GradientPoint(219, 1, 1, 1));
        sc.Points.Add(new GradientPoint(239, 255, 0, 255));
        sc.Points.Add(new GradientPoint(255, 0, 0, 0));

        return sc;
      }
    }

    public static List<Gradient> Predefined
    {
      get
      {
        if (null == m_common_schemes)
        {
          m_common_schemes = new List<Gradient>();
          m_common_schemes.Add(BlackAndWhite);
          m_common_schemes.Add(BlackAndWhiteInv);
          m_common_schemes.Add(CoolEdit);
          m_common_schemes.Add(RedYellow);
          m_common_schemes.Add(Search);
          m_common_schemes.Add(Search2);
          m_common_schemes.Add(Blue);
          m_common_schemes.Add(Blue2);
          m_common_schemes.Add(BlueSelect);
          m_common_schemes.Add(BlueInv);
          m_common_schemes.Add(Topographic);
        }

        return m_common_schemes;
      }
    }

    public static List<Gradient> Custom
    {
      get
      {
        if (null == m_custom_schemes)
          m_custom_schemes = new List<Gradient>();

        return m_custom_schemes;
      }
    }

    public static void Load(string file)
    {
      List<Gradient> init = Predefined;

      Custom.Clear();

      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(file);

        foreach (XmlNode nod in doc.FirstChild.ChildNodes)
        {
          Gradient sc = Gradient.FromXml(nod);
          m_custom_schemes.Add(sc);
        }
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("Load({0}): exception", file), ex);
        Trace.TraceError(ex.Message);
      }
    }

    public static void Save(string file)
    {
      if (null == m_custom_schemes || 0 == m_custom_schemes.Count)
        return;

      XmlDocument doc = new XmlDocument();
      XmlNode nod = doc.CreateElement("SCHEMES");

      doc.AppendChild(nod);

      foreach (Gradient scheme in m_custom_schemes)
        scheme.SaveXML(nod);

      doc.Save(file);
    }
  }
}
