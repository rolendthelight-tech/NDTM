using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	/// <summary>
	/// Представляет упорядоченную пару координат Х и Y с плавающей запятой, определяющую точку на двумерной плоскости.
	/// </summary>
	/// <filterpriority>1</filterpriority>
	[ComVisible(true)]
	[Serializable]
	public struct PointD
	{
		/// <summary>
		/// Представляет новый экземпляр класса <see cref="T:System.Drawing.PointD"/> с неинициализированными данными членов.
		/// </summary>
		/// <filterpriority>1</filterpriority>
		public static readonly PointD Empty = new PointD();
		private double x;
		private double y;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="T:System.Drawing.PointD"/> с указанными координатами.
		/// </summary>
		/// 
		/// <param name="x">Положение точки по горизонтали.</param>
		/// <param name="y">Положение точки по вертикали.</param>
		/// <filterpriority>1</filterpriority>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public PointD(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом PointF", true)]
		public PointD(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом PointF", true)]
		public PointD(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом PointF", true)]
		public PointD(long x, long y)
		{
			this.x = x;
			this.y = y;
		}

		[Obsolete("Использование неявного приведения типов для этого класса не рекомендуется. Возможно вы перепутали его с классом PointF", true)]
		public PointD(ulong x, ulong y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Получает значение, определяющее, пуст ли класс <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Значение true, если оба свойства <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> равны 0, в противном случае возвращается значение false.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return x == 0d && y == 0d;
			}
		}

		/// <summary>
		/// Получает или задаёт координату Х точки <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Координата Х точки <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public double X
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.x;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.x = value;
			}
		}

		/// <summary>
		/// Получает или задаёт координату Y точки <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Координата Y точки <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public double Y
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.y;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.y = value;
			}
		}

		/// <summary>
		/// Смещает точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Возвращает перенесённую точку <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий пару чисел, которые нужно добавить к значениям координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static PointD operator +(PointD pt, Size sz)
		{
			return PointD.Add(pt, sz);
		}

		/// <summary>
		/// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательное значение, заданное параметром <see cref="T:System.Drawing.Size"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param><filterpriority>3</filterpriority>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static PointD operator -(PointD pt, Size sz)
		{
			return PointD.Subtract(pt, sz);
		}

		/// <summary>
		/// Смещает точка <see cref="T:System.Drawing.PointD"/> на указанный размер <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для добавления к координатам X и Y точки <see cref="T:System.Drawing.PointD"/>.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static PointD operator +(PointD pt, SizeD sz)
		{
			return PointD.Add(pt, sz);
		}

		/// <summary>
		/// Смещает точку <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static PointD operator -(PointD pt, SizeD sz)
		{
			return PointD.Subtract(pt, sz);
		}

		/// <summary>
		/// Сравнивает две структуры <see cref="T:System.Drawing.PointD"/>. Результат определяет, равны или нет значения свойств <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> двух структур <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Значение true, если значения <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> левой и правой структур <see cref="T:System.Drawing.PointD"/> равны; в противном случае — false.
		/// </returns>
		/// <param name="left">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><filterpriority>3</filterpriority>
		public static bool operator ==(PointD left, PointD right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		/// <summary>
		/// Определяет, равны или нет координаты указанных точек.
		/// </summary>
		/// 
		/// <returns>
		/// Значение true, чтобы указать, что значения <see cref="P:System.Drawing.PointD.X"/> и <see cref="P:System.Drawing.PointD.Y"/> параметров <paramref name="left"/> и <paramref name="right"/> не равны; в противном случае — false.
		/// </returns>
		/// <param name="left">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><param name="right">Объект <see cref="T:System.Drawing.PointD"/> для сравнения.</param><filterpriority>3</filterpriority>
		public static bool operator !=(PointD left, PointD right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Смещает указанную точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.Size"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.Size"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
		public static PointD Add(PointD pt, Size sz)
		{
			return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
		}

		/// <summary>
		/// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.Size"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
		public static PointD Subtract(PointD pt, Size sz)
		{
			return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
		}

		/// <summary>
		/// Смещает указанную точку <see cref="T:System.Drawing.PointD"/> на заданное значение <see cref="T:System.Drawing.SizeD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Объект <see cref="T:System.Drawing.SizeD"/>, определяющий числа, которые нужно добавить к значениям координат <paramref name="pt"/>.</param>
		public static PointD Add(PointD pt, SizeD sz)
		{
			return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
		}

		/// <summary>
		/// Смещает <see cref="T:System.Drawing.PointD"/> на отрицательную величину заданного размера.
		/// </summary>
		/// 
		/// <returns>
		/// Смещённая точка <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <param name="pt">Класс <see cref="T:System.Drawing.PointD"/> для преобразования.</param><param name="sz">Размер <see cref="T:System.Drawing.SizeD"/>, указывающий числа для вычитания из координат <paramref name="pt"/>.</param>
		public static PointD Subtract(PointD pt, SizeD sz)
		{
			return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
		}

		/// <summary>
		/// Определяет, содержит или нет объект <see cref="T:System.Drawing.PointD"/> те же координаты, что и указанный объект <see cref="T:System.Object"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Метод возвращает значение true, если <paramref name="obj"/> является <see cref="T:System.Drawing.PointD"/> и имеет такие же координаты, как и <see cref="T:System.Drawing.Point"/>.
		/// </returns>
		/// <param name="obj">Политика <see cref="T:System.Object"/> для проверки.</param><filterpriority>1</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is PointD))
				return false;
			PointD pointD = (PointD)obj;
			if (pointD.X == this.X && pointD.Y == this.Y)
				return pointD.GetType().Equals(this.GetType());
			else
				return false;
		}

		/// <summary>
		/// Возвращает хэш-код для этой структуры <see cref="T:System.Drawing.PointD"/>.
		/// </summary>
		/// 
		/// <returns>
		/// Целое значение, указывающее значение хэша для этой структуры <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Преобразует объект <see cref="T:System.Drawing.PointD"/> в строку, доступную для чтения.
		/// </summary>
		/// 
		/// <returns>
		/// Строка, представляющая структуру <see cref="T:System.Drawing.PointD"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", this.x, this.y);
		}
	}
}
