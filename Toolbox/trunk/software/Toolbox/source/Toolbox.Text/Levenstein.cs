using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Text
{
  public class Levenstein
  {
    public static int FindLevensteinDistance(string refText, string testText, char separator)
    {
      var refList = refText.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      var testList = testText.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      int distance;
      GetMatrix(testList, refList, out distance);
      return distance;
    }

    static List<TokenInfo> Split(string text, char[] separators)
    {
      var tokens = new List<TokenInfo>();

      if (String.IsNullOrEmpty(text))
        return tokens;

      int index = 0, p0 = 0;
      while (p0 < text.Length)
      {
        int p1 = text.IndexOfAny(separators, p0);

        if (p1 == p0)
        {
          p0 = p1 + 1;
          continue;
        }
        if (p1 < 0)
          p1 = text.Length;

        int l = p1 - p0;
        var token = new TokenInfo { Index = index++, Position = p0, Value = text.Substring(p0, l), Length = l };
        tokens.Add(token);

        p0 = p1 + 1;
      }

      return tokens;
    }

    public static void FindLevensteinDistance(string template, string tested, char[] separators, List<string> fillers, out List<Edition> editions)
    {
      editions = new List<Edition>();

      //if (String.IsNullOrEmpty(template) || String.IsNullOrEmpty(tested))
      //  return;

      var templateList = Split(template, separators);
      int orderIndex = 0;
      foreach (var info in templateList)
      {
        if (fillers == null || !fillers.Contains(info.Value))
          info.OrderIndex = orderIndex++;
      }

      var testedList = Split(tested, separators);
      orderIndex = 0;
      foreach (var info in testedList)
      {
        if (fillers == null || !fillers.Contains(info.Value))
          info.OrderIndex = orderIndex++;
      }

      int distance;
      FindLevensteinDistance(templateList.Where(a => a.OrderIndex >= 0).Select(a => a.Value).ToList(),
        testedList.Where(a => a.OrderIndex >= 0).Select(a => a.Value).ToList(), out distance, out editions);

      var d = templateList.Where(a => a.OrderIndex >= 0).ToDictionary(a => a.OrderIndex, a => a);
      foreach (var e in editions)
      {
        var t = d[e.Index];
        e.Index = t.Index;
        e.Position = t.Position;
        e.Length = t.Length;
      }
    }

    public static void FindLevensteinDistance(string template, ref string tested, char[] separators, List<string> fillers, out List<EditionPair> editionPairs, bool delimiter, bool noEndings, bool splitTestTextByRefText)
    {
      var borders = new List<int>();
      int pos = -1;
      if (splitTestTextByRefText)
      {
        while ((pos = template.IndexOf('\n', pos + 1)) != -1)
          borders.Add(pos);
        tested = tested.Replace('\n', ' ').Replace("  ", " ");
      }

      var templateList = Split(template, separators);
      int orderIndex = 0;
      foreach (var info in templateList)
      {
        if (fillers == null || !fillers.Contains(info.Value))
          info.OrderIndex = orderIndex++;
      }

      var testedList = Split(tested, separators);
      orderIndex = 0;
      foreach (var info in testedList)
      {
        if (fillers == null || !fillers.Contains(info.Value))
          info.OrderIndex = orderIndex++;
      }

      int distance;
      var templ = templateList.Where(a => a.OrderIndex >= 0).Select(a => a.Value).ToList();
      var tocomp = testedList.Where(a => a.OrderIndex >= 0).Select(a => a.Value).ToList();
      int limit = 0;
      if (delimiter && templ.Count > 1000)
      {
        limit = (int)Math.Max(Math.Abs(templ.Count - tocomp.Count) * 1.5, templ.Count / 50);
      }
      List<Edition> templateEditions;
      FindLevensteinDistance(templ, tocomp, out distance, out templateEditions, limit, noEndings);

      editionPairs = CreateEditionPairs(templateEditions);

      var d0 = templateList.Where(a => a.OrderIndex >= 0).ToDictionary(a => a.OrderIndex, a => a);
      var d1 = testedList.Where(a => a.OrderIndex >= 0).ToDictionary(a => a.OrderIndex, a => a);

      foreach (var p in editionPairs)
      {
        {
          var e = p.RefEdition;
          if (e.Index >= 0)
          {

            var t = d0[e.Index];
            e.Index = t.Index;
            e.Position = t.Position;
            if (e.EditionType != EditionType.Insert)
              e.Length = t.Length;
          }
        }
        {
          var e = p.TestEdition;
          if (e == null)
            continue;

          var t = d1[e.Index];
          e.Index = t.Index;
          e.Position = t.Position;
          e.Length = t.Length;
        }
      }

      if (splitTestTextByRefText)
      {
        var shifts = new Dictionary<int, int>();
        for (int i = 0; i < tested.Length; i++)
          shifts.Add(i, 0);
        int shift = 0;
        int prevBorder = 0;
        int prevPos1 = 0;
        int prevPos2 = 0;
        foreach (var border in borders)
        {
          var eds = editionPairs.Where(a => a.RefEdition.Position <= border);
          if (!eds.Any())
          {
            tested = tested.Insert(0, "\r\n");
            shift += 2;
            shifts[0] = shift;
          }
          for (int i = eds.Count() - 1; i >= 0; i--)
          {
            var ed = eds.ElementAt(i);
            if (ed.RefEdition.Position < prevBorder)
            {
              tested = tested.Insert(prevPos2, "\r\n");
              shift += 2;
              shifts[prevPos1] = shift;
              prevBorder = border;
              break;
            }
            if (ed.RefEdition.EditionType == EditionType.Delete)
              continue;
            else
            {
              int pos1 = ed.TestEdition.Position + ed.TestEdition.Length;
              int pos2 = pos1 + shift;
              tested = tested.Insert(pos2, "\r\n");
              shift += 2;
              shifts[pos1] = shift;
              prevPos1 = pos1;
              prevPos2 = pos2;
              prevBorder = border;
              break;
            }
          }
        }
        shift = 0;
        for (int i = 0; i < shifts.Count; i++)
        {
          if (shifts[i] == 0)
            shifts[i] = shift;
          else
            shift = shifts[i];
        }
        editionPairs.RemoveAll(a => a.RefEdition.EditionType == EditionType.Coincidence);
        foreach (var p in editionPairs)
        {
          if (p.RefEdition.EditionType == EditionType.Delete)
            continue;
          var e = p.TestEdition;
          e.Position = e.Position + shifts[e.Position];
        }
      }
      else
        editionPairs.RemoveAll(a => a.RefEdition.EditionType == EditionType.Coincidence);
    }

    public static void FillPosition(List<Edition> editions, string text, char[] separators, List<string> fillers)
    {
      var tokens = Split(text, separators);
      int orderIndex = 0;
      foreach (var t in tokens)
      {
        //if (fillers == null || !fillers.Contains(t.Value))
        t.OrderIndex = orderIndex++;
      }
      var d = tokens.Where(a => a.OrderIndex >= 0).ToDictionary(a => a.OrderIndex, a => a);
      foreach (var e in editions)
      {
        var t = d[e.Index];
        e.Index = t.Index;
        e.Position = t.Position;
        e.Length = t.Length;
      }
    }

    public static void FindLevensteinDistance(List<string> refList, List<string> testList, out int distance, out List<Edition> editions)
    {
      editions = new List<Edition>();
      var matrix = GetMatrix(testList, refList, out distance);

      for (int i = refList.Count, j = testList.Count; i >= 0 && j >= 0; )
      {
        if (i == 0 && j == 0)
          break;
        if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]))
        {
          i--;
          var Ed = new Edition {Index = i, EditionType = EditionType.Delete};
          editions.Add(Ed);
        }
        else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
        {
          j--;
          i--;
          if (matrix[i][j] != matrix[i + 1][j + 1])
          {
            var Ed = new Edition {Index = i, EditionType = EditionType.Replace};
            editions.Add(Ed);
          }
        }
        else
        {
          j--;
          var Ed = new Edition {Index = i - 1, EditionType = EditionType.Insert};
          editions.Add(Ed);
        }
      }
    }


    public static void FindLevensteinDistance(List<string> refList, List<string> testList, out int distance, out List<Edition> editions, int delimiter = 0, bool noEndings = false)
    {
      editions = new List<Edition>();
      List<List<int>> matrix;
      if (noEndings)
      {
        if (delimiter > 0)
          matrix = GetMatrix(testList, refList, out distance, true, delimiter);
        else
          matrix = GetMatrix(testList, refList, out distance, true);
      }
      else
      {
        if (delimiter > 0)
          matrix = GetMatrix(testList, refList, out distance, delimiter);
        else
          matrix = GetMatrix(testList, refList, out distance);
      }

      for (int i = refList.Count, j = testList.Count; i >= 0 && j >= 0; )
      {
        if (delimiter > 0 && (j < i - delimiter || j > i + delimiter))
          continue;
        if (i == 0 && j == 0)
          break;
        if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]) && (delimiter == 0 || j < i + delimiter))
        {
          i--;
          Edition Ed = new Edition();
          Ed.Index = i;
          Ed.EditionType = EditionType.Delete;
          editions.Add(Ed);
        }
        else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] &&
                 matrix[i - 1][j - 1] <= matrix[i - 1][j])
        {
          j--;
          i--;
          if (matrix[i][j] != matrix[i + 1][j + 1])
          {
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.EditionType = EditionType.Replace;
            editions.Add(Ed);
          }
          else
          {
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.EditionType = EditionType.Coincidence;
            editions.Add(Ed);
          }
        }
        else if (delimiter == 0 || j > i - delimiter)
        {
          j--;
          Edition Ed = new Edition();
          Ed.Index = i - 1;
          Ed.EditionType = EditionType.Insert;
          editions.Add(Ed);
        }
        else
        {
          j--;
          i--;
          if (matrix[i][j] != matrix[i + 1][j + 1])
          {
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.EditionType = EditionType.Replace;
            editions.Add(Ed);
          }
          else
          {
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.EditionType = EditionType.Coincidence;
            editions.Add(Ed);
          }
        }
      }
    }

    public static List<List<int>> GetMatrix(List<string> testList, List<string> refList, out int distance)
    {
      var matrix = new List<List<int>>();
      var TempList = new List<int>();
      for (int i = 0; i < testList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < refList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;

      for (int i = 0; i < refList.Count; i++)
      {
        for (int j = 0; j < testList.Count; j++)
        {
          //e = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]);
          if (refList[i] == testList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
          else
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
        }
      }

      distance = matrix[refList.Count][testList.Count];
      return matrix;
    }

    public static List<List<int>> GetMatrix(List<string> testList, List<string> refList, out int distance, int delimiter)
    {
      var matrix = new List<List<int>>();
      var TempList = new List<int>();
      for (int i = 0; i < testList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < refList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;

      for (int i = 0; i < refList.Count; i++)
      {
        int j = i - delimiter;
        if (j < 0)
          j = 0;
        int top = Math.Min(i + delimiter, testList.Count);
        for (; j < top; j++)
        {
          var left = matrix[i + 1][j] == 0 ? matrix[i][j + 1] : matrix[i + 1][j];
          var upper = matrix[i][j + 1] == 0 ? matrix[i + 1][j] : matrix[i][j + 1];
          if (refList[i] == testList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], left + 1), upper + 1);
          else
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], left), upper) + 1;
        }
      }

      distance = matrix[refList.Count][testList.Count];
      return matrix;
    }

    public static List<List<int>> GetMatrix(List<string> testList, List<string> refList, out int distance, bool noEndings)
    {
      var matrix = new List<List<int>>();
      var TempList = new List<int>();
      for (int i = 0; i < testList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < refList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;

      for (int i = 0; i < refList.Count; i++)
      {
        for (int j = 0; j < testList.Count; j++)
        {
          if (refList[i] == testList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
          else
          {
            int length = Math.Min(refList[i].Length, testList[j].Length);
            if (length > 3 && Math.Abs(refList[i].Length - testList[j].Length) < 3)
            {
              if (length == 4)
                length = 3;
              else if (length == 5 || length == 6)
                length = 4;
              else
                length = length - 3;
              if (refList[i].Substring(0, length) == testList[j].Substring(0, length))
              {
                matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1),
                                                matrix[i][j + 1] + 1);
                continue;
              }
            }
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
          }
        }
      }
      distance = matrix[refList.Count][testList.Count];
      return matrix;
    }

    public static List<List<int>> GetMatrix(List<string> testList, List<string> refList, out int distance, bool noEndings, int delimiter)
    {
      var matrix = new List<List<int>>();
      var TempList = new List<int>();
      for (int i = 0; i < testList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < refList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;

      for (int i = 0; i < refList.Count; i++)
      {
        int j = i - delimiter;
        if (j < 0)
          j = 0;
        int top = Math.Min(i + delimiter, testList.Count);
        for (; j < top; j++)
        {
          var left = matrix[i + 1][j] == 0 ? matrix[i][j + 1] : matrix[i + 1][j];
          var upper = matrix[i][j + 1] == 0 ? matrix[i + 1][j] : matrix[i][j + 1];
          if (refList[i] == testList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], left + 1), upper + 1);
          else
          {
            int length = Math.Min(refList[i].Length, testList[j].Length);
            if (length > 3 && Math.Abs(refList[i].Length - testList[j].Length) < 3)
            {
              if (length == 4)
                length = 3;
              else if (length == 5 || length == 6)
                length = 4;
              else
                length = length - 3;
              if (refList[i].Substring(0, length) == testList[j].Substring(0, length))
              {
                matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], left + 1),
                                                upper + 1);
                continue;
              }
            }
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], left), upper) + 1;
          }
        }
      }
      distance = matrix[refList.Count][testList.Count];
      return matrix;
    }

    public static List<EditionPair> CreateEditionPairs(List<Edition> Editions)
    {
      var pairs = new List<EditionPair>();
      int diff = 0;

      foreach (var ed in Editions.OrderBy(a => a.Index))
      {
        var p = new EditionPair { RefEdition = ed };
        pairs.Add(p);

        if (ed.Index < 0)
        {
          p.TestEdition = new Edition { Index = diff++, EditionType = EditionType.Insert };
          continue;
        }

        switch (ed.EditionType)
        {
          case EditionType.Replace:
            p.TestEdition = new Edition() { Index = ed.Index + diff, EditionType = EditionType.Replace };
            continue;
          case EditionType.Coincidence:
            p.TestEdition = new Edition() { Index = ed.Index + diff, EditionType = EditionType.Coincidence };
            continue;
          case EditionType.Delete:
            diff--;
            continue;
          case EditionType.Insert:
            p.TestEdition = new Edition() { Index = ed.Index + 1 + diff, EditionType = EditionType.Insert };
            diff++;
            break;
        }
      }
      return pairs;
    }

    /// <summary> Расчет редакционной строки </summary>    
    public static string CalculateEditString(List<Edition> editions, bool insertNewLine)
    {
      var sb = new StringBuilder();

      foreach (var edition in editions.OrderBy(z => z.Index))
      {
        int i = 0, j = 0;

        switch (edition.EditionType)
        {
          case EditionType.Replace:
            sb.Append(String.Format(" {0}=>{1}", edition.From, edition.To));

            if (insertNewLine)
              sb.Append(Environment.NewLine);
            break;

          case EditionType.Coincidence:
            sb.AppendFormat(" {0}", edition.From);
            if (insertNewLine)
              sb.Append(Environment.NewLine);
            break;

          case EditionType.Delete:
            sb.AppendFormat(" {0}=>[]", edition.From);
            if (insertNewLine)
              sb.Append(Environment.NewLine);
            break;

          case EditionType.Insert:
            sb.AppendFormat(" []=>[{0}]", edition.To);
            if (insertNewLine)
              sb.Append(Environment.NewLine);
            break;
        }
      }
      var result = sb.ToString();
      return result;
    }
  }
}