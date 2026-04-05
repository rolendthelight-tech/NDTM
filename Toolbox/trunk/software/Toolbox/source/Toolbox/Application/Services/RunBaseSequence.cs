using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Toolbox.Extensions;

namespace Toolbox.Application.Services
{
  [PercentNotification]
  public class RunBaseSequence : CancelableRunBase
  {
    private readonly IRunBase[] m_run_bases;
    private readonly float[] m_weights;
    private readonly float m_total_weight;
    private int m_current;

    public RunBaseSequence(IEnumerable<IRunBase> runBases)
    {
      if (runBases == null)
        throw new ArgumentNullException("runBases");

      m_run_bases = runBases.Where(r => r != null).ToArray();
      m_weights = new float[m_run_bases.Length];

      for (int i = 0; i < m_run_bases.Length; i++)
        m_weights[i] = this.GetWeight(m_run_bases[i]);

      m_total_weight = m_weights.Sum();
    }

    public override void Run()
    {
      if (m_run_bases.Length == 0)
        throw new InvalidOperationException("No tasks to run");

      m_current = -1;

      for (int i = 0; i < m_run_bases.Length; i++)
      {
        m_current = i;

        try
        {
          var cancelable = m_run_bases[i] as ICancelableRunBase;
          if (cancelable != null)
          {
            cancelable.CanCancelChanged += this.HandeCanCancelchanged;
            cancelable.CancellationCheck += this.HandleCancellationCheck;
          }

          m_run_bases[i].ProgressChanged += this.HandleReportProgress;

          m_run_bases[i].Run();
        }
        finally
        {
          var cancelable = m_run_bases[i] as ICancelableRunBase;
          if (cancelable != null)
          {
            cancelable.CanCancelChanged -= this.HandeCanCancelchanged;
            cancelable.CancellationCheck -= this.HandleCancellationCheck;
          }

          m_run_bases[i].ProgressChanged -= this.HandleReportProgress;
        }
      }
    }

    private float GetWeight(IRunBase runBase)
    {
      var sp = runBase as IServiceProvider;

      if (sp == null)
        return 1;

      var tvo = sp.GetService<TaskVisualizationOverride>();

      if (tvo == null)
        return 1;

      return tvo.Weight > 0 ? tvo.Weight.Value : 1;
    }

    private void HandleReportProgress(object sender, ProgressChangedEventArgs e)
    {
      var status = e.UserState as string;

      var progress = (int)(m_weights.Take(m_current).Sum() * 100 / m_total_weight
        + m_weights[m_current] * e.ProgressPercentage / m_total_weight);

      this.ReportProgress(progress, status);
    }

    private void HandleCancellationCheck(object sender, CancelEventArgs e)
    {
      e.Cancel = this.CancellationPending;
    }

    private void HandeCanCancelchanged(object sender, EventArgs e)
    {
      this.CanCancel = m_current >= 0 && m_run_bases[m_current] is ICancelableRunBase
        && ((ICancelableRunBase)m_run_bases[m_current]).CanCancel;
    }
  }
}
