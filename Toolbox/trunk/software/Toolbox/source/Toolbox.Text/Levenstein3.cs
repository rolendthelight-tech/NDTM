using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolbox.Text
{
  public class Levenstein3
  {
     public enum type { delete, insert, replace, partcoincidence, coincidence }

    public class Edition
    {
      /// <summary>
      /// индекс символа "эталонной" строки (Template), который должен быть, в соответствии с типом type,
      /// удален, заменен или ПОСЛЕ которого должен быть вставлен символ строки StrToCompare; т.о. может быть
      /// равен -1, если вставка символа должна быть осуществлена в начало строки Template
      /// </summary>
      public int Index;

      public type Type;
      /// <summary>
      /// Позиция элемента в строке
      /// </summary>
      public int Position;
      /// <summary>
      /// Длина элемента [символ]
      /// </summary>
      public int Length;

      public override string ToString()
      {
        return Index + ", " + Type;
      }
       public Edition()
       {}
       public Edition(int index, int position, type type)
       {
          Index = index;
          Position = position;
          Type = type;
       }
    };

    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator)
    {
      var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      int distance;
      GetMatrix(ToCompareList, TemplateList, out distance);
      return distance;
    }

    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator, out int coincidence)
    {
       var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
       var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
       int distance;
       GetMatrix(ToCompareList, TemplateList, out distance, out coincidence);
       return distance;
    }


    public static int FindLevensteinDistance(List<string> Template, List<string> StrToCompare)
    {
        int distance;
        GetMatrix(StrToCompare, Template, out distance);
        return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, out int length)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, out distance);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, List<List<int>> matrix, out int length)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, matrix, out distance);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, out int length, string skip)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, out distance, skip);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, List<List<int>> matrix, out int length, string skip)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, matrix, out distance, skip);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, out int length, out string best, out int coincidence)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, out distance, out best, out coincidence);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, out int length, out string best, out int coincidence, string skip, bool fullLength = false)
    {
       int distance;
       length = GetMatrix(GraphToCompare, Template, out distance, out best, out coincidence, skip, fullLength);
       return distance;
    }

    public static int FindLevensteinDistance(List<string> Template, List<List<string>> GraphToCompare, string skip, out List<Edition> Editions)
    {
       int coincidence;
       Editions = new List<Edition>();
       GetMatrix(GraphToCompare, Template, out coincidence, skip, Editions);
       return coincidence;
    }

    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator, int delimiter)
    {
      var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      int distance;
      GetMatrix(ToCompareList, TemplateList, delimiter, out distance);
      return distance;
    }

    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator, int delimiter, List<List<int>> matrix)
    {
      var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      int distance;
      GetMatrix(ToCompareList, TemplateList, delimiter, out distance, matrix);
      return distance;
    }


    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator, out List<Edition> editions)
    {
      int distance;
      var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
      FindLevensteinDistance(TemplateList, ToCompareList, out distance, out editions);
      return distance;
    }

    public static int FindMaxCoincidence(string Template, string StrToCompare, char separator, bool weighted = false)
    {
        var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        int distance;
        GetMatrixCoincidence(ToCompareList, TemplateList, out distance, weighted);
        return distance;
    }

    public static int FindMaxCoincidence(string Template, string StrToCompare, char separator, out List<int> CoincidenceIndices)
    {
        var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        int distance;
        GetMatrixCoincidence(ToCompareList, TemplateList, out distance, out CoincidenceIndices);
        return distance;
    }


    public static double FindMahalanobisDistance(List<string> Template, List<Dictionary<string, double>> GraphToCompare, Dictionary<string, double>Deletes, out int length)
    {
       double distance;
       length = GetMatrix(GraphToCompare, Template, Deletes, out distance);
       return distance;
    }


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

    static List<TokenInfo> Split(string text, string separator)
    {
      var tokens = new List<TokenInfo>();

      if (String.IsNullOrEmpty(text))
        return tokens;

   
      int index = 0, p0 = 0;
      while (p0 < text.Length)
      {
        int p1 = text.IndexOf(separator, p0);

        if (p1 == p0)
        {
          p0 += p1 + separator.Length;
          continue;
        }
        if (p1 < 0)
          p1 = text.Length;

        int l = p1 - p0;
        var token = new TokenInfo {Index = index++, Position = p0, Value = text.Substring(p0, l), Length = l};
        tokens.Add(token);

        p0 = p1 + separator.Length;
      }

      return tokens;
    }

    public static void FindLevensteinDistance(string template, string tested, string separator, List<string> paulers, out List<Edition> editions)
    {
      editions = new List<Edition>();

      //if (String.IsNullOrEmpty(template) || String.IsNullOrEmpty(tested))
        //return;

      var templateList = Split(template, separator);
      int orderIndex = 0;
      foreach (var info in templateList)
      {
        if (paulers == null || !paulers.Contains(info.Value))
          info.OrderIndex = orderIndex++;
      }

      var testedList = Split(tested, separator);
      orderIndex = 0;
      foreach (var info in testedList)
      {
        if (paulers == null || !paulers.Contains(info.Value))
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


    public static void FillPosition(List<Edition> editions, string text, string separator, List<string> paulers)
    {
      var tokens = Split(text, separator);
      int orderIndex = 0;
      foreach (var t in tokens)
      {
        //if (paulers == null || !paulers.Contains(t.Value))
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


    public static void FindMaxSubstring(List<string> TemplateList, List<string> ToCompareList, out int length, out List<Edition> Edition)
    {
       Edition = new List<Edition>();
       List<List<int>> matrix = GetMatrix(ToCompareList, TemplateList, out length, true);

       int i = 0;
       int j = 0; 
       while (i < TemplateList.Count && j < ToCompareList.Count)
       {
          if (matrix[i + 1][j + 1] < matrix[i + 1][j])
          {
             if (TemplateList[i].Length == 1)
             {
                Edition Ed = new Edition();
                Ed.Index = i;
                Ed.Position = j-1;
                Ed.Type = type.insert;
                Edition.Add(Ed);
             }
             i++;
          }
          else if (matrix[i + 1][j + 1] < matrix[i][j + 1])
             j++;
          else if (TemplateList[i] == ToCompareList[j])
          {
             Edition Ed = new Edition();
             Ed.Index = i;
             Ed.Position = j;
             Ed.Type = type.coincidence;
             Edition.Add(Ed);
             i++;
             j++;
          }
          else if (TemplateList[i].Length > 5 && ToCompareList[j].Length > 5 && TemplateList[i].Substring(0, 5) == ToCompareList[j].Substring(0, 5))
          {
             Edition Ed = new Edition();
             Ed.Index = i;
             Ed.Position = j;
             Ed.Type = type.partcoincidence;
             Edition.Add(Ed);
             i++;
             j++;
          }
          else if (matrix[i + 1][j] >= matrix[i][j + 1])
          {
             if (TemplateList[i].Length == 1)
             {
                Edition Ed = new Edition();
                Ed.Index = i;
                Ed.Position = j - 1;
                Ed.Type = type.insert;
                Edition.Add(Ed);
             }
             i++;
          }
          else
             j++;
       }
    }
    
     
    public static void FindLevensteinDistance(List<string> TemplateList, List<string> ToCompareList, out int distance, out List<Edition> Edition)
    {
      Edition = new List<Edition>();
      List<List<int>> matrix = GetMatrix(ToCompareList, TemplateList, out distance);

      for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0; )
      {
        if (i == 0 && j == 0)
          break;
        if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]))
        {
          i--;
          Edition Ed = new Edition();
          Ed.Index = i;
          Ed.Position = j;
          Ed.Type = type.delete;
          Edition.Add(Ed);
        }
        else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
        {
          j--;
          i--;
          if (matrix[i][j] != matrix[i + 1][j + 1])
          {
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.Position = j;
            Ed.Type = type.replace;
            Edition.Add(Ed);
          }
        }
        else
        {
          j--;
          Edition Ed = new Edition();
          Ed.Index = i - 1;
          Ed.Position = j;
          Ed.Type = type.insert;
          Edition.Add(Ed);
        }
      }
    }

    public static int FindLevensteinDistance(string Template, string StrToCompare, char separator, ref Dictionary<int, int> coincidences)
    {
        int distance;
        var TemplateList = Template.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var ToCompareList = StrToCompare.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        List<List<int>> matrix = GetMatrix(ToCompareList, TemplateList, out distance);

        for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0; )
        {
            if (i == 0 && j == 0)
                break;
            if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]))
            {
                i--;
            }
            else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
            {
                j--;
                i--;
                if (matrix[i][j] == matrix[i + 1][j + 1])
                {
                    coincidences.Add(i, j);
                }
            }
            else
            {
                j--;
            }
        }
        return distance;
    }

    public static List<List<int>> GetMatrix(List<string> ToCompareList, List<string> TemplateList, out int distance)
    {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;

      for (int i = 0; i < TemplateList.Count; i++)
      {
        for (int j = 0; j < ToCompareList.Count; j++)
        {
          //e = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]);
          if (TemplateList[i] == ToCompareList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
          else
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
        }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return matrix;
    }

    public static List<List<int>> GetMatrix(List<string> ToCompareList, List<string> TemplateList, out int distance, out int coincidence)
    {
       List<List<int>> matrix = new List<List<int>>();
       List<int> TempList = new List<int>();
       for (int i = 0; i < ToCompareList.Count + 1; i++)
          TempList.Add(0);
       for (int i = 0; i < TemplateList.Count + 1; i++)
          matrix.Add(new List<int>(TempList));
       for (int i = 0; i < matrix.Count; i++)
          matrix[i][0] = i;
       for (int i = 0; i < matrix[0].Count; i++)
          matrix[0][i] = i;

       for (int i = 0; i < TemplateList.Count; i++)
       {
          for (int j = 0; j < ToCompareList.Count; j++)
          {
             if (TemplateList[i] == ToCompareList[j])
                matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
             else
                matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
          }
       }
       distance = matrix[TemplateList.Count][ToCompareList.Count];

       coincidence = 0;
       var v = new List<string>();
       for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0; )
       {
          if (i == 0 || j == 0)
             break;
          
          if (matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
          {
             if (TemplateList[i - 1] == ToCompareList[j - 1])
             {
                coincidence++;
                v.Add(TemplateList[i - 1]);
                j--;
                i--;
                continue;
             }
          }
          if ((matrix[i - 1][j - 1] < matrix[i][j - 1] && matrix[i - 1][j - 1] < matrix[i - 1][j]))
          {
             j--;
             i--;
          }
          else if (matrix[i-1][j] <= matrix[i][j-1])
          {
             i--;
          }
          else
          {
             j--;
          }
       }
       /*string s = "";
       for (int i = v.Count-1; i >=0; i--)
       {
          s += " " + v[i];
       }
       Console.WriteLine(s);
       UnloadMatrix(matrix, ToCompareList, TemplateList, "c:\\Users\\stanislav\\main\\tests\\matrix.txt");*/
       return matrix;
    }


   private static List<List<int>> GetMatrix(List<string> ToCompareList, List<string> TemplateList, int delimiter, out int distance)
    {
      delimiter = Math.Max(delimiter, Math.Abs(ToCompareList.Count - TemplateList.Count));
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
        matrix.Add(new List<int>(TempList));
      /*for (int i = 0; i < matrix.Count; i++)
        matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
        matrix[0][i] = i;*/
      for (int i = 0; i < matrix.Count; i++)
        for (int j = 0; j < matrix[i].Count; j++)
          matrix[i][j] = Math.Abs(i - j);

      for (int i = 0; i < TemplateList.Count; i++)
      {
        int j = i-delimiter;
        if (j < 0)
          j = 0;
        int top = Math.Min(i+delimiter, ToCompareList.Count);
        for (; j < top; j++)
        {
          if (TemplateList[i] == ToCompareList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
          else
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
        }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return matrix;
    }


   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, out int distance)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
         matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
         matrix[0][i] = i;

      int length = 0;
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (matrix[TemplateList.Count][j+1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            return j;
         }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return ToCompareList.Count;
   }

   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, List<List<int>> matrix, out int distance)
   {
      int length = 0;
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            return j;
         }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return ToCompareList.Count;
   }


   public static List<List<int>> GetMatrix(List<string> ToCompareList, List<string> TemplateList, out int length, bool substring)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int j = ToCompareList.Count-1; j >= 0; j--)
      {
         for (int i = TemplateList.Count-1; i >= 0; i--)
         {
            if (ToCompareList[j] == TemplateList[i] || (TemplateList[i].Length > 5 && ToCompareList[j].Length > 5 && TemplateList[i].Substring(0, 5) == ToCompareList[j].Substring(0, 5)))
               matrix[i][j] = 1 + matrix[i + 1][j + 1];
            else
               matrix[i][j] = Math.Max(matrix[i][j + 1], matrix[i + 1][j]);
         }
      }

      length = matrix[0][0];
      return matrix;
   }


   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, out int distance, string skip)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
         matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
         matrix[0][i] = i;

      int length = 0;
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else if (ToCompareList[j].Contains(skip))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i][j + 1]) + 1, matrix[i + 1][j]);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            return j;
         }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return ToCompareList.Count;
   }

   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, List<List<int>> matrix, out int distance, string skip)
   {
      int length = 0;
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else if (ToCompareList[j].Contains(skip))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i][j + 1]) + 1, matrix[i + 1][j]);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            return j;
         }
      }

      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return ToCompareList.Count;
   }

   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, out int distance, out string best, out int coincidence)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
         matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
         matrix[0][i] = i;

      int length = 0;
      distance = 0;
      var v = new List<string>();
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            length = j;
            break;
         }
      }
      if (length == 0)
      {
         distance = matrix[TemplateList.Count][ToCompareList.Count];
         length = ToCompareList.Count;
      }
      else
      {
         for (int i = ToCompareList.Count-1; i >= length; i--)
         {
            v.Add(ToCompareList[i].First());
         }
      }
      
      coincidence = 0;
      for (int i = TemplateList.Count, j = length; i >= 0 && j >= 0; )
      {
         if (i == 0 || j == 0)
            break;

         if (matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
         {
            if (ToCompareList[j - 1].Contains(TemplateList[i - 1]))
            {
               coincidence++;
               v.Add(TemplateList[i - 1]);
               j--;
               i--;
               continue;
            }
         }
         if ((matrix[i - 1][j - 1] < matrix[i][j - 1] && matrix[i - 1][j - 1] < matrix[i - 1][j]))
         {
            j--;
            i--;
            v.Add(ToCompareList[j].First());
         }
         else if (matrix[i - 1][j] <= matrix[i][j - 1])
         {
            i--;
         }
         else
         {
            j--;
            v.Add(ToCompareList[j].First());
         }
      }
      best = "";
      for (int i = v.Count - 1; i >= 0; i--)
      {
         best += " " + v[i];
      }
      return length;
   }

   public static int GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, out int distance, out string best, out int coincidence, string skip, bool fullLength)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
         matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
         matrix[0][i] = i;

      int length = 0;
      distance = 0;
      var v = new List<string>();
      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               if (ToCompareList[j].Contains(skip))
                  matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1] + 1);
               else
                  matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else if (ToCompareList[j].Contains(skip))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i][j + 1]) + 1, matrix[i + 1][j]);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
         if (!fullLength && matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            length =  j;
            break;
         }
      }
      if (length == 0)
      {
         distance = matrix[TemplateList.Count][ToCompareList.Count];
         length = ToCompareList.Count;
      }
      else
      {
         for (int i = ToCompareList.Count - 1; i >= length; i--)
         {
            if (!ToCompareList[i].Contains(skip))
               v.Add(ToCompareList[i].First());
         }
      }
      coincidence = 0;
      for (int i = TemplateList.Count, j = length; i >= 0 && j >= 0; )
      {
         if (i == 0 || j == 0)
            break;
          
         if (matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
         {
            if (ToCompareList[j - 1].Contains(TemplateList[i - 1]))
            {
               coincidence++;
               v.Add(TemplateList[i - 1]);
               j--;
               i--;
               continue;
            }
            else if (matrix[i - 1][j - 1] != matrix[i][j])
            {
               v.Add(ToCompareList[j - 1].First());
               j--;
               i--;
               continue;
            }
         }
         if (matrix[i-1][j] < matrix[i][j-1])
         {
            i--;
         }
         else
         {
            j--;
            if (!ToCompareList[j].Contains(skip))
               v.Add(ToCompareList[j].First());
         }
      }
      best = "";
      for (int i = v.Count-1; i >=0; i--)
      {
         best += " " + v[i];
      }
      //UnloadMatrix(matrix, ToCompareList, TemplateList, "c:\\Users\\stanislav\\main\\tests\\matrix.txt");
      return length;
   }


   public static void GetMatrix(List<List<string>> ToCompareList, List<string> TemplateList, out int coincidence, string skip, List<Edition> Editions)
   {
      List<List<int>> matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < ToCompareList.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<int>(TempList));
      for (int i = 0; i < matrix.Count; i++)
         matrix[i][0] = i;
      for (int i = 0; i < matrix[0].Count; i++)
         matrix[0][i] = i;

      for (int j = 0; j < ToCompareList.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            if (ToCompareList[j].Contains(TemplateList[i]))
               if (ToCompareList[j].Contains(skip))
                  matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1] + 1);
               else
                  matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
            else if (ToCompareList[j].Contains(skip))
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i][j + 1]) + 1, matrix[i + 1][j]);
            else
               matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
         }
      }

      coincidence = 0;
      for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0; )
      {
         if (i == 0 || j == 0)
            break;

         if (matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
         {
            if (ToCompareList[j - 1].Contains(TemplateList[i - 1]))
            {
               Edition Ed = new Edition();
               Ed.Index = i;
               Ed.Type = type.replace;
               Editions.Add(Ed);
               coincidence++;
               j--;
               i--;
               continue;
            }
            else if (matrix[i - 1][j - 1] != matrix[i][j])
            {
               Edition Ed = new Edition();
               Ed.Index = i;
               Ed.Type = type.replace;
               Editions.Add(Ed);
               j--;
               i--;
               continue;
            }
         }
         if (matrix[i - 1][j] < matrix[i][j - 1])
         {
            i--;
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.Type = type.delete;
            Editions.Add(Ed);
         }
         else
         {
            j--;
            Edition Ed = new Edition();
            Ed.Index = i;
            Ed.Type = type.insert;
            Editions.Add(Ed);
         }
      }
   }


   public static int GetMatrix(List<Dictionary<string, double>> GraphToCompare, List<string> TemplateList, Dictionary<string, double> Deletes, out double distance)
   {
      List<List<double>> matrix = new List<List<double>>();
      List<double> TempList = new List<double>();
      for (int i = 0; i < GraphToCompare.Count + 1; i++)
         TempList.Add(0);
      for (int i = 0; i < TemplateList.Count + 1; i++)
         matrix.Add(new List<double>(TempList));
      matrix[0][0] = 0;
      for (int i = 1; i < matrix.Count; i++)
         matrix[i][0] = matrix[i-1][0] + Deletes[TemplateList[i-1]];
      for (int i = 1; i < matrix[0].Count; i++)
         matrix[0][i] = matrix[0][i - 1] + GraphToCompare[i-1]["#"];

      int length = 0;
      for (int j = 0; j < GraphToCompare.Count; j++)
      {
         for (int i = 0; i < TemplateList.Count; i++)
         {
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j] + GraphToCompare[j][TemplateList[i]], matrix[i + 1][j] + GraphToCompare[j]["#"]), matrix[i][j + 1]) + Deletes[TemplateList[i]];
         }
         if (matrix[TemplateList.Count][j + 1] > matrix[TemplateList.Count][j])
         {
            distance = matrix[TemplateList.Count][j];
            return j;
         }
      }
      distance = matrix[TemplateList.Count][GraphToCompare.Count];
      return GraphToCompare.Count;
   }


    public static void GetMatrixCoincidence(List<string> ToCompareList, List<string> TemplateList, out int coincidence, bool weighted)
    {
        coincidence = 0;
        int length = 0;
        List<List<int>> matrix = new List<List<int>>();
        List<int> TempList = new List<int>();
        for (int i = 0; i < ToCompareList.Count + 1; i++)
            TempList.Add(0);
        for (int i = 0; i < TemplateList.Count + 1; i++)
            matrix.Add(new List<int>(TempList));
        for (int i = 0; i < matrix.Count; i++)
            matrix[i][0] = 0;
        for (int i = 0; i < matrix[0].Count; i++)
            matrix[0][i] = 0;

        for (int i = 0; i < TemplateList.Count; i++)
        {
            for (int j = 0; j < ToCompareList.Count; j++)
            {
                if (TemplateList[i] != ToCompareList[j])
                    matrix[i + 1][j + 1] = Math.Max(Math.Max(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]);
                else
                    matrix[i + 1][j + 1] = matrix[i][j] + 1;// Math.Max(Math.Max(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
            }
        }
        coincidence = matrix[TemplateList.Count][ToCompareList.Count]; 

        for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0;)
        {
            if (i == 0 && j == 0)
                break;

            if (j == 0 || (i > 0 && matrix[i - 1][j] == matrix[i][j]))
            {
                i--;
            }
            else if (i == 0 || matrix[i][j - 1] == matrix[i][j])
            {
                j--;
            }
            else
            {
                j--;
                i--;
                if (matrix[i][j] == matrix[i + 1][j + 1] - 1)
                {
                    //coincidence++;
                    length += TemplateList[i].Length;
                }
            }
        }
        if (weighted && coincidence > 0)
            coincidence = length;
            //coincidence += length / 2;
    }

    private static void GetMatrixCoincidence(List<string> ToCompareList, List<string> TemplateList, out int coincidence, out List<int> CoincidenceIndices)
    {
        CoincidenceIndices = new List<int>();
        coincidence = 0;
        List<List<int>> matrix = new List<List<int>>();
        List<int> TempList = new List<int>();
        for (int i = 0; i < ToCompareList.Count + 1; i++)
            TempList.Add(0);
        for (int i = 0; i < TemplateList.Count + 1; i++)
            matrix.Add(new List<int>(TempList));
        for (int i = 0; i < matrix.Count; i++)
            matrix[i][0] = i;
        for (int i = 0; i < matrix[0].Count; i++)
            matrix[0][i] = i;

        for (int i = 0; i < TemplateList.Count; i++)
        {
            for (int j = 0; j < ToCompareList.Count; j++)
            {
                if (TemplateList[i] == ToCompareList[j])
                    matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
                else
                    matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
            }
        }
        for (int i = TemplateList.Count, j = ToCompareList.Count; i >= 0 && j >= 0; )
        {
            if (i == 0 && j == 0)
                break;
            if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]))
            {
                i--;
            }
            else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
            {
                j--;
                i--;
                if (matrix[i][j] == matrix[i + 1][j + 1])
                {
                    coincidence++;
                    CoincidenceIndices.Add(i);
                }
            }
            else
            {
                j--;
            }
        }
    }

    private static void GetMatrix(List<string> ToCompareList, List<string> TemplateList, int delimiter, out int distance, List<List<int>> matrix)
    {
      for (int i = 0; i < TemplateList.Count; i++)
      {
        int j = i - delimiter;
        if (j < 0)
          j = 0;
        int top = Math.Min(i + delimiter, ToCompareList.Count);
        for (; j < top; j++)
        {
          if (TemplateList[i] == ToCompareList[j])
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j] + 1), matrix[i][j + 1] + 1);
          else
            matrix[i + 1][j + 1] = Math.Min(Math.Min(matrix[i][j], matrix[i + 1][j]), matrix[i][j + 1]) + 1;
        }
      }
      distance = matrix[TemplateList.Count][ToCompareList.Count];
      return;
    }

    public static List<List<int>> MakeRawMatrix(int first, int second)
    {
      var Matrix = new List<List<int>>();
      List<int> TempList = new List<int>();
      for (int i = 0; i < first + 1; i++)
        TempList.Add(0);
      for (int i = 0; i < second + 1; i++)
        Matrix.Add(new List<int>(TempList));
      for (int i = 0; i < Matrix.Count; i++)
        for (int j = 0; j < Matrix[i].Count; j++)
          Matrix[i][j] = Math.Abs(i - j);
      return Matrix;
    }


    private static void UnloadMatrix(List<List<int>> matrix, List<string> ToCompareList, List<string> TemplateList, string file)
    {
       using (var sw = new StreamWriter(file, false, Encoding.UTF8))
       {
          var s = "\t";
          foreach (var v in ToCompareList)
             s += "\t" + v;
          sw.WriteLine(s);
          s = "";
          for (int i = 0; i < ToCompareList.Count+1; i++)
             s += "\t" + i;
          sw.WriteLine(s);
          for (int i = 0; i < TemplateList.Count; i++)
          {
             s = TemplateList[i];
             foreach (var e in matrix[i + 1])
                s += "\t" + e;
             sw.WriteLine(s);
          }
       }
    }
    private static void UnloadMatrix(List<List<int>> matrix, List<List<string>> ToCompareList, List<string> TemplateList, string file)
    {
       using (var sw = new StreamWriter(file, false, Encoding.UTF8))
       {
          var s = "\t";
          foreach (var v in ToCompareList)
             s += "\t" + string.Join(" ", v);
          sw.WriteLine(s);
          s = "";
          for (int i = 0; i < ToCompareList.Count + 1; i++)
             s += "\t" + i;
          sw.WriteLine(s);
          for (int i = 0; i < TemplateList.Count; i++)
          {
             s = TemplateList[i];
             foreach (var e in matrix[i + 1])
                s += "\t" + e;
             sw.WriteLine(s);
          }
       }
    }
  }
}
