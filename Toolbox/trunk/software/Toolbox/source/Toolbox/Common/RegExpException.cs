using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	[Serializable]
	public class RegExpException : ApplicationException
	{
		public RegExpException([NotNull] String message)
			: base(message)
		{
			if (message == null) throw new ArgumentNullException("message");
		}

		public RegExpException([NotNull] String message, Exception innerException)
			: base(message, innerException)
		{
			if (message == null) throw new ArgumentNullException("message");
		}

		protected RegExpException([NotNull] SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
