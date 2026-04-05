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
	public class ResourcePoolEndedException<T> : InvalidOperationException
		where T : class, IDisposable
	{
		[NonSerialized]
		[CanBeNull]
		private readonly IResourcePool<T> m_pool;

		public ResourcePoolEndedException(String message, [NotNull] IResourcePool<T> pool)
			: base(message)
		{
			if (pool == null) throw new ArgumentNullException("pool");

			this.m_pool = pool;
		}

		public ResourcePoolEndedException(String message, Exception innerException, [NotNull] IResourcePool<T> pool)
			: base(message, innerException)
		{
			if (pool == null) throw new ArgumentNullException("pool");

			this.m_pool = pool;
		}

		protected ResourcePoolEndedException([NotNull] SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[CanBeNull]
		public IResourcePool<T> Pool
		{
			get { return this.m_pool; }
		}
	}
}
