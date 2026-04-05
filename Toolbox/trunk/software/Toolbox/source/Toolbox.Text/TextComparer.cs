using System.Collections.Generic;

namespace Toolbox.Text
{
  public static class TextComparer
  {
    public static List<Edition> Compare(List<string> refElements, List<string> testElements, List<string> fillers, bool noEndings = false)
    {
      var fillerIndices = new Dictionary<int, int> {{-1, 0}};
      var shift = 0;
      for (int i = 0; i < refElements.Count; i++)
      {
        if (fillers.Contains(refElements[i]))
          shift++;
        else
          fillerIndices.Add(i - shift, shift);
      }
      refElements.RemoveAll(fillers.Contains);

      const int delimiter = 0;
      int distance;
      List<Edition> editions;
      Levenstein.FindLevensteinDistance(refElements, testElements, out distance, out editions, delimiter, noEndings);
      foreach (var ed in editions)
        ed.Index += fillerIndices[ed.Index];
      return editions;
    }

    public static List<EditionPair> Compare(string refText, string testText, List<string> fillers, char[] separators, bool noEndings = false)
    {
      List<EditionPair> editionPairs;
      var t = testText;
      Levenstein.FindLevensteinDistance(refText, ref t, separators, fillers, out editionPairs, true, noEndings, false);
      return editionPairs;
    }

    public static List<EditionPair> Compare(string refText, ref string testText, List<string> fillers, char[] separators, bool noEndings, bool splitTestTextByRefText)
    {
      List<EditionPair> editionPairs;
      Levenstein.FindLevensteinDistance(refText, ref testText, separators, fillers, out editionPairs, true, noEndings, splitTestTextByRefText);
      return editionPairs;
    }
  }
}
