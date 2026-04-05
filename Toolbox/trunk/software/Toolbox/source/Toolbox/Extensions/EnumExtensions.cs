using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Toolbox.Common;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
  public static class EnumExtensions
  {
    #region Fields

	  [NotNull] private static readonly ILog _log = LogManager.GetLogger(typeof (EnumExtensions));
	  [NotNull] private static readonly Dictionary<Type, EnumLabelDicts> _enum_labels = new Dictionary<Type, EnumLabelDicts>();

    #endregion

    #region Enum Helpers

	  /// <summary>
	  /// Получает метку у перечисления
	  /// </summary>
	  /// <param name="value">Объект перечислимого типа</param>
	  /// <returns>Метка</returns>
		[NotNull]
	  public static string GetLabel([NotNull] this Enum value)
	  {
		  if (value == null) throw new ArgumentNullException("value");

		  //TODO value.GetType().GetField(value.GetType().GetEnumName(value)).GetCustomAttributes(true)

		  lock (((ICollection) _enum_labels).SyncRoot)
		  {
			  var labels = GetLabels(value.GetType());
			  EnumLabel label;
			  if (labels.EnumDict.TryGetValue(value, out label))
			  {
				  return label.Label;
			  }
		  }
		  return value.ToString();
	  }

	  /// <summary>
    /// Очищает локализованные метки у перечислений
    /// </summary>
    public static void ClearEnumLabels()
    {
      lock (((ICollection) _enum_labels).SyncRoot)
        _enum_labels.Clear();
    }

    public static T ParseEnumLabel<T>([NotNull] string label)
      where T : struct
    {
	    if (label == null) throw new ArgumentNullException("label");

			object result = ParseEnumLabel(typeof(T), label);

	    if (result != null)
		    return (T) result;
	    else
		    return (T) Enum.Parse(typeof (T), label);
    }

	  [CanBeNull]
	  internal static Enum ParseEnumLabel([NotNull] Type enumType, [NotNull] string label)
    {
	    if (enumType == null) throw new ArgumentNullException("enumType");
	    if (label == null) throw new ArgumentNullException("label");

	    lock (((ICollection) _enum_labels).SyncRoot)
	    {
		    var labels = GetLabels(enumType);
		    EnumLabel label2;
		    if (labels.LabelDict.TryGetValue(label, out label2))
			    return label2.Value;
		    else
			    return null;
	    }
    }

		private class EnumLabel : Tuple<Enum, string, string, bool>
    {
			public EnumLabel([NotNull] Enum value, [NotNull] string label, [CanBeNull] string tip, bool obsolete)
				: base(value, label, tip, obsolete)
	    {
		    if (value == null) throw new ArgumentNullException("value");
		    if (label == null) throw new ArgumentNullException("label");
	    }

	    [NotNull]
	    public Enum Value
	    {
		    get { return this.Item1; }
	    }

	    [NotNull]
	    public string Label
	    {
				get { return this.Item2; }
	    }

	    [CanBeNull]
	    public string Tip
	    {
				get { return this.Item3; }
	    }

	    public bool Obsolete
	    {
				get { return this.Item4; }
	    }
    }

		private class EnumLabelDicts : Tuple<Dictionary<Enum, EnumLabel>, Dictionary<string, EnumLabel>>
		{
			public EnumLabelDicts([NotNull] Dictionary<Enum, EnumLabel> enumDict, [NotNull] Dictionary<string, EnumLabel> labelDict)
				: base(enumDict, labelDict)
			{
				if (enumDict == null) throw new ArgumentNullException("enumDict");
				if (labelDict == null) throw new ArgumentNullException("labelDict");
			}

			[NotNull]
			public Dictionary<Enum, EnumLabel> EnumDict
			{
				get { return this.Item1; }
			}

			[NotNull]
			public Dictionary<string, EnumLabel> LabelDict
			{
				get { return this.Item2; }
			}
		}

	  [NotNull]
	  private static EnumLabelDicts GetLabels([NotNull] Type enumType)
    {
	    if (enumType == null) throw new ArgumentNullException("enumType");

			EnumLabelDicts dicts;
			if (!_enum_labels.TryGetValue(enumType, out dicts))
      {
				dicts = new EnumLabelDicts(new Dictionary<Enum, EnumLabel>(), new Dictionary<string, EnumLabel>());
	      foreach (var fi in enumType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
	      {
		      string label = null;
		      string tip = null;
		      bool obsolete = false;

		      if (fi.IsDefined(typeof (DisplayNameAttribute), true))
		      {
			      label = ((DisplayNameAttribute) fi.GetCustomAttributes(typeof (DisplayNameAttribute), true)[0]).DisplayName;
		      }

		      if (fi.IsDefined(typeof (DisplayTipAttribute), true))
		      {
			      tip = ((DisplayTipAttribute) fi.GetCustomAttributes(typeof (DisplayTipAttribute), true)[0]).DisplayTip;
		      }

		      if (fi.IsDefined(typeof (ObsoleteAttribute), true))
		      {
			      obsolete = true;
		      }

		      if (string.IsNullOrEmpty(label))
			      label = fi.Name;

		      var item = new EnumLabel((Enum) fi.GetValue(null), label, tip, obsolete);
		      try
		      {
			      dicts.EnumDict.Add(item.Value, item);
		      }
		      catch (ArgumentException ex)
		      {
			      _log.ErrorFormat(ex, "GetLabels(): пропущен элемент {0}", item.Value);
		      }
					if (item.Label != null)
			      try
			      {
							dicts.LabelDict.Add(item.Label, item);
			      }
			      catch (ArgumentException ex)
			      {
							_log.ErrorFormat(ex, "GetLabels(): пропущена метка \"{0}\" элемента {1}", item.Label, item.Value);
			      }
					else
						_log.ErrorFormat("GetLabels(): пропущена метка null элемента {0}", item.Value);
				}

	      _enum_labels.Add(enumType, dicts);
      }
			return dicts;
    }

    #endregion
  }

}
