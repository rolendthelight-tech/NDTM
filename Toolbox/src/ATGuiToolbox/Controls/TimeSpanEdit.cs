using System;
using DevExpress.XtraEditors;

namespace AT.Toolbox.Controls
{
  public partial class TimeSpanEdit : XtraUserControl
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(TimeSpanEdit).Name);

    public TimeSpanEdit()
    {
      InitializeComponent();
    }

    public TimeSpan EditValue
    {
      get { return SpanFromStrings(spinEdit1.EditValue.ToString(), comboBoxEdit1.EditValue.ToString()); }
      set
      {
        string s1;
        string s2;

        StringsToSpan(value, out s1, out s2);

        spinEdit1.EditValue = s1;
        comboBoxEdit1.EditValue = s2;
      }
    }

    private TimeSpan SpanFromStrings(string s, string s1)
    {
      int i = Int32.Parse(s);

      switch (s1)
      {
        case "Ms":
          return TimeSpan.FromMilliseconds(i);
        case "Sec":
          return TimeSpan.FromSeconds(i);
        case "Min":
          return TimeSpan.FromMinutes(i);
        case "Hr":
          return TimeSpan.FromHours(i);
        case "Day":
          return TimeSpan.FromDays(i);
        default:
          return new TimeSpan(0);
      }
    }

    private void StringsToSpan(TimeSpan s, out string s1, out string s2)
    {
      try
      {
        if (Math.Truncate(s.TotalDays) == s.TotalDays)
        {
          s1 = s.Days.ToString();
          s2 = "Day";
          return;
        }

        if (Math.Truncate(s.TotalHours) == s.TotalHours)
        {
          s1 = s.Hours.ToString();
          s2 = "Hr";
          return;
        }

        if (Math.Truncate(s.TotalMinutes) == s.TotalMinutes)
        {
          s1 = s.Minutes.ToString();
          s2 = "Min";
          return;
        }

        if (Math.Truncate(s.TotalSeconds) == s.TotalSeconds)
        {
          s1 = s.Seconds.ToString();
          s2 = "Sec";
          return;
        }

        s1 = s.Milliseconds.ToString();
        s2 = "Ms";
        return;
      }
      catch (Exception ex)
      {
        Log.Error(string.Format("StringsToSpan({0}): exception", s), ex);
        s1 = "0";
        s2 = "Sec";
        return;
      }
    }
  }
}