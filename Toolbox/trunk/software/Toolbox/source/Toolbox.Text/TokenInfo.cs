namespace Toolbox.Text
{
  class TokenInfo
  {
    public string Value;
    public int Index;
    public int Position;
    public int Length;
    public int OrderIndex = -1;

    public override string ToString()
    {
      return Value + ", pos = " + Position + ", Index = " + Index + ", OrderIndex = " + OrderIndex + ", Length = " +
             Length;
    }
  }
}