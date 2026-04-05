using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Toolbox.Data
{
  /// <summary>
  /// Очередь для передачи потока байт через интерфейс System.IO.Stream
  /// </summary>
  public class QueueStream : Stream
  {
    private readonly object _writeSyncRoot = new object();
    private readonly object _readSyncRoot = new object();
    private readonly LinkedList<ArraySegment<byte>> _segments = new LinkedList<ArraySegment<byte>>();
    private readonly ManualResetEventSlim _dataAvailableResetEvent = new ManualResetEventSlim();

    public override bool CanRead
    {
      get { return true; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      //if (_dataAvailableResetEvent.Wait(ReadTimeout))
      //  throw new TimeoutException("No data available");

      lock (_readSyncRoot)
      {
        int currentCount = 0, currentOffset = 0;

        while (currentCount != count)
        {
          if (_segments.First == null)
            break;

          var segment = _segments.First.Value;
          _segments.RemoveFirst();

          var index = segment.Offset;
          for (; index < segment.Count && currentCount < count; index++)
          {
            if (currentOffset < offset)
              currentOffset++;
            else
              buffer[currentCount++] = segment.Array[index];
          }

          if (currentCount == count)
          {
            if (index < segment.Offset + segment.Count)
              _segments.AddFirst(new ArraySegment<byte>(segment.Array, index, segment.Offset + segment.Count - index));
          }

          if (_segments.Count == 0)
          {
            _dataAvailableResetEvent.Reset();

            return currentCount;
          }
        }

        return currentCount;
      }
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      lock (_writeSyncRoot)
      {
        var copy = new byte[count];
        Array.Copy(buffer, offset, copy, 0, count);

        _segments.AddLast(new ArraySegment<byte>(copy));

        _dataAvailableResetEvent.Set();
      }
    }

    public override void Flush() { }

    public override bool CanSeek
    {
      get { return false; }
    }

    public override void SetLength(long value) { }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return 0;
    }

    public override long Length
    {
      get
      {
        lock (_readSyncRoot)
        {
          return _segments.Sum(a => a.Count - a.Offset);
        }
      }
    }

    public override long Position { get; set; }
  }
}