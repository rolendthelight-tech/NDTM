using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Toolbox.Mono
{
  /// <summary>
  /// Класс для сериализации XElement в Mono
  /// </summary>
  public class XElementWrapper : IXmlSerializable
  {
    XElement _element;

    public XElementWrapper() { }

    public XElementWrapper(XElement element)
    {
      _element = element;
    }

    public static explicit operator XElement(XElementWrapper content)
    {
      return content._element;
    }

    public static implicit operator XElementWrapper(XElement element)
    {
      return new XElementWrapper { _element = element };
    }

    #region IXmlSerializable Members

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      var data = reader.ReadOuterXml();
      if(!string.IsNullOrWhiteSpace(data))
        _element = XElement.Parse(data);
    }

    public void WriteXml(XmlWriter writer)
    {
      if (_element == null)
        return;

      foreach (XElement el in _element.Elements())
        el.WriteTo(writer);
    }

    #endregion
  }
}
