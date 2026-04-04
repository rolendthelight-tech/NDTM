using System;

namespace AT.Toolbox
{
  public class InvalidPluginManifestException : Exception
  {

    public InvalidPluginManifestException(string message, params object[] args)
      : base(string.Format(message, args))
    {
    }

    public InvalidPluginManifestException(string message)
      : base(message)
    {
    }
  }
}