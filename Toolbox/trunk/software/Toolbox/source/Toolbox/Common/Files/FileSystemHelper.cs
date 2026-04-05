using System;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using log4net;

namespace Toolbox.Common.Files
{
  public static class FileSystemHelper
  {
    private const int MaxOperationAttempts = 10;
    private const int FileSystemOperationLatence = 1000;

	  [NotNull] private static readonly ILog _log = LogManager.GetLogger("FileSystemHelper");

    public static void CreateDirectory(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return;

      if (Directory.Exists(path))
        return;

      var maxAttempts = MaxOperationAttempts;
      while (true)
      {
        try
        {
          if (Directory.Exists(path))
            return;

          Directory.CreateDirectory(path);
          break;
        }
        catch (Exception ex)
        {
          if (maxAttempts-- == 0)
            throw;

          _log.Warn(string.Format("CreateDirectory(path = {0}): ", path), ex);
          Thread.Sleep(FileSystemOperationLatence);
        }
      }
    }

    public static void DeleteDirectory(string path)
    {
      DeleteDirectory(path, false);
    }

    public static void DeleteDirectory(string path, bool recursive)
    {
      if (string.IsNullOrWhiteSpace(path))
        return;

      if (!Directory.Exists(path))
        return;

      var maxAttempts = MaxOperationAttempts;
      while (true)
      {
        try
        {
          if (!Directory.Exists(path))
            return;

          Directory.Delete(path, recursive);
          break;
        }
        catch (Exception ex)
        {
          if (maxAttempts-- == 0)
            throw;

          _log.Warn(string.Format("DeleteDirectory(path = {0}): ", path), ex);
          Thread.Sleep(FileSystemOperationLatence);
        }
      }
    }

	  [NotNull]
	  public static string[] ReadAllLines([NotNull] [PathReference] string path, [NotNull] Encoding encoding)
    {
	    if (path == null) throw new ArgumentNullException("path");
	    if (encoding == null) throw new ArgumentNullException("encoding");

	    path = path.Replace('\\', Path.DirectorySeparatorChar); // for support Mono
      path = path.Replace('/', Path.DirectorySeparatorChar);

      if (!File.Exists(path))
        throw new FileNotFoundDetalizedException("Can't find file", path);

      var maxAttempts = MaxOperationAttempts;
      while (true)
      {
        try
        {
          return File.ReadAllLines(path, encoding);
        }
        catch (Exception ex)
        {
          if (maxAttempts-- == 0)
            throw;

          _log.Warn(string.Format("ReadAllLines(path = {0}): ", path), ex);
          Thread.Sleep(FileSystemOperationLatence);
        }
      }
    }
  }
}
