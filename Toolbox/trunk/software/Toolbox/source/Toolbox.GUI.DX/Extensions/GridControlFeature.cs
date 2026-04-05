using System;
using System.ComponentModel;
using DevExpress.XtraGrid;
using Toolbox.Application;

namespace Toolbox.GUI.DX.Extensions
{
  /// <summary>
  /// Базовый класс для расширений XtraGrid
  /// </summary>
  [ToolboxItem(false)]
  public abstract class GridControlFeature : IComponent, ISwitchableLanguage
  {
  	protected GridControlFeature()
    {
    }

  	protected GridControlFeature(IContainer container)
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
