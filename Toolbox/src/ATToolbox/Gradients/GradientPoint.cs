using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;


namespace AT.Toolbox.Gradients
{
  public struct HLS
  {
    public int H;
    public int L;
    public int S;
  }

  //TODO: перевести на DataContract
  public class GradientPoint
  {
    public byte B;
    public byte dB;
    public byte G;
    public byte R;

    public GradientPoint(byte db, byte r, byte g, byte b)
    {
      dB = db;
      R = r;
      G = g;
      B = b;
    }

    public GradientPoint(byte db, System.Drawing.Color cl)
    {
      dB = db;
      R = cl.R;
      G = cl.G;
      B = cl.B;
    }

    public System.Drawing.Color Color
    {
      get { return Color.FromArgb(R, G, B); }
      set
      {
        R = value.R;
        G = value.G;
        B = value.B;
      }
    }

    public HLS HueLightSaturation
    {
      get { return HLSRoutines.GetHLS(Color); }
      set
      {
        Color c = HLSRoutines.GetRGB(value);

        if (c != Color.Transparent)
          Color = c;
      }
    }

    public void SetColor(byte r, byte g, byte b)
    {
      R = r;
      G = g;
      B = b;
    }

    public static GradientPoint FromXml(XmlNode nod)
    {
      if (null == nod.Attributes["db"])
        return null;
      if (null == nod.Attributes["r"])
        return null;
      if (null == nod.Attributes["g"])
        return null;
      if (null == nod.Attributes["b"])
        return null;

      byte dB = byte.Parse(nod.Attributes["db"].Value);
      byte R = byte.Parse(nod.Attributes["r"].Value);
      byte G = byte.Parse(nod.Attributes["g"].Value);
      byte B = byte.Parse(nod.Attributes["b"].Value);

      return new GradientPoint(dB, R, G, B);
    }

    public void SaveXML(XmlNode nod)
    {
      XmlNode nod2 = nod.OwnerDocument.CreateElement("PT");

      nod2.Attributes.Append(nod.OwnerDocument.CreateAttribute("db"));
      nod2.Attributes.Append(nod.OwnerDocument.CreateAttribute("r"));
      nod2.Attributes.Append(nod.OwnerDocument.CreateAttribute("g"));
      nod2.Attributes.Append(nod.OwnerDocument.CreateAttribute("b"));
      nod2.Attributes["db"].Value = dB.ToString();
      nod2.Attributes["r"].Value = R.ToString();
      nod2.Attributes["g"].Value = G.ToString();
      nod2.Attributes["b"].Value = B.ToString();

      nod.AppendChild(nod2);
    }

    public GradientPoint Clone()
    {
      return new GradientPoint(dB, R, G, B);
    }
  }
}
