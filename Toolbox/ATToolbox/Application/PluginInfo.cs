using System.Collections;
using System.Reflection;

namespace AT.Toolbox
{
  public class PluginInfo
  {
    public string Name { get; internal set; }
    public string AssemblyFile { get; internal set; }
    public Assembly Assembly { get; internal set; }
  }
}