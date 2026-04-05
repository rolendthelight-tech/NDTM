using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;

namespace AT.Toolbox
{
  public interface ILookupProvider
  {
    IList GetLookupValues(string propertyName);
  }

  public abstract class ErrorMessageBaseAttribute : Attribute
  {
    internal ErrorMessageBaseAttribute(string errorMessage)
    {
      if (string.IsNullOrEmpty(errorMessage))
        throw new ArgumentNullException("errorMessage");

      this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; private set; }

    public InfoLevel Level { get; set; }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class ErrorMessageAttribute : ErrorMessageBaseAttribute
  {
    public ErrorMessageAttribute(string errorMessage) : base(errorMessage) { }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class EmptyErrorMessageAttribute : ErrorMessageBaseAttribute
  {
    public EmptyErrorMessageAttribute(string errorMessage) : base(errorMessage) { }
 }

  public class LookupTypeConverter : TypeConverter
  {
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return context.Instance is ILookupProvider;
    }

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
      return context.Instance is ILookupProvider;
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      var instance = context.Instance as ILookupProvider;

      if (instance != null)
        return new StandardValuesCollection(instance.GetLookupValues(context.PropertyDescriptor.Name));
      else
        return new StandardValuesCollection(new ArrayList());
    }
  }
}
