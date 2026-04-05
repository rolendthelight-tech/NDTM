using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace AT.Toolbox.Serialization
{
  public abstract class SerializationWrapper : IFormatter
  {
    public SerializationWrapper()
    {

    }

    public SerializationWrapper(IEnumerable<Type> KnownTypes)
    {

    }

    protected abstract void InnerSerialize(object t, Stream s);

    protected abstract object InnerDeSerialize(Stream s);

    public abstract object Deserialize(Stream s, Type DesiredType);

    public virtual void Close()
    {

    }

    public object Deserialize( Stream serializationStream )
    {
      return InnerDeSerialize(serializationStream);
    }

    public void Serialize( Stream serializationStream, object graph )
    {
      InnerSerialize(graph, serializationStream);
    }

    public abstract ISurrogateSelector SurrogateSelector { get; set; }

    public abstract SerializationBinder Binder { get; set; }

    public abstract StreamingContext Context { get; set; }
  }

  public class BinaryFormatterWrapper : SerializationWrapper
  {
    protected BinaryFormatter m_writer;

    public BinaryFormatterWrapper()
    {
      m_writer = new BinaryFormatter();
    }

    public BinaryFormatterWrapper(IEnumerable<Type> KnownTypes)
    {
      m_writer = new BinaryFormatter();
    }

    protected override void InnerSerialize(object t, Stream s)
    {
      m_writer.Serialize(s, t);
      s.Flush();
    }

    protected override object InnerDeSerialize(Stream s)
    {
      return m_writer.Deserialize(s);
    }

    public override object Deserialize(Stream s, Type DesiredType)
    {
      object o = InnerDeSerialize(s);
      return o.GetType() == DesiredType ? o : null;
    }

    public override ISurrogateSelector SurrogateSelector
    {
      get { return m_writer.SurrogateSelector;  }
      set { m_writer.SurrogateSelector = value; }
    }

    public override SerializationBinder Binder
    {
      get { return m_writer.Binder; }
      set { m_writer.Binder = value; }
    }

    public override StreamingContext Context
    {
      get { return m_writer.Context; }
      set { m_writer.Context = value; }
    }
  }

  public class DataContractSerializerWrapper : SerializationWrapper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DataContractSerializerWrapper).Name);

    protected Dictionary<Type, DataContractSerializer> m_serializers = new Dictionary<Type, DataContractSerializer>();

    IEnumerable<Type> m_known_types;

    public DataContractSerializerWrapper()
    {
      m_known_types = new List<Type>();
    }

    public DataContractSerializerWrapper(IEnumerable<Type> KnownTypes)
    {
      m_known_types = KnownTypes;

      foreach (Type type in KnownTypes)
        m_serializers.Add(type, new DataContractSerializer(type, m_known_types));
    }

    protected override void InnerSerialize(object t, Stream s)
    {
      if (!m_serializers.ContainsKey(t.GetType()))
        m_serializers.Add(t.GetType(), new DataContractSerializer(t.GetType(), m_known_types));

      m_serializers[t.GetType()].WriteObject(s, t);
      s.Flush();
    }

    protected override object InnerDeSerialize(Stream s)
    {
      foreach (KeyValuePair<Type, DataContractSerializer> pair in m_serializers)
      {
        try
        {
          return pair.Value.ReadObject(s);
        }
        catch (Exception ex)
        {
          Log.Error("InnerDeSerialize(): exception", ex);
        }
      }

      return null;
    }

    public override object Deserialize(Stream s, Type DesiredType)
    {
      try
      {
        if (!m_serializers.ContainsKey(DesiredType))
          m_serializers.Add(DesiredType, new DataContractSerializer(DesiredType, m_known_types));

        return m_serializers[DesiredType].ReadObject(s);
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("Deserialize({0}): exception", DesiredType), ex);
      }

      return null;
    }

    public override ISurrogateSelector SurrogateSelector
    {
      get { throw new NotImplementedException();  }
      set { throw new NotImplementedException(); }
    }

    public override SerializationBinder Binder
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public override StreamingContext Context
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
  }
}
