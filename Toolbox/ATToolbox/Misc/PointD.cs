namespace System.Drawing
{

	using System.Diagnostics;

	using System.Drawing;
	using System.ComponentModel;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;

	/** 
	 * Represents a point in 2D coordinate space
	 * (double precision floating-point coordinates) 
	 */
	/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD"]/*' />
	/// <devdoc>
	///    Represents an ordered pair of x and y coordinates that 
	///    define a point in a two-dimensional plane.
	/// </devdoc> 
	[Serializable]
	[System.Runtime.InteropServices.ComVisible(true)]
	public struct PointD
	{


		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.Empty"]/*' />
		/// <devdoc> 
		///    <para>
		///       Creates a new instance of the <see cref='System.Drawing.PointD'/> class 
		///       with member data left uninitialized. 
		///    </para>
		/// </devdoc> 
		public static readonly PointD Empty = new PointD();
		private double x;
		private double y;
		/** 
		 * Create a new Point object at the given location
		 */
		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.PointD"]/*' /> 
		/// <devdoc>
		///    <para> 
		///       Initializes a new instance of the <see cref='System.Drawing.PointD'/> class
		///       with the specified coordinates.
		///    </para>
		/// </devdoc> 
		public PointD(double x, double y)
		{
			this.x = x;
			this.y = y;
		}


		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.IsEmpty"]/*' />
		/// <devdoc>
		///    <para> 
		///       Gets a value indicating whether this <see cref='System.Drawing.PointD'/> is empty.
		///    </para> 
		/// </devdoc> 
		[Browsable(false)]
		public bool IsEmpty
		{
			get
			{
				return x == 0d && y == 0d;
			}
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.X"]/*' /> 
		/// <devdoc> 
		///    <para>
		///       Gets the x-coordinate of this <see cref='System.Drawing.PointD'/>. 
		///    </para>
		/// </devdoc>
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.Y"]/*' />
		/// <devdoc> 
		///    <para>
		///       Gets the y-coordinate of this <see cref='System.Drawing.PointD'/>. 
		///    </para> 
		/// </devdoc>
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.operator+"]/*' /> 
		/// <devdoc>
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by a given <see cref='System.Drawing.Size'/> .
		///    </para> 
		/// </devdoc>
		public static PointD operator +(PointD pt, Size sz)
		{
			return Add(pt, sz);
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.operator-"]/*' />
		/// <devdoc>
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by the negative of a given <see cref='System.Drawing.Size'/> . 
		///    </para>
		/// </devdoc> 
		public static PointD operator -(PointD pt, Size sz)
		{
			return Subtract(pt, sz);
		}

		/// <devdoc>
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by a given <see cref='System.Drawing.SizeF'/> . 
		///    </para>
		/// </devdoc> 
		public static PointD operator +(PointD pt, SizeF sz)
		{
			return Add(pt, sz);
		}

		/// <devdoc>
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by the negative of a given <see cref='System.Drawing.SizeF'/> . 
		///    </para>
		/// </devdoc> 
		public static PointD operator -(PointD pt, SizeF sz)
		{
			return Subtract(pt, sz);
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.operator=="]/*' />
		/// <devdoc>
		///    <para> 
		///       Compares two <see cref='System.Drawing.PointD'/> objects. The result specifies
		///       whether the values of the <see cref='System.Drawing.PointD.X'/> and <see cref='System.Drawing.PointD.Y'/> properties of the two <see cref='System.Drawing.PointD'/> 
		///       objects are equal. 
		///    </para>
		/// </devdoc> 
		public static bool operator ==(PointD left, PointD right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.operator!="]/*' />
		/// <devdoc> 
		///    <para> 
		///       Compares two <see cref='System.Drawing.PointD'/> objects. The result specifies whether the values
		///       of the <see cref='System.Drawing.PointD.X'/> or <see cref='System.Drawing.PointD.Y'/> properties of the two 
		///    <see cref='System.Drawing.PointD'/>
		///    objects are unequal.
		/// </para>
		/// </devdoc> 
		public static bool operator !=(PointD left, PointD right)
		{
			return !(left == right);
		}

		/// <devdoc> 
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by a given <see cref='System.Drawing.Size'/> .
		///    </para>
		/// </devdoc> 
		public static PointD Add(PointD pt, Size sz)
		{
			return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
		}

		/// <devdoc> 
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by the negative of a given <see cref='System.Drawing.Size'/> .
		///    </para>
		/// </devdoc> 
		public static PointD Subtract(PointD pt, Size sz)
		{
			return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
		}

		/// <devdoc> 
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by a given <see cref='System.Drawing.SizeF'/> .
		///    </para>
		/// </devdoc> 
		public static PointD Add(PointD pt, SizeF sz)
		{
			return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
		}

		/// <devdoc> 
		///    <para>
		///       Translates a <see cref='System.Drawing.PointD'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
		///    </para>
		/// </devdoc> 
		public static PointD Subtract(PointD pt, SizeF sz)
		{
			return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.Equals"]/*' /> 
		/// <devdoc>
		///    <para>[To be supplied.]</para>
		/// </devdoc>
		public override bool Equals(object obj)
		{
			if (!(obj is PointD)) return false;
			PointD comp = (PointD)obj;
			return
			comp.X == this.X &&
			comp.Y == this.Y &&
			comp.GetType().Equals(this.GetType());
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.GetHashCode"]/*' /> 
		/// <devdoc>
		///    <para>[To be supplied.]</para> 
		/// </devdoc> 
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <include file='doc\PointD.uex' path='docs/doc[@for="PointD.ToString"]/*' />
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", x, y);
		}
	}
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
