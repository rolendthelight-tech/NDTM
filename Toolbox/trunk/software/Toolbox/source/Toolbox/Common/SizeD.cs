using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;


namespace System.Drawing
{
	/// <summary>
	/// Содержит упорядоченную пару чисел с плавающей запятой, обычно ширину и высоту прямоугольника.
	/// </summary>
	/// <filterpriority>1</filterpriority>
	[ComVisible(true)]
	[TypeConverter(typeof(SizeDConverter))]
	[Serializable]
	public struct SizeD
	{
		/// <summary>
		/// Получает структуру <see cref="T:System.Drawing.SizeD"/>, имеющую значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.SizeD"/>, имеющая значения <see cref="P:System.Drawing.SizeD.Height"/> и <see cref="P:System.Drawing.SizeD.Width"/>, равные 0.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public static readonly SizeD Empty = new SizeD();
		private double width;
		private double height;

				/// <summary>
		/// Инициализирует новый экземпляр структуры <see cref="T:System.Drawing.SizeD"/> из указанной существующей структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// <param name="size">Структура <see cref="T:System.Drawing.SizeD"/>, на основе которой будет создана новая структура <see cref="T:System.Drawing.SizeD"/>.</param>
		public SizeD(SizeD size)
		{
			this.width = size.width;
			this.height = size.height;
		}

		/// <summary>
		/// Инициализирует новый экземпляр структуры <see cref="T:System.Drawing.SizeD"/> из указанной структуры <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// <param name="pt">Структура <see cref="T:System.Drawing.PointD"/>, из которой инициализируется эта структура <see cref="T:System.Drawing.SizeD"/>.</param>
		public SizeD(PointD pt)
		{
			this.width = pt.X;
			this.height = pt.Y;
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public SizeD(double width, double height)
		{
			this.width = width;
			this.height = height;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом SizeF", true)]
		public SizeD(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом SizeF", true)]
		public SizeD(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом SizeF", true)]
		public SizeD(long width, long height)
		{
			this.width = width;
			this.height = height;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом SizeF", true)]
		public SizeD(ulong width, ulong height)
		{
			this.width = width;
			this.height = height;
		}

		/// <summary>
		/// Получает значение, указывающее, имеет ли эта структура <see cref="T:System.Drawing.SizeD"/> нулевую ширину и высоту.
		/// </summary>
		/// 
		/// <returns>
		/// Это свойство возвращает значение true, когда эта структура <see cref="T:System.Drawing.SizeD"/> имеет нулевую ширину и высоту; в противном случае возвращается значение false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		[Browsable(false)]
		public bool IsEmpty
		{
			[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
			get
			{
				if (this.width == 0d)
					return this.height == 0d;
				else
					return false;
			}
		}

		/// <summary>
		/// Получает или задаёт горизонтальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Горизонтальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>, обычно измеряемый в пикселях.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public double Width
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.width;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.width = value;
			}
		}

		/// <summary>
		/// Получает или задаёт вертикальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Вертикальный компонент этой структуры <see cref="T:System.Drawing.SizeD"/>, обычно измеряемый в пикселях.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public double Height
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.height;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.height = value;
			}
		}

		/// <summary>
		/// Преобразует заданную структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.PointD"/>, которая является результатом преобразования, выполненного с помощью этого оператора.
		/// </returns>
		/// <param name="size">Преобразуемая структура <see cref="T:System.Drawing.SizeD"/>.</param><filterpriority>3</filterpriority>
		public static explicit operator PointD(SizeD size)
		{
			return new PointD(size.Width, size.Height);
		}

		/// <summary>
		/// Прибавляет ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> к ширине и высоте другой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.Size"/>, получаемая в результате операции сложения.
		/// </returns>
		/// <param name="sz1">Первая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><param name="sz2">Вторая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><filterpriority>3</filterpriority>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static SizeD operator +(SizeD sz1, SizeD sz2)
		{
			return SizeD.Add(sz1, sz2);
		}

		/// <summary>
		/// Вычитает ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> из ширины и высоты другой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.SizeD"/>, полученная в результате операции вычитания.
		/// </returns>
		/// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора вычитания.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора вычитания.</param><filterpriority>3</filterpriority>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static SizeD operator -(SizeD sz1, SizeD sz2)
		{
			return SizeD.Subtract(sz1, sz2);
		}

		/// <summary>
		/// Проверяет, действительно ли две структуры <see cref="T:System.Drawing.SizeD"/> эквивалентны.
		/// </summary>
		/// 
		/// <returns>
		/// Этот оператор возвращает значение true, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> имеют равные ширину и высоту; в противном случае возвращается значение false.
		/// </returns>
		/// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора равенства.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора равенства.</param><filterpriority>3</filterpriority>
		public static bool operator ==(SizeD sz1, SizeD sz2)
		{
			if (sz1.Width == sz2.Width)
				return sz1.Height == sz2.Height;
			else
				return false;
		}

		/// <summary>
		/// Проверяет, различны ли две структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Этот оператор возвращает значение true, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> отличаются по ширине или по высоте, и значение false, если параметры <paramref name="sz1"/> и <paramref name="sz2"/> равны.
		/// </returns>
		/// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора неравенства.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора неравенства.</param><filterpriority>3</filterpriority>
		public static bool operator !=(SizeD sz1, SizeD sz2)
		{
			return !(sz1 == sz2);
		}

		/// <summary>
		/// Прибавляет ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> к ширине и высоте другой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.SizeD"/>, получаемая в результате операции сложения.
		/// </returns>
		/// <param name="sz1">Первая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param><param name="sz2">Вторая складываемая структура <see cref="T:System.Drawing.SizeD"/>.</param>
		public static SizeD Add(SizeD sz1, SizeD sz2)
		{
			return new SizeD(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		/// <summary>
		/// Вычитает ширину и высоту одной структуры <see cref="T:System.Drawing.SizeD"/> из ширины и высоты другой структуры <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Структура <see cref="T:System.Drawing.SizeD"/>, полученная в результате операции вычитания.
		/// </returns>
		/// <param name="sz1">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится слева от оператора вычитания.</param><param name="sz2">Структура <see cref="T:System.Drawing.SizeD"/>, которая находится справа от оператора вычитания.</param>
		public static SizeD Subtract(SizeD sz1, SizeD sz2)
		{
			return new SizeD(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SizeD))
				return false;
			SizeD sizeD = (SizeD)obj;
			if (sizeD.Width == this.Width && sizeD.Height == this.Height)
				return sizeD.GetType().Equals(this.GetType());
			else
				return false;
		}

		/// <summary>
		/// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.Size"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.Size"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Преобразует структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Возвращает структуру <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public PointD ToPointD()
		{
			return (PointD)this;
		}

		/// <summary>
		/// Преобразует структуру <see cref="T:System.Drawing.SizeD"/> в структуру <see cref="T:System.Drawing.Size"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Возвращает структуру <see cref="T:System.Drawing.Size"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public Size ToSize()
		{
			return new Size(width: (int)this.Width, height: (int)this.Height);
		}

		/// <summary>
		/// Создаёт удобную для восприятия строку, представляющую эту структуру <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Строка, представляющая эту структуру <see cref="T:System.Drawing.SizeD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/></PermissionSet>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{Width={0}, Height={1}}}", this.width, this.height);
		}
	}

  /// <summary>
  /// Преобразует объекты <see cref="T:System.Drawing.SizeD"/> из одного типа в другой.
  /// </summary>
  public class SizeDConverter : TypeConverter
  {
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Drawing.SizeDConverter"/>.
    /// </summary>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public SizeDConverter()
    {
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof (string))
        return true;
      else
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (InstanceDescriptor))
        return true;
      else
        return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str1 = value as string;
      if (str1 == null)
        return base.ConvertFrom(context, culture, value);
      string str2 = str1.Trim();
      if (str2.Length == 0)
        return (object) null;
      if (culture == null)
        culture = CultureInfo.CurrentCulture;
      char ch = culture.TextInfo.ListSeparator[0];
      string[] strArray = str2.Split(new char[1]
      {
        ch
      });
      double[] numArray = new double[strArray.Length];
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (double));
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (double) converter.ConvertFromString(context, culture, strArray[index]);
      if (numArray.Length == 2)
        return (object) new SizeD(numArray[0], numArray[1]);
			throw new ArgumentException(string.Format("Ошибка разбора строки \"{0}\" по формату \"{1}\"", (object)str2, (object)"Width,Height"));
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == (Type) null)
        throw new ArgumentNullException("destinationType");
      if (destinationType == typeof (string) && value is SizeD)
      {
        SizeD sizeD = (SizeD) value;
        if (culture == null)
          culture = CultureInfo.CurrentCulture;
        string separator = culture.TextInfo.ListSeparator + " ";
        TypeConverter converter = TypeDescriptor.GetConverter(typeof (double));
        string[] strArray1 = new string[2];
        int num1 = 0;
        string[] strArray2 = strArray1;
        int index1 = num1;
        int num2 = 1;
        int num3 = index1 + num2;
        string str1 = converter.ConvertToString(context, culture, (object) sizeD.Width);
        strArray2[index1] = str1;
        string[] strArray3 = strArray1;
        int index2 = num3;
        int num4 = 1;
        int num5 = index2 + num4;
        string str2 = converter.ConvertToString(context, culture, (object) sizeD.Height);
        strArray3[index2] = str2;
        return (object) string.Join(separator, strArray1);
      }
      else
      {
        if (destinationType == typeof (InstanceDescriptor) && value is SizeD)
        {
          SizeD sizeD = (SizeD) value;
          ConstructorInfo constructor = typeof (SizeD).GetConstructor(new Type[2]
          {
            typeof (double),
            typeof (double)
          });
          if (constructor != (ConstructorInfo) null)
            return (object) new InstanceDescriptor((MemberInfo) constructor, (ICollection) new object[2]
            {
              (object) sizeD.Width,
              (object) sizeD.Height
            });
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
    {
      return (object) new SizeD((double) propertyValues[(object) "Width"], (double) propertyValues[(object) "Height"]);
    }

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
      return TypeDescriptor.GetProperties(typeof (SizeD), attributes).Sort(new string[2]
      {
        "Width",
        "Height"
      });
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
      return true;
    }
  }
}
