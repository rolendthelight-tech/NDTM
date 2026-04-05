using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolbox.Text
{
  public class TextItem
  {
    public static char[] WordDelimiters = new[] {'.', '?', ',', ';', ' ', '\r', '\n'};

    public static string[] punctuationMarks =  { ".", ",", "?", "!" };

    protected String _text;

    public int Position { get; set; }
    
    public String Text
    {
      get
      {
        if (String.IsNullOrEmpty(_text))
          _text = String.Empty;
        return _text;
      }
      set { _text = value; }
    }

    public override string ToString()
    {
      return Position + ": " + Text;
    }

    /// <summary>
    ///   Поиск первого символа - не разделителя
    /// </summary>
    /// <param name="text"></param>
    /// <param name="indexStart"></param>
    /// <param name="delimiters"></param>
    /// <returns></returns>
    public static int FindNextChar(string text, int indexStart, char[] delimiters)
    {
      for (int i = indexStart; i < text.Length; i++)
      {
        char c = text[i];

        bool finded = false;
        for (int j = 0; j < delimiters.Length; j++)
        {
          if ((c == delimiters[j]))
          {
            //nonchar = false;
            finded = true;
            break;
          }
        }
        if (finded)
          continue;
        else return i;
      }

      return -1;
    }

    /// <summary>
    ///   Поиск разделителя
    /// </summary>
    /// <param name="text"></param>
    /// <param name="indexStart"></param>
    /// <param name="delimiters"></param>
    /// <returns></returns>
    public static int FindNextDelimiter(string text, int indexStart, char[] delimiters)
    {
      for (int i = indexStart; i < text.Length; i++)
      {
        char c = text[i];
        for (int j = 0; j < delimiters.Length; j++)
        {
          if ((c == delimiters[j]))
          {
            return i;
          }
        }
      }

      return -1;
    }

    public static TextItem[] GetWordList(string text, char[] delimiters, List<string> excludedWord = null)
    {
      if (excludedWord == null)
      {
      }
      var list = new List<TextItem>();

      int i = 0;
      while (i < text.Length)
      {
        int startindex = i;

        int endIndex;
        startindex = FindNextChar(text, startindex, delimiters);
        if (startindex >= 0)
        {
          endIndex = FindNextDelimiter(text, startindex + 1, delimiters);

          if (endIndex == -1)
            endIndex = text.Length;

          string word = text.Substring(startindex, endIndex - startindex);

          bool add = true;
          if (excludedWord != null)
          {
            if (excludedWord.Contains(word))
              add = false;
          }
          if (add)
            list.Add(new TextItem {Position = startindex, Text = word, Length = word.Length});

          i = endIndex;
        }
        else
        {
          break;
        }
      }
      return list.ToArray();
    }

    //public static TextItem[] GetWordListAsterisk(string text, char[] delimiters, List<string> excludedWord = null)
    //{
    //  if (excludedWord == null)
    //  {
    //  }
    //  List<TextItem> list = new List<TextItem>();
    //  int i = 0;
    //  while (i < text.Length)
    //  {
    //    int startindex = i;
    //    int endIndex;
    //    int asteriskIndex = text.IndexOf('*', startindex);
    //    startindex = FindNextChar(text, startindex, delimiters);
    //    if (startindex >= 0)
    //    {
    //      if (asteriskIndex >=0 && asteriskIndex<=startindex)
    //      {
    //        //пропускаем до скобки
    //        int braceIndex = text.IndexOf('(', asteriskIndex+1);
    //        int colonIndex = text.IndexOf(':', asteriskIndex + 1);
    //        if (braceIndex>=0)
    //        {
    //          startindex = braceIndex + 1;
    //          endIndex = text.IndexOf(')', braceIndex + 1);
    //          if (endIndex<0)
    //          {
    //            endIndex = FindNextDelimiter(text, braceIndex + 1, delimiters);  
    //          }
    //        }
    //        else
    //        {              
    //          //endIndex = text.IndexOf(':', asteriskIndex + 1);
    //          endIndex = FindNextChar(text, asteriskIndex + 1, delimiters);
    //          if (endIndex>=0)
    //            startindex = endIndex; //обнуляем слово
    //        }
    //      }
    //      else
    //      {
    //        endIndex = FindNextDelimiter(text, startindex + 1, delimiters);  
    //      }
    //      if (endIndex == -1)
    //        endIndex = text.Length;
    //      string word = text.Substring(startindex, endIndex - startindex);
    //      bool add = true;
    //      if (excludedWord != null)
    //      {
    //        if (excludedWord.Contains(word))
    //          add = false;
    //      }
    //      if (word.Length == 0 || word.Equals(")") || word.Equals("**") || word.Equals(":"))
    //        add = false;
    //      if (add)
    //      {
    //        var processed = word.Replace(":", "");
    //        if (processed.Length!= word.Length)
    //        {
    //          Trace.WriteLine("");
    //        }
    //        list.Add(new TextItem { Position = startindex, Text = processed, Length = word.Length });
    //      }
    //      i = endIndex;
    //      if (endIndex < text.Length-1 && text[endIndex] == ')')
    //        i++;
    //    }
    //    else
    //    {
    //      break;
    //    }
    //  }
    //  return list.ToArray();
    //}
  
    ///<summary>
    ///   Выдает слова с учетом слов со звездой или символом процентов.
    ///   После слова со звездой(процентом) должно быть в скобках правильно произнесенное слово (для процента - написание с цифрой)
    /// </summary>
    /// <param name="text"></param>
    /// <param name="delimiters"></param>
    /// <param name="excludedWord"></param>
    /// <returns></returns>
    public static TextItem[] GetWordListAsteriskAndPercent(string text, char[] delimiters,
                                                           List<string> excludedWord = null)
    {
      TextItem[] words = GetWordList(text, delimiters, excludedWord);

      var list = new List<TextItem>();

      for (int j = 0; j < words.Length; j++)
      {
        TextItem word = words[j];

        if (word.Text.StartsWith("*") || word.Text.StartsWith("%") || word.Text.StartsWith("~") || word.Text.EndsWith("~"))
        {
          int braceWrongIndex = word.Text.IndexOf(")", StringComparison.Ordinal);
          
          int braceIndex = word.Text.IndexOf("(", StringComparison.Ordinal);

          if (braceIndex >= 0)
          {
            if (braceWrongIndex >= 0 && braceWrongIndex <braceIndex)
            {
              list.Add(new TextItem
              {
                Position = word.Position + 1,

                FullTextPosition = word.Position,
                FullText = word.Text,
                Text = word.Text.Substring(1, braceWrongIndex - 1),
                Length = braceWrongIndex
              });
            }


            int endBraceIndex = word.Text.IndexOf(")", braceIndex + 1, StringComparison.Ordinal);

            if (endBraceIndex == word.Text.Length - 1)
            {
              String processed;
              int position;
              if (word.Text.StartsWith("%"))
              {
                processed = word.Text.Substring(1, braceIndex - 1);
                position = word.Position + 1;
              }
              else
              {
                processed = word.Text.Substring(braceIndex + 1, endBraceIndex - braceIndex - 1);
                position = word.Position + braceIndex + 1;
              }

              //if (processed.Equals("**"))
              //  continue;

              list.Add(new TextItem
                {
                  Position = position,
                  Text = processed,
                  FullTextPosition = word.Position,
                  FullText = word.Text,
                  Length = processed.Length
                });
            }
            else //не встретили закрывающую скобку или после нее есть текст
            {
              int position;
              String processed;
              if (word.Text.StartsWith("%"))
              {
                processed = word.Text.Substring(1, braceIndex - 1);
                position = word.Position + 1;
              }
              else
              {
                if (word.Text.EndsWith("~"))
                {
                  processed = word.Text.Substring(braceIndex + 1, word.Text.Length-braceIndex-3);
                  position = word.Position + braceIndex + 1;
                }
                else
                {
                  processed = word.Text.Substring(braceIndex + 1);
                  position = word.Position + braceIndex + 1;  
                }
                
              }


              list.Add(new TextItem
                {
                  Position = position,
                  Text = processed,
                  FullTextPosition = word.Position,
                  FullText = word.Text,
                  Length = processed.Length
                });
            }
          }
          else
          {
            string processed = word.Text.Substring(1).Replace(":", "");
            if (processed.Length != word.Text.Length)
            {
              Trace.WriteLine("");
            }
            int position = word.Position; 
            int len;

            len = word.Length;
            int index = 0;
            if (word.Text.EndsWith("~"))
            {                            
              len -= 1;
            }

            if (word.Text.StartsWith("*") || word.Text.StartsWith("%") || word.Text.StartsWith("~") )
            {
              index++;
              position++;
              len  -= 1;              
            }

            if (len >= 0)
            {
              processed = word.Text.Substring(index, len).Replace(":", "");  
            }
            
            list.Add(new TextItem
              {
                Position = position,
                FullTextPosition = word.Position,
                FullText = word.Text,
                Text = processed,
                Length = len
              });
          }
        }
        else
        {
          list.Add(new TextItem
            {
              Position = word.Position,
              FullTextPosition = word.Position,
              Text = word.Text,
              FullText = word.Text,
              Length = word.Length
            });
        }
      }
      return list.ToArray();
    }

    /// <summary> Начало полного слова </summary>
    public int FullTextPosition { get; protected set; }

    /// <summary> Полный текст слова </summary>
    public string FullText { get; set; }

    public static List<TextItem> GetWordsForSegments(String[] list, List<string> markers, char[] wordDelimiters,
                                                     out int[] indices)
    {
      var items = new List<TextItem>();
      var offsets = new int[list.Length];

      indices = new int[list.Length + 1]; // индекс конца транскрипции в общем списке слов
      indices[list.Length] = Int32.MaxValue;

      int offset = 0;
      for (int i = 0; i < list.Length; i++)
      {
        string transcription = list[i];
        TextItem[] words = GetWordListAsteriskAndPercent(transcription, wordDelimiters, markers);
        items.AddRange(words);

        foreach (TextItem textItem in words)
        {
          textItem.IndexInList = i;
        }

        offsets[i] = offset + transcription.Length;

        if (i == 0)
        {
          indices[0] = words.Length;
        }
        else
        {
          indices[i] = indices[i - 1] + words.Length;
        }
      }

      return items;
    }

    public int IndexInList { get; set; }

    public int Length { get; set; }

    public int FullLength
    {
      get { return FullText.Length; }
    }


    public static List<string> SplitWordsWithPunctuation(TextItem[] words)
    {      
      List<string> sourceTexts = new List<string>();

      StringBuilder sb = new StringBuilder();

      foreach (var word in words)
      {
        if (punctuationMarks.Contains(word.FullText))
        {
          sourceTexts.Add(String.Format(sb.ToString() + " " + word.FullText));
          sb.Clear();
        }
        else
        {
          if (sb.Length > 0)
          {
            sourceTexts.Add(sb.ToString());
            sb.Clear();
          }
          //else
          //{
          sb.Append(word.FullText);
          //}                
        }
      }

      if (sb.Length > 0)
      {
        sourceTexts.Add(sb.ToString());
      }
      return sourceTexts;
    }

    public static Dictionary<string, string> ReadDictionary(string path, bool inverseColumns)
    {
      var result = new Dictionary<string, string>();
      var lines = File.ReadAllLines(path);

      foreach (var line in lines)
      {
        var items = line.Split('\t', ' ');
        if (items.Length != 2)
          continue;

        if (inverseColumns)
        {
          result[items[1]] = items[0];
        }
        else
        {
          result[items[0]] = items[1];  
        }
        
      }
      return result;
    }

  }
}
