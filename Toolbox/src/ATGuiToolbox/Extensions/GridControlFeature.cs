using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DevExpress.XtraGrid;
using System.Windows.Forms;

namespace AT.Toolbox.Extensions
{
  /// <summary>
  /// Базовый класс для расширений XtraGrid
  /// </summary>
  [ToolboxItem(false)]
  public abstract class GridControlFeature : IComponent, ISwitchableLanguage
  {
    public GridControlFeature()
    {
    }

    public GridControlFeature(IContainer container)
    {
      container.Add(this);
    }

    /// <summary>
    /// Обеспечивает внедрение расширения в грид
    /// </summary>
    /// <param name="grid">Грид, который требуется расширить</param>
    public abstract void ApplyFeature(GridControl grid);

    #region IComponent Members

    public event EventHandler Disposed;

    [Browsable(false)]
    public ISite Site { get; set; }

    #endregion

    #region IDisposable Members

    ~GridControlFeature()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.Terminating != null)
        {
          this.Terminating(this, EventArgs.Empty);
        }
        if (this.Disposed != null)
        {
          this.Disposed(this, EventArgs.Empty);
        }
      }
    }

    #endregion

    #region ISwitchableLanguage Members

    public void InitLocalization()
    {
      AppSwitchablePool.RegisterSwitchable(this);
      this.PerformLocalization(this, EventArgs.Empty);
    }

    public virtual void PerformLocalization(object sender, EventArgs e)
    {
    }

    public event EventHandler Terminating;

    #endregion
  }
}
