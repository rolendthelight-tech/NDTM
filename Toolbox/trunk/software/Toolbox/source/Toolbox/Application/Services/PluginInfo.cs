using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Toolbox.Application.Services
{
  public class PluginInfo
  {
    public PluginInfo() { }

		public PluginInfo(string name, [NotNull] [PathReference] string assemblyFile)
    {
			if (assemblyFile == null) throw new ArgumentNullException("assemblyFile");
			if (string.IsNullOrEmpty(assemblyFile)) throw new ArgumentException("Empty", "assemblyFile");

      if (string.IsNullOrEmpty(name))
        name = System.IO.Path.GetFileNameWithoutExtension(assemblyFile);

      this.Name = name;
      this.AssemblyFile = assemblyFile;
    }

    public string Name { get; internal set; }

    public string AssemblyFile { get; internal set; }

    public Assembly Assembly { get; internal set; }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.Name))
        return this.Name;

      else if (!string.IsNullOrEmpty(this.AssemblyFile))
        return this.AssemblyFile;

      return base.ToString();
    }
  }
}