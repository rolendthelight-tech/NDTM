namespace AT.Toolbox.Help
{
  public interface IHelpSupported
  {
    HelpMapper HelpMap { get; }
    void ShowToolTip(string str);
  }
}