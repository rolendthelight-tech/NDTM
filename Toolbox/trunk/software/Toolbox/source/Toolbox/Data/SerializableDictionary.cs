using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Toolbox.Data
{
  [XmlRoot("dictionary")]
  [Serializable]
  public class SerializableDictionary<TKey, TValue>
      : Dictionary<TKey, TValue>, IXmlSerializable
  {
    private static readonly XmlSerializer _keySerializer = new XmlSerializer(typeof(TKey));
    private static readonly XmlSerializer _valueSerializer = new XmlSerializer(typeof(TValue));

    public SerializableDictionary() { }

    protected SerializableDictionary([NotNull] SerializationInfo info, StreamingContext context)
      : base(info, context) { }

    #region IXmlSerializable Members

    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml([NotNull] System.Xml.XmlReader reader)
    {
	    if (reader == null) throw new ArgumentNullException("reader");

	    bool was_empty = reader.IsEmptyElement;
      reader.Read();

      if (was_empty)
        return;

      while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");

        reader.ReadStartElement("key");
        TKey key = (TKey)_keySerializer.Deserialize(reader);
        reader.ReadEndElement();

        reader.ReadStartElement("value");
        TValue value = (TValue)_valueSerializer.Deserialize(reader);
        reader.ReadEndElement();

        this.Add(key, value);

        reader.ReadEndElement();
        reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    void IXmlSerializable.WriteXml([NotNull] System.Xml.XmlWriter writer)
    {
	    if (writer == null) throw new ArgumentNullException("writer");

	    foreach (TKey key in this.Keys)
      {
        writer.WriteStartElement("item");

        writer.WriteStartElement("key");
        _keySerializer.Serialize(writer, key);
        writer.WriteEndElement();

        writer.WriteStartElement("value");
        TValue value = this[key];
        _valueSerializer.Serialize(writer, value);
        writer.WriteEndElement();

        writer.WriteEndElement();
      }
    }

	  #endregion
  }
}
