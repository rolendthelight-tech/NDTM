using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Toolbox.Text
{
   public class Aligner
   {
      static readonly log4net.ILog log = log4net.LogManager.GetLogger("Aligner");
      static readonly List<string> paulers = new List<string> { ",", ".", "!", "?", ";", ":", "\"", "\'", "\n", "\r" };

      public static List<string> Paulers { get { return paulers; } }

      Dictionary<string, List<string>> templDict = new Dictionary<string, List<string>>();
      Dictionary<string, string> templTexts = new Dictionary<string, string>();
      Dictionary<string, List<string>> templNgramsInDocs = new Dictionary<string, List<string>>();
      Dictionary<string, List<int[]>> indicesInRef = new Dictionary<string, List<int[]>>();
      Dictionary<string, List<int>> DotIndices = new Dictionary<string, List<int>>();
      private readonly int _gramms;
      private readonly double _maxWER;
      private readonly double _minCoverage;


      public Aligner(Dictionary<string, string> refTexts, double maxWER, double minCoverage, int gramms = 4)
      {
         if (File.Exists("config.log4net") || File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.log4net"))
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "config.log4net"));
         else Console.WriteLine("No log configuration for Aligner");
         _maxWER = maxWER;
         _minCoverage = minCoverage;
         _gramms = gramms;
         templTexts = refTexts;
         ParseRefToNgrams(refTexts);
         log.Info("Reference texts loaded");
      }

      public Aligner(Dictionary<string, List<string>> refTexts, double maxWER, double minCoverage, int gramms = 4)
      {
         if (File.Exists("config.log4net") || File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.log4net"))
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "config.log4net"));
         else Console.WriteLine("No log configuration for Aligner");
         _maxWER = maxWER;
         _minCoverage = minCoverage;
         _gramms = gramms;
         ParseRefToNgrams(refTexts);
         log.Info("Reference texts loaded");
      }


      private void ParseRefToNgrams(Dictionary<string, string> refTexts)
      {
         foreach (var refText in refTexts)
         {
            var t = refText.Value;
            indicesInRef.Add(refText.Key, GetIndexList(t));
            foreach (var pauler in paulers)
               t = t.Replace(pauler, "");
            t = t.Replace("ё", "е");
            t = t.ToLower();
            templDict.Add(refText.Key, t.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
         }
         foreach (var templ in templDict)
         {
            var ngrams = new List<string>();
            for (int i = 0; i < templ.Value.Count - _gramms + 1; i++)
            {
               var s = string.Join(" ", templ.Value.GetRange(i, _gramms));
               ngrams.Add(s);
            }
            templNgramsInDocs.Add(templ.Key, ngrams);
         }
      }

      private void ParseRefToNgrams(Dictionary<string, List<string>> refTexts)
      {
         foreach (var refText in refTexts)
         {
            var words = new List<string>();
            var indices = new List<int[]>();
            var dotIndices = new List<int>();
            int count = 0;
            int shift = 0;
            foreach (var word in refText.Value)
            {
               if (word == ".")
               {
                  dotIndices.Add(count+shift);
                  shift++;
                  continue;
               }
               var word1 = word.Replace("ё", "е").ToLower();
               words.Add(word1);
               indices.Add(new int[]{count+shift});
               count++;
            }
            indicesInRef.Add(refText.Key, indices);
            templDict.Add(refText.Key, words);
            DotIndices.Add(refText.Key, dotIndices);
         }
         foreach (var templ in templDict)
         {
            var ngrams = new List<string>();
            for (int i = 0; i < templ.Value.Count - _gramms + 1; i++)
            {
               var s = string.Join(" ", templ.Value.GetRange(i, _gramms));
               ngrams.Add(s);
            }
            templNgramsInDocs.Add(templ.Key, ngrams);
         }
      }


      public List<Confirmation> Align(string inText)
      {
         var result = new List<Confirmation>();
         var inVect = new List<string>();
         var str = inText;

         var absIndices = GetIndexList(str);
         foreach (var pauler in paulers)
            str = str.Replace(pauler, "");
         str = str.Replace("ё", "е");
         str = str.ToLower();
         inVect = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
         
         var inNgrams = new Dictionary<string, List<int>>();
         for (int i = 0; i < inVect.Count - _gramms + 1; i++)
         {
            var s = string.Join(" ", inVect.GetRange(i, _gramms));
            if (inNgrams.ContainsKey(s))
               inNgrams[s].Add(i);
            else
               inNgrams.Add(s, new List<int> { i });
         }
         foreach(var templNgrams in templNgramsInDocs)
         {
            var foundNgrams = new Dictionary<string, List<int>>();
            foreach (var templNgram in templNgrams.Value)
            {
               if (inNgrams.ContainsKey(templNgram))
               {
                  if (foundNgrams.ContainsKey(templNgram))
                     foundNgrams[templNgram].AddRange(inNgrams[templNgram]);
                  else
                     foundNgrams.Add(templNgram, inNgrams[templNgram]);
               }
            }
            int division = templDict[templNgrams.Key].Count;
            var locIndices = new List<int>();
            if ((double)foundNgrams.Count / (double)inNgrams.Count < 0.1)
               continue;
            if (division <= 0)
            {
              continue;
            }
            for (int k = 0; k < inVect.Count / division + 1; k++)
               locIndices.Add(0);
            foreach (var foundNgram in foundNgrams)
               foreach (var ind in foundNgram.Value.Distinct())
                  locIndices[ind/division]++;
            var max = locIndices.Max();
            var index = locIndices.IndexOf(max);
            int start = index*division - division/2;
            int end = start + division*2;
            for (int k = 0; k < foundNgrams.Count; k++)
               foundNgrams.ElementAt(k).Value.RemoveAll(a => a < start || a > end);
            var empties = new List<KeyValuePair<string,List<int>>>(foundNgrams.Where(a => a.Value.Count == 0));
            foreach (var empty in empties)
               foundNgrams.Remove(empty.Key);
            if (foundNgrams.Count == 0)
               continue;
            int first = foundNgrams.Values.Min(a=>a.Min());
            int last = foundNgrams.Values.Max(a => a.Max());
            
            var editions = new List<Levenstein3.Edition>();

            var subVect = inVect.GetRange(first, last + _gramms - first);
            /*int distance;
            Levenstein3.Levenstein3.FindLevensteinDistance(templDict[templNgrams.Key], subVect, out distance, out editions);            
            if ((double)distance / templDict[templNgrams.Key].Count < minCoverage)
               continue;*/
            int coince;
            Levenstein3.FindMaxSubstring(templDict[templNgrams.Key], subVect, out coince, out editions);
            if ((double)coince / inVect.Count < _minCoverage)
               continue;
            result.AddRange(MakeConfirmation(subVect, templNgrams.Key, editions, absIndices, inText, first));
         }
         return result;
      }


      public List<Confirmation> Align(List<string> inText)
      {
         var result = new List<Confirmation>();
         var inVect = new List<string>();
         var str = string.Join(" ", inText);

         //var absIndices = GetIndexList(str);
         foreach (var pauler in paulers)
            str = str.Replace(pauler, "");
         str = str.Replace("ё", "е");
         str = str.ToLower();
         inVect = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

         var inNgrams = new Dictionary<string, List<int>>();
         for (int i = 0; i < inVect.Count - _gramms + 1; i++)
         {
            var s = string.Join(" ", inVect.GetRange(i, _gramms));
            if (inNgrams.ContainsKey(s))
               inNgrams[s].Add(i);
            else
               inNgrams.Add(s, new List<int> { i });
         }
         foreach (var templNgrams in templNgramsInDocs)
         {
            var foundNgrams = new Dictionary<string, List<int>>();
            foreach (var templNgram in templNgrams.Value)
            {
               if (inNgrams.ContainsKey(templNgram))
               {
                  if (foundNgrams.ContainsKey(templNgram))
                     foundNgrams[templNgram].AddRange(inNgrams[templNgram]);
                  else
                     foundNgrams.Add(templNgram, inNgrams[templNgram]);
               }
            }
            int division = templDict[templNgrams.Key].Count;
            var locIndices = new List<int>();
            if ((double)foundNgrams.Count / (double)inNgrams.Count < 0.1)
               continue;
            if (division <= 0)
            {
               continue;
            }
            for (int k = 0; k < inVect.Count / division + 1; k++)
               locIndices.Add(0);
            foreach (var foundNgram in foundNgrams)
               foreach (var ind in foundNgram.Value.Distinct())
                  locIndices[ind / division]++;
            var max = locIndices.Max();
            var index = locIndices.IndexOf(max);
            int start = index * division - division / 2;
            int end = start + division * 2;
            for (int k = 0; k < foundNgrams.Count; k++)
               foundNgrams.ElementAt(k).Value.RemoveAll(a => a < start || a > end);
            var empties = new List<KeyValuePair<string, List<int>>>(foundNgrams.Where(a => a.Value.Count == 0));
            foreach (var empty in empties)
               foundNgrams.Remove(empty.Key);
            if (foundNgrams.Count == 0)
               continue;
            int first = foundNgrams.Values.Min(a => a.Min());
            int last = foundNgrams.Values.Max(a => a.Max());

            var editions = new List<Levenstein3.Edition>();

            var subVect = inVect.GetRange(first, last + _gramms - first);
            int coince;
            Levenstein3.FindMaxSubstring(templDict[templNgrams.Key], subVect, out coince, out editions);
            if ((double)coince / inVect.Count < _minCoverage)
               continue;
            result.AddRange(MakeConfirmation(templNgrams.Key, editions, first));
         }
         return result;
      }

      private List<int[]> GetIndexList(string str)
      {
         var absIndices = new List<int[]>();
         bool started = false;
         int startPos = 0;
         for (int i = 0; i < str.Length; i++)
         {
            if (str[i] == ' ' || paulers.Contains(str[i].ToString()))
            {
               if (started)
               {
                  absIndices.Add(new int[] { startPos, i - 1 });
                  started = false;
               }
            }
            else if (!started)
            {
               started = true;
               startPos = i;
            }
         }
         if (started)
            absIndices.Add(new int[] { startPos, str.Length - 1 });
         return absIndices;
      }


      private List<Confirmation> MakeConfirmation(List<string> subVect, string templID, List<Levenstein3.Edition> editions, List<int[]> absIndices, string inText, int first)
      {
         List<Confirmation> confirmations = new List<Confirmation>();
         for (int i = 0; i < editions.Count; i++)
         {
            int coince = 1;
            double bestWer = _maxWER + 1;
            int bestIndex = i;
            double errors = 0;
            for (int j = i+1; j < editions.Count && j < i + 30; j++)
            {
               errors += Math.Max(editions[j].Index - editions[j - 1].Index, editions[j].Position - editions[j - 1].Position) - 1;
               coince++;
               double wer = errors / (coince + errors);
               if (wer <= _maxWER)
               {
                  bestWer = wer;
                  bestIndex = j;
               }
            }
            if (bestWer <= _maxWER &&  ((editions[i].Position + first) >=0))
            {
               int startRef = indicesInRef[templID][editions[i].Index][0];
               int endRef = indicesInRef[templID][editions[bestIndex].Index][1];
               string templateSubstring = templTexts[templID].Substring(startRef, endRef - startRef + 1);
               int start = absIndices[editions[i].Position + first][0];
               int end = absIndices[editions[bestIndex].Position + first][1];
               string confirmedSubstring = inText.Substring(start, end - start + 1);
               confirmations.Add(new Confirmation(templID, templateSubstring, confirmedSubstring, start, end, startRef,
                                                  endRef, new List<Levenstein3.Edition>(), bestWer));
               i = bestIndex;
            }
         }
         return confirmations;
      }


      private List<Confirmation> MakeConfirmation(string templID, List<Levenstein3.Edition> editions, int first)
      {
         List<Confirmation> confirmations = new List<Confirmation>();
         for (int i = 0; i < editions.Count; i++)
         {
            if (editions[i].Type == Levenstein3.type.insert)
               continue;
            int coince = 1;
            double bestWer = _maxWER + 1;
            int bestIndex = i;
            double errors = 0;
            for (int j = i + 1; j < editions.Count && j < i + 30; j++)
            {
               if (editions[i].Type == Levenstein3.type.insert)
                  continue;
               errors += Math.Max(editions[j].Index - editions[j - 1].Index, editions[j].Position - editions[j - 1].Position) - 1;
               coince++;
               double wer = errors / (coince + errors);
               if (wer <= _maxWER)
               {
                  bestWer = wer;
                  bestIndex = j;
               }
            }
            if (bestWer <= _maxWER)
            {
               var eds = new List<Levenstein3.Edition>();
               for (int k = i; k <= bestIndex; k++)
               {
                  int ind = indicesInRef[templID][editions[k].Index][0];
                  int pos = editions[k].Position + first;
                  eds.Add(new Levenstein3.Edition(ind, pos, editions[k].Type));
                  if (DotIndices[templID].Contains(ind+1))
                     eds.Add(new Levenstein3.Edition(ind+1, pos, Levenstein3.type.insert));
               }
               confirmations.Add(new Confirmation(templID, eds, bestWer));
               i = bestIndex;
            }
         }
         return confirmations;
      }

      /*private List<Confirmation> MakeConfirmation(List<string> subVect, string templID, List<Levenstein3.Levenstein3.Edition> editions, List<int[]> absIndices, int first)
      {
         List<Confirmation> confirmations = new List<Confirmation>();
         var startConf = 0;
         var endConf = 0;
         var startTempl = 0;
         var endTempl = 0;
         int error = 0;
         int words = 0;
         int prevError = -1;
         int errorsConseq = 0;
         for (int i = editions.Count - 1; i >= 0; i--)
         {
            error++;
            int dif = editions[i].Index - prevError - 1;
            if (dif > 0)
            {
               endTempl = editions[i].Index - 1;
               endConf = editions[i].Position - 1;
               if (editions[i].type == Levenstein3.Levenstein3.type.insert)
                  endTempl++;
            }
            else
               errorsConseq++;
            words += dif + 1;
            double wer = error/(double) words;
            if ( wer > maxWER || errorsConseq > 3)
            {
               if (endConf - startConf > 2)
               {
                  var templ = string.Join(" ", templDict[templID].GetRange(startTempl, endTempl - startTempl + 1));
                  var conf = string.Join(" ", subVect.GetRange(startConf, endConf - startConf + 1));
                  var eds = editions.Where(a => a.Index > startTempl && a.Index < endTempl).ToList();
                  for (int k = 0; k < eds.Count; k++)
                  {
                     eds[k].Index += startTempl;
                     eds[k].Position += startConf;
                  }
                  confirmations.Add(new Confirmation(templID, templ, conf, absIndices[startConf+first][0], absIndices[endConf+first][1], eds, wer));
               }
               startTempl = editions[i].Index + 1;
               endTempl = editions[i].Index + 1;
               startConf = editions[i].Position + 1;
               endConf = editions[i].Position + 1;
               error = 0;
               words = 0;
            }
            prevError = editions[i].Index;
         }
         return confirmations;
      }*/
   }
}
