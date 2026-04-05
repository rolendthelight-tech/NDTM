using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Toolbox.GUI.Base;
using DevExpress.XtraBars;
using System.Windows.Forms;
using System.IO;

namespace Toolbox.GUI.DX.Controls.Menu
{
  public partial class LastFilesMenuCreator : Component, ILastFileView, ISupportInitialize
  {
	  [NotNull] private readonly LastFileList m_list;
    private bool m_initialized;
    private BarSubItem m_menu;

    public LastFilesMenuCreator()
    {
      m_list = new LastFileList(this);
    }

    public LastFilesMenuCreator([NotNull] IContainer container)
    {
	    if (container == null) throw new ArgumentNullException("container");

			container.Add(this);

      m_list = new LastFileList(this);
    }

    public event EventHandler<FileNameEventArgs> FileOpened;

    public BarSubItem Menu
    {
      get { return m_menu; }
      set
      {
        if (ReferenceEquals(m_menu, value))
          return;

        m_menu = value;
        m_initialized = false;
      }
    }

    [DefaultValue(null)]
    public BarShortcut Shortcut { get; set; }

    [DefaultValue(null)]
    public string Filter { get; set; }

    #region ILastFileView Members

		void ILastFileView.AddMenuItem(string caption, [NotNull] [PathReference] string path, bool atStart)
    {
	    if (path == null) throw new ArgumentNullException("path");

			if (this.Menu != null)
      {
        var item = new BarButtonItem();
        item.Caption = caption;

        item.ItemClick += delegate
        {
          if (this.FileOpened != null)
          {
            if (!File.Exists(path) && !Directory.Exists(path))
              return;

            this.FileOpened(this, new FileNameEventArgs(path));
          }

          m_list.Update(path);
        };

        if (this.Menu.Manager != null)
          this.Menu.Manager.Items.Add(item);

        if (atStart)
          this.Menu.ItemLinks.Insert(0, item);
        else
          this.Menu.ItemLinks.Add(item);
      }
    }

	  void ILastFileView.Clear()
    {
      if (this.Menu != null)
      {
        foreach (var link in this.Menu.ItemLinks.OfType < BarItemLink>().ToArray())
        {
          if (this.Menu.Manager != null)
            this.Menu.Manager.Items.Remove(link.Item);
        }

        this.Menu.ClearLinks();
      }
    }

    void ILastFileView.Hide()
    {
      if (m_menu != null)
        m_menu.Visibility = BarItemVisibility.Never;
    }

    void ILastFileView.Show()
    {
      if (m_menu != null)
      {
        m_menu.Visibility = BarItemVisibility.Always;

        if (m_menu.ItemLinks.Count > 0 && this.Shortcut != null)
        {
          m_menu.ItemLinks[0].Item.ItemShortcut = new BarShortcut(this.Shortcut);
          m_menu.ItemLinks[0].Item.ItemShortcut.DisplayString = " ";
        }
      }
    }

    public void OpenFile()
    {
      using (var dlg = new OpenFileDialog())
      {
        dlg.Filter = this.Filter;

        if (dlg.ShowDialog(this.Menu.Manager != null ? this.Menu.Manager.Form : null) == DialogResult.OK)
        {
          if (this.FileOpened != null)
            this.FileOpened(this, new FileNameEventArgs(dlg.FileName));

          m_list.Update(dlg.FileName);
        }
      }
    }

		public void Update([NotNull] [PathReference] string fileName)
    {
	    if (fileName == null) throw new ArgumentNullException("fileName");

			m_list.Update(fileName);
    }

	  [CanBeNull]
	  System.Drawing.Font ILastFileView.Font
    {
      get { return this.Menu != null ? this.Menu.Font : null; }
    }

    [DefaultValue(0)]
    public int Width { get; set; }

    #endregion

    #region ISupportInitialize Members

    void ISupportInitialize.BeginInit()
    {
    }

    void ISupportInitialize.EndInit()
    {
      if (!m_initialized && this.Menu != null && !this.DesignMode)
      {
        m_list.Init();
        m_initialized = true;

        if (m_menu != null)
        {
          m_menu.Visibility = BarItemVisibility.Always;

          if (m_menu.ItemLinks.Count > 0 && this.Shortcut != null)
          {
            m_menu.ItemLinks[0].Item.ItemShortcut = new BarShortcut(this.Shortcut);
            m_menu.ItemLinks[0].Item.ItemShortcut.DisplayString = " ";
            m_menu.Caption += string.Format("\t{0}", this.Shortcut);
          }
        }
      }
    }

    #endregion
  }
}
