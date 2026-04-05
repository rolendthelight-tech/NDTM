using System;
using System.IO;

namespace Toolbox.Data
{
	/// <summary>
	/// Обёртка для потока, не позволяющая его закрыть
	/// </summary>
	public class NonCloseableStream : Stream
	{
		private readonly Stream m_stream;
		private bool m_disposed;

		public NonCloseableStream(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			this.m_stream = stream;
			this.m_disposed = false;
		}

		public override void Flush()
		{
			ThrowIfDisposed();
			this.m_stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			ThrowIfDisposed();
			return this.m_stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			ThrowIfDisposed();
			this.m_stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			return this.m_stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			this.m_stream.Write(buffer, offset, count);
		}

		public override bool CanRead
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.Position;
			}
			set
			{
				ThrowIfDisposed();
				this.m_stream.Position = value;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ThrowIfDisposed();
			return this.m_stream.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			ThrowIfDisposed();
			return this.m_stream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			ThrowIfDisposed();
			return this.m_stream.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			ThrowIfDisposed();
			this.m_stream.EndWrite(asyncResult);
		}

		public override bool CanTimeout
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.CanTimeout;
			}
		}

		public override int ReadByte()
		{
			ThrowIfDisposed();
			return this.m_stream.ReadByte();
		}

		public override void WriteByte(byte value)
		{
			ThrowIfDisposed();
			this.m_stream.WriteByte(value);
		}

		public override int ReadTimeout
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.ReadTimeout;
			}
			set
			{
				ThrowIfDisposed();
				this.m_stream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				ThrowIfDisposed();
				return this.m_stream.WriteTimeout;
			}
			set
			{
				ThrowIfDisposed();
				this.m_stream.WriteTimeout = value;
			}
		}

		public override void Close()
		{
			base.Close(); // Здесь нет ошибки
		}

		protected override void Dispose(bool disposing)
		{
			this.m_disposed = true;
			base.Dispose(disposing); // Здесь нет ошибки
		}

		private void ThrowIfDisposed()
		{
			if(this.m_disposed)
				throw new ObjectDisposedException(this.GetType().Name);
		}
	}
}