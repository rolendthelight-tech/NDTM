using System.ComponentModel;
using DevExpress.XtraGrid;
using System.Drawing.Design;

namespace Toolbox.GUI.DX.Extensions
{
  /// <summary>
  /// Набор расширений для XtraGrid
  /// </summary>
  public class GridControlExtender : Component, ISupportInitialize
  {
    #region Fields

    private BindingList<GridControlFeature> m_features = new BindingList<GridControlFeature>();

    #endregion

    #region Constructors

    public GridControlExtender()
    {
    }

    public GridControlExtender(IContainer container)
    {
      container.Add(this);
    }

    #endregion

    #region Properties

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor(typeof(GridControlFeatureEditor), typeof(UITypeEditor))]
    [Category("Behavior")]
    public BindingList<GridControlFeature> Features
    {
      get { return m_features; }
    }

    /// <summary>
    /// Грид, функциональность которого надо расширить
    /// </summary>
    [Category("Behavior")]
    public GridControl Grid { get; set; }

    #endregion

    #region ISupportInitialize Members

    void ISupportInitialize.BeginInit()
    {
    }

    /// <summary>
    /// После инициализации применяем заданные расширения
    /// </summary>
    void ISupportInitialize.EndInit()
    {
      if (this.Grid == null)
        return;

      foreach (GridControlFeature feature in this.Features)
      {
        feature.ApplyFeature(this.Grid);
        feature.InitLocalization();
      }
    }

    #endregion
  }
}
