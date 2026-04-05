using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DevExpress.XtraBars.Docking;
using Toolbox.Configuration;
using System.IO;
using log4net;
using Toolbox.Common;
using System.Windows.Forms;

namespace Toolbox.GUI.DX.Extensions
{
  public partial class DockManagerLayoutSaver : Component
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(DockManagerLayoutSaver));
    private int m_dock_hash;
    private DockManager m_dock_manager;

    public DockManagerLayoutSaver()
    {
      InitializeComponent();
    }

    public DockManagerLayoutSaver(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    [DefaultValue(null)]
    public DockManager Manager
    {
      get { return m_dock_manager; }
      set
      {
        m_dock_manager = value;
        m_dock_hash = m_dock_manager != null ? GetDockHashCode() : 0;
      }
    }

    public void SaveLayout()
    {
      if (m_dock_manager == null)
        return;

      string file_name = new ConfigFileFinder(string.Format("Layouts\\DockPanels.{0}.xml", m_dock_hash)).OutputConfigFilePath;

      if (!string.IsNullOrEmpty(file_name))
      {
        if (File.Exists(file_name))
          File.Delete(file_name);

        if (!Directory.Exists(Path.GetDirectoryName(file_name)))
          Directory.CreateDirectory(Path.GetDirectoryName(file_name));

        try
        {
          m_dock_manager.SaveLayoutToXml(file_name);
        }
        catch (Exception ex)
        {
          _log.Error("Failed to load layout", ex);
        }
      }
    }


    public void RestoreLayout()
    {
      if (m_dock_manager == null)
        return;

      string file_name = new ConfigFileFinder(string.Format("Layouts\\DockPanels.{0}.xml", m_dock_hash),
        true).InputConfigFilePath;

      if (!string.IsNullOrEmpty(file_name) && File.Exists(file_name))
      {
        try
        {
          m_dock_manager.RestoreLayoutFromXml(file_name);
        }
        catch (Exception ex)
        {
          _log.Error("Failed to load layout", ex);
        }
      }
    }

    public int GetDockHashCode()
    {
      return GetHashCodeHelper.CombineHashCodesCustomEnum(
        m_dock_manager.Panels.Cast<DockPanel>(),
        typeof(DockPanel).Assembly.GetName().Version.GetHashCode(),
        typeof(DockPanel).Assembly.GetName().Version.GetHashCode(),
        panel => GetHashCodeHelper.CombineHashCodesCustomEnum(
          panel.Controls.Cast<Control>(),
          panel.ID.GetHashCode(), panel.ID.GetHashCode(),
          ctrl => ctrl.Name.GetHashCode()));
    }
  }
}
