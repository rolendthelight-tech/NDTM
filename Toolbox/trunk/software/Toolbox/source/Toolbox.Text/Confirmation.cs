using System;
using System.Collections.Generic;

namespace Toolbox.Text
{
   public class Confirmation
   {
      private string refID;
      private string templateSubstring;
      private string confirmedSubstring;
      private int start;
      private int end;
      private int startRef;
      private int endRef;
      private List<Levenstein3.Edition> editions;
      private double wer;

      public string RefID { get { return refID; } }
      public string TemplateSubstring { get { return templateSubstring; } }
      public string ConfirmedSubstring { get { return confirmedSubstring; } }
      public int Start { get { return start; } }
      public int End { get { return end; } }
      public int StartRef { get { return startRef; } }
      public int EndRef { get { return endRef; } }
      public double WER { get { return wer; } }
      public List<Levenstein3.Edition> Editions { get { return editions; } }

      public Confirmation(string refID, string templateSubstring, string confirmedSubstring, int start, int end, int startRef, int endRef, List<Levenstein3.Edition> editions, double wer)
      {
         this.refID = refID;
         this.templateSubstring = templateSubstring;
         this.confirmedSubstring = confirmedSubstring;
         this.editions = editions;
         this.start = start;
         this.end = end;
         this.startRef = startRef;
         this.endRef = endRef;
         this.wer = wer;
      }

      public Confirmation(string refID, List<Levenstein3.Edition> editions, double wer)
      {
         this.refID = refID;
         this.editions = editions;
         this.wer = wer;
      }

      public override string ToString()
      {
        return String.Format("{0} {1} ", confirmedSubstring, templateSubstring);
      }
   }
}
