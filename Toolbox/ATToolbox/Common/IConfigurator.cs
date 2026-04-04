using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using log4net;

namespace AT.Toolbox
{
  public interface IConfigurator
  {
    bool LoadSection(Type sectionType, InfoBuffer buffer);

    TSection GetSection<TSection>() where TSection : ConfigurationSection, new();

    void SaveSection<TSection>(TSection section) where TSection : ConfigurationSection, new();

    void HandleError(Exception error);

    void ApplySettings();

    void SaveSettings();
  }

  [Serializable]
  [DataContract]
  public abstract class ConfigurationSection
  {
    private static readonly ILog _log = LogManager.GetLogger("ConfigurationSection");
    
    [OnDeserializing]
    private void OnDeserializing(StreamingContext context)
    {
      try
      {
        this.LoadDefaults();
      }
      catch (Exception ex)
      {
        _log.Error("OnDeserializing() Exception", ex);
      }
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      this.BeforeSave();
    }
    
    protected virtual void LoadDefaults()
    {
      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(this))
      {
        if (pd.IsReadOnly)
          continue;

        var def = pd.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

        if (def != null)
          pd.SetValue(this, def.Value);
      }
    }

    public virtual bool Validate(InfoBuffer buffer)
    {
      //TODO: имеет смысл в базовом классе писать предупреждение в log4net, не выводя его пользователю
      return true;
    }

    public virtual void ApplySettings()
    {
    }

    protected ConfigurationSection()
    {
      this.LoadDefaults();
    }

    protected internal virtual void OnError(Exception error)
    {
    }

    protected virtual void BeforeSave()
    {
    }
  }
}
