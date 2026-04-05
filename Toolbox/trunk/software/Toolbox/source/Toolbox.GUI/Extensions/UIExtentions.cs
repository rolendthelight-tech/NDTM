using System.Linq;
using System.Windows.Forms;

namespace Toolbox.GUI.Extensions
{
  public static class UIExtentions
  {
    public static int GetSelectionPos(this TextBoxBase textBox, out int lineNumber)
    {
      int selectionStart = textBox.SelectionStart;
      int lineNo = lineNumber = 1;
      if (string.IsNullOrEmpty(textBox.Text))
      {
        return 1;
      }

      int lastLineEnd = textBox.Lines.Select(line => line.Length).Aggregate(delegate(int pos, int next)
      {
        if (pos < selectionStart)
        {
          lineNo++;
          return pos + next + 1;
        }

        return pos;
      });
      lineNumber = lineNo;

      return selectionStart - (lastLineEnd - textBox.Lines[lineNo - 1].Length) + 1;
    }
  }
}
