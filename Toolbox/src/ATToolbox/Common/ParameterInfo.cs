using System.Runtime.Serialization;

namespace AT.Toolbox
{
  [DataContract]
  public class ParameterInfo
  {
    public const int DefaultFirstWorkHour = 3;

    [DataMember]
    public string AssemblyName { get; set; }

    [DataMember]
    public string TypeName { get; set; }

    [DataMember]
    public bool IsNullable { get; set; }

    [DataMember]
    public bool Browsable { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public string Value { get; set; }
  }
}