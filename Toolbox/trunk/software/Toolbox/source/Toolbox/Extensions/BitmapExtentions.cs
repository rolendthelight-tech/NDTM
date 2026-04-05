using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Extensions
{
  public static class BitmapExtentions
  {
	  [NotNull]
	  public static byte[] GetBytes([NotNull] this Bitmap bitmap, ImageFormat format = null)
    {
	    if (bitmap == null) throw new ArgumentNullException("bitmap");

			if (format == null)
        format = ImageFormat.Jpeg;

		  using (var stream = new MemoryStream())
		  {
			  bitmap.Save(stream, format);
			  return stream.ToArray().ThrowIfNull("array");
		  }
    }
  }
}
