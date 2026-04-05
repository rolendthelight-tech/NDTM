using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Text
{
    public class Levenstein2
    {
      public static void FindLevensteinDistance(List<string> refList, List<string> testList, out int distance, out List<Edition> editions)
      {
        editions = new List<Edition>();
        List<List<int>> matrix = GetMatrix(testList, refList, out distance);

        for (int i = refList.Count, j = testList.Count; i >= 0 && j >= 0; )
        {
          if (i == 0 && j == 0)
            break;

          var Ed = new Edition();
          editions.Add(Ed);

          if (j == 0 || (i > 0 && matrix[i - 1][j] < matrix[i][j]))
          {
            i--;
            Ed.Index = i;
            Ed.EditionType = EditionType.Delete;
            Ed.From = refList[i];
            Ed.FromIndex = i;
          }
          else if (i > 0 && j > 0 && matrix[i - 1][j - 1] <= matrix[i][j - 1] && matrix[i - 1][j - 1] <= matrix[i - 1][j])
          {
            j--;
            i--;
            Ed.Index = i;
            Ed.To = testList[j];
            Ed.ToIndex = j;

            Ed.From = refList[i];
            Ed.FromIndex = i;
            if (matrix[i][j] != matrix[i + 1][j + 1])
            {
              Ed.EditionType = EditionType.Replace;
            }
            else
            {
              Ed.EditionType = EditionType.Coincidence;
            }
          }
          else
          {
            j--;
            Ed.To = testList[j];
            Ed.ToIndex = j;
            Ed.FromIndex = (i == 0) ? i : (i - 1);
            Ed.Index = i - 1;
            Ed.EditionType = EditionType.Insert;
          }
        }
      }

      private static List<List<int>> GetMatrix(List<string> testList, List<string> refList, out int distance)
      {
        List<List<int>> matrix = new List<List<int>>();
        List<int> TempList = new List<int>();
        for (int i = 0; i < testList.Count + 1; i++)
          TempList.Add(0);
        for (int i = 0; i < refList.Count + 1; i++)
          matrix.Add(new List<int>(TempList));
        for (int i = 0; i < matrix.Count; i++)
          matrix[i][0] = i;
        for (int i = 0; i < matrix[0].Count; i++)
          matrix[0][i] = i;
        int e = 0;

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

      ///// <summary> Расчет редакционной строки </summary>    
      //public static string CalculateEditString(List<Edition> editions, bool insertNewLine)
      //{
      //  StringBuilder sb = new StringBuilder();

      //  foreach (var edition in editions.OrderBy(z => z.Index))
      //  {
      //    int i = 0, j = 0;

      //    switch (edition.EditionType)
      //    {
      //      case EditionType.Replace:
      //        sb.Append(String.Format(" {0}=>{1}", edition.From, edition.To));

      //        if (insertNewLine)
      //          sb.Append(Environment.NewLine);
      //        //s.Replaced++;
      //        //j = alphabet.GetIndex(transfer[edition.To]);
      //        //i = alphabet.GetIndex(transfer[edition.From]);


      //        break;
      //      case EditionType.Coincidence:
      //        //s.Corrected++;
      //        //j = alphabet.GetIndex(transfer[edition.To]);
      //        //i = alphabet.GetIndex(transfer[edition.From]);
      //        sb.AppendFormat(" {0}", edition.From);
      //        if (insertNewLine)
      //          sb.Append(Environment.NewLine);
      //        break;
      //      case EditionType.Delete:
      //        sb.AppendFormat(" {0}=>[]", edition.From);
      //        if (insertNewLine)
      //          sb.Append(Environment.NewLine);
      //        //s.Deleted++;
      //        //j = alphabet.Count;
      //        //i = alphabet.GetIndex(transfer[edition.From]);
      //        break;
      //      case EditionType.Insert:
      //        sb.AppendFormat(" []=>[{0}]", edition.To);
      //        if (insertNewLine)
      //          sb.Append(Environment.NewLine);
      //        //s.Inserted++;
      //        //j = alphabet.Count + 1;
      //        //i = alphabet.GetIndex(transfer[edition.To]);
      //        break;
      //    }
      //  }
      //  var result = sb.ToString();
      //  return result;
      //}
    }
}
