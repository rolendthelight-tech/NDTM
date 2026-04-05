using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;
using JetBrains.Annotations;
using Toolbox.Application.Services;

namespace Toolbox.Data
{
  public interface ILookupProvider
  {
	  [CanBeNull]
	  IList GetLookupValues(string propertyName);
  }

  public abstract class ErrorMessageBaseAttribute : Attribute
  {
    internal ErrorMessageBaseAttribute([NotNull] string errorMessage)
    {
      if (errorMessage == null) throw new ArgumentNullException("errorMessage");
      if (string.IsNullOrEmpty(errorMessage)) throw new ArgumentException("Empty", "errorMessage");

      this.ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; private set; }

    public InfoLevel Level { get; set; }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class ErrorMessageAttribute : ErrorMessageBaseAttribute
  {
    public ErrorMessageAttribute([NotNull] string errorMessage) : base(errorMessage) { }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class EmptyErrorMessageAttribute : ErrorMessageBaseAttribute
  {
    public EmptyErrorMessageAttribute([NotNull] string errorMessage) : base(errorMessage) { }
 }

  public class LookupTypeConverter : TypeConverter
  {
    public override bool GetStandardValuesSupported([NotNull] ITypeDescriptorContext context)
    {
	    if (context == null) throw new ArgumentNullException("context");

			return context.Instance is ILookupProvider;
    }

	  public override bool GetStandardValuesExclusive([NotNull] ITypeDescriptorContext context)
	  {
		  if (context == null) throw new ArgumentNullException("context");

			return context.Instance is ILookupProvider;
	  }

	  [NotNull]
	  public override StandardValuesCollection GetStandardValues([NotNull] ITypeDescriptorContext context)
    {
		  if (context == null) throw new ArgumentNullException("context");

			var instance = context.Instance as ILookupProvider;

      if (instance != null)
        return new StandardValuesCollection(instance.GetLookupValues(context.PropertyDescriptor.Name));
      else
        return new StandardValuesCollection(new ArrayList());
    }
  }
}
