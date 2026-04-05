using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Toolbox.Application.Services
{
	[Serializable]
	public class InvalidPluginManifestException : Exception
	{

		[StringFormatMethod("format")]
		public InvalidPluginManifestException([NotNull] string format, [NotNull] params object[] args)
			: base(string.Format(format, args))
		{
		}

		public InvalidPluginManifestException(string message)
			: base(message)
		{
		}

		public InvalidPluginManifestException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidPluginManifestException([NotNull] SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}