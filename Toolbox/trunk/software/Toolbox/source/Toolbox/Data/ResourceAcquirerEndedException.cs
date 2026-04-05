using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Data;

namespace Toolbox.Data
{
	[Serializable]
	public class ResourceAcquirerEndedException<T> : InvalidOperationException
	{
		[NonSerialized]
		[CanBeNull]
		private readonly IResourceAcquirer<T> m_acquirer;

		public ResourceAcquirerEndedException(String message, [NotNull] IResourceAcquirer<T> acquirer)
			: base(message)
		{
			if (acquirer == null) throw new ArgumentNullException("acquirer");

			this.m_acquirer = acquirer;
		}

		public ResourceAcquirerEndedException(String message, Exception innerException, [NotNull] IResourceAcquirer<T> acquirer)
			: base(message, innerException)
		{
			if (acquirer == null) throw new ArgumentNullException("acquirer");

			this.m_acquirer = acquirer;
		}

		protected ResourceAcquirerEndedException([NotNull] SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[CanBeNull]
		public IResourceAcquirer<T> Acquirer
		{
			get { return this.m_acquirer; }
		}
	}
}
