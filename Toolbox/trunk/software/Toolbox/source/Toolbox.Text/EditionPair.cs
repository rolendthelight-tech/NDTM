namespace Toolbox.Text
{
  public class EditionPair
  {
    public Edition RefEdition;
    public Edition TestEdition;

    public override string ToString()
    {
      return "Ref =" + RefEdition + " Test =" + TestEdition;
    }
  }
}
