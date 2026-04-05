using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Заменитель BackgroundWorker когда требуется IReportProgress
  /// </summary>
  public class BackgroundWorkerReportProgress : BackgroundWorker, IReportProgress
  {
    private int m_percentage;

    public BackgroundWorkerReportProgress()
    {
      this.WorkerReportsProgress = true;
    }

    #region Implementation of IReportProgress

    void IReportProgress.ReportProgress(string state)
    {
      ((IReportProgress)(this)).ReportProgress(m_percentage, state);
    }

    void IReportProgress.ReportProgress(int percentage, string state)
    {
			if (percentage < 0 || percentage > 100)
				throw new ArgumentOutOfRangeException("percentage", percentage, "percentage < 0 ∨ percentage > 100");

			this.m_percentage = percentage;
      base.ReportProgress(percentage, state);
    }

    void IReportProgress.ReportProgress(int percentage)
    {
      ((IReportProgress)(this)).ReportProgress(percentage, null);
    }

    #endregion
  }
}
