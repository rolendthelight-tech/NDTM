using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Runtime.Serialization;
using AT.Toolbox.Extensions;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Обеспечивает упаковку и распаковку объекта для использования удаленным сервисом
  /// </summary>
  public interface IPackable
  {
    byte[] Pack();
    bool Unpack(byte[] data);
  }

  /// <summary>
  /// Помечает поле как пакуемое
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class PackableFieldAttribute : Attribute { }

  /// <summary>
  /// Объект для компактной передачи табличных данных
  /// </summary>
  [Serializable]
  public class PackedMatrix
  {
    /// <summary>
    /// Заголовки колонок
    /// </summary>
    public string[] Headers;

    /// <summary>
    /// Массив колонок, каждая колонка - массив скаляров одного типа
    /// </summary>
    public Array[] Data;

    /// <summary>
    /// Возвращает количество строк
    /// </summary>
    /// <returns></returns>
    public int GetLength()
    {
      if (this.Data != null && this.Data.Length > 0)
      {
        return this.Data[0].Length;
      }

      return 0;
    }
  }

  /// <summary>
  /// Пакет, хранящий упакованные данные и идентификатор типа
  /// </summary>
  [Serializable]
  public class Package
  {
    byte[] m_data = new byte[0];
    Guid m_type_guid;

    public Package() { }

    public Package(IPackable source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      m_data = source.Pack() ?? new byte[0];
      m_type_guid = source.GetType().GUID;
      this.AssemblyVersion = this.GetAssemblyVersion(source.GetType().Assembly);
    }

    public Package(byte[] data)
    {
      int guidLength = Guid.Empty.ToByteArray().Length;
      if (data.Length < guidLength)
        return;

      byte[] guidPart = new byte[guidLength];
      for (int i = 0; i < guidLength; i++)
      {
        guidPart[i] = data[i];
      }

      byte[] dataPart = new byte[data.LongLength - guidLength];
      for (long i = guidLength; i < data.LongLength; i++)
      {
        dataPart[i - guidLength] = data[i];
      }

      m_type_guid = new Guid(guidPart);
      this.AssemblyVersion = this.GetAssemblyVersion(PackHelper.GetTypeByGuid(this.m_type_guid).Assembly);
      this.Data = dataPart;
    }

    public Guid TypeGuid
    {
      get { return m_type_guid; }
      set
      {
        m_type_guid = value;
        this.AssemblyVersion = this.GetAssemblyVersion(PackHelper.GetTypeByGuid(this.m_type_guid).Assembly);
      }
    }

    public string AssemblyVersion { get; private set; }

    public byte[] Data
    {
      get { return m_data; }
      set { m_data = value ?? new byte[0]; }
    }

    private string GetAssemblyVersion(Assembly asm)
    {
      return asm.GetName().Version.ToString();
    }

    public IPackable CreateInstance()
    {
      if (this.m_type_guid == Guid.Empty)
        return null;

      // Перед тем, как проверить наличие данных, необходимо проверить допустимость типа
      Type targetType = PackHelper.GetTypeByGuid(this.m_type_guid);

      if (m_data.Length == 0)
        return null;

      IPackable item = (IPackable)Activator.CreateInstance(targetType);
      return item.Unpack(m_data) ? item : null;
    }

    public byte[] ToByteArray()
    {
      byte[] guidPart = this.TypeGuid.ToByteArray();
      byte[] ret = new byte[guidPart.Length + this.Data.LongLength];

      for (int i = 0; i < guidPart.Length; i++)
      {
        ret[i] = guidPart[i];
      }
      for (long i = guidPart.Length; i < ret.LongLength; i++)
      {
        ret[i] = this.Data[i - guidPart.Length];
      }
      return ret;
    }
  }


  /// <summary>
  /// Вспомогательный класс для использования словарей в качестве формата упаковки
  /// </summary>
  public static class PackHelper
  {
    private static readonly Dictionary<Guid, Type> typeGuids = new Dictionary<Guid, Type>();
    private static readonly object locker = new object();

    public static Type GetTypeByGuid(Guid typeGuid)
    {
      lock (locker)
      {
        if (typeGuids.Count == 0)
        {
          LoadTypeGuids();
        }
        return typeGuids[typeGuid];
      }
    }

    private static void LoadTypeGuids()
    {
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (!asm.IsAtToolbox())
          continue;

        foreach (Type type in asm.GetTypes())
        {
          if (type.IsAbstract || (!typeof(ISerializable).IsAssignableFrom(type) 
            && !typeof(IPackable).IsAssignableFrom(type)))
            continue;

          typeGuids[type.GUID] = type;
        }
      }
    }

    public static byte[] PackFieldValues(Dictionary<string, object> fieldValues)
    {
      // Преобразуем словарь в массив для уменьшения размера (пустой словарь занимает 1300 байт)
      ObjectTextStore[] items = new ObjectTextStore[fieldValues.Count];
      int i = 0;
      foreach (KeyValuePair<string, object> kv in fieldValues)
      {
        ObjectTextStore item = new ObjectTextStore() { Text = kv.Key };

        // Пакуем вложенные объекты
        IPackable packableMember = kv.Value as IPackable;
        if (packableMember != null)
        {
          item.Value = new Package(packableMember);
        }
        else
        {
          item.Value = kv.Value;
        }
        items[i] = item;
        i++;
      }
      using (MemoryStream ms = new MemoryStream())
      {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, items);
        return ms.ToArray();
      }
    }

    public static Dictionary<string, object> UnpackFieldValues(byte[] data)
    {
      ObjectTextStore[] ret = null;
      using (MemoryStream ms = new MemoryStream(data))
      {
        ret = new BinaryFormatter().Deserialize(ms) as ObjectTextStore[] ?? new ObjectTextStore[0];
      }
      Dictionary<string, object> dic = new Dictionary<string, object>();
      foreach (ObjectTextStore ts in ret)
      {
        // Распаковываем вложенные пакеты
        Package package = ts.Value as Package;
        if (package != null)
        {
          ts.Value = package.CreateInstance();
        }
        dic.Add(ts.Text, ts.Value);
      }
      return dic;
    }
  }
}
