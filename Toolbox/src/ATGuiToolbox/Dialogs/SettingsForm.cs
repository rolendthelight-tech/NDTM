using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AT.Toolbox.Base;
using AT.Toolbox.Misc;
using AT.Toolbox.Properties;
using AT.Toolbox.Settings;
using DevExpress.XtraNavBar;

namespace AT.Toolbox.Dialogs
{
  /// <summary>
  /// Форма настроек 
  /// </summary>
  public partial class SettingsForm : LocalizableForm 
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SettingsForm).Name);

    #region Private Variables 

    private bool m_restart_required;
    private readonly Dictionary<Type, PropertyPageWrapper> m_pages = new Dictionary<Type, PropertyPageWrapper>();
    private readonly HashSet<Type> m_skip_restart = new HashSet<Type>();
    private readonly BindingList<SettingError> m_errors = new BindingList<SettingError>();

    #endregion

    public SettingsForm()
    {
      InitializeComponent();
    }

    #region Properties

    /// <summary>
    /// Внешний буфер сообщений. Используется для 
    /// </summary>
    public InfoBuffer ExternalBuffer { get; set; }

    #endregion

    #region Private Methods 

    private void CreateMenu(IList<IPropertyPage> list)
    {
      foreach (var item_group in list.ToLookup(p => p.Group))
      {
        NavBarGroup grp = new NavBarGroup(item_group.Key);
        grp.Name = item_group.Key;

        m_nav_bar.Groups.Add(grp);

        foreach (var pg in item_group)
        {
          NavBarItem itm = new NavBarItem(pg.PageName);
          itm.LinkClicked += this.HandleLinkClicked;
          itm.Tag = pg.GetType();
          itm.LargeImage = pg.Image;
          itm.SmallImage = pg.Image;
          m_nav_bar.Items.Add(itm);

          NavBarItemLink lnk = new NavBarItemLink(itm);

          grp.ItemLinks.Add(lnk);

          m_pages.Add(pg.GetType(), new PropertyPageWrapper(pg, itm));
        }

        grp.Expanded = true;
      }
    }

    void SelectPage(IPropertyPage pg)
    {
      if (null == pg)
        return;

      m_splitter.Panel2.Controls.Clear();
      m_splitter.Panel2.Controls.Add(pg.Page);
      pg.Page.Dock = DockStyle.Fill;
    }

    private void ValidateSettings(bool useExternalBuffer)
    {
      m_errors.Clear();

      foreach (var pw in m_pages.Values)
      {
        if (pw.Page.UIThreadValidation)
        {
          var buffer = new InfoBuffer();

          pw.Valid = pw.Page.ValidateSetting(buffer);

          foreach (var info in buffer)
            m_errors.Add(new SettingError { PageType = pw.Page.GetType(), Message = info.Message });
        }
      }

      if (useExternalBuffer && this.ExternalBuffer != null)
      {
        foreach (var pw in m_pages.Values)
        {
          if (pw.Valid == null)
          {
            foreach (var info in this.ExternalBuffer)
            {
              if (pw.Page.IsSourceOfMessage(info))
              {
                m_errors.Add(new SettingError { PageType = pw.Page.GetType(), Message = info.Message });
                
                if (info.Level > InfoLevel.Warning)
                  pw.Valid = false;

                m_skip_restart.Add(pw.Page.GetType());
              }
            }
          }
        }
      }

      var vw = new ValidationWork(m_pages.Values);

      if (vw.RunRequired)
      {
        using (var bg_frm = new BackgroundWorkerForm())
        {
          bg_frm.Work = vw;
          bg_frm.ShowDialog(this);

          foreach (var err in vw.Errors)
            m_errors.Add(err);
        }
      }

      m_error_splitter.Panel2Collapsed = (m_errors.Count == 0);
    }

    private bool ApplySettings()
    {
      foreach (var pw in m_pages.Values)
      {
        if (pw.Changed || pw.Valid == false)
          pw.Valid = null;
      }
      
      this.ValidateSettings(false);

      foreach (var pw in m_pages)
        pw.Value.Update();

      if (m_pages.Any(kv => kv.Value.Valid != true))
        return false;

      foreach (var pw in m_pages.Values)
      {
        if (pw.Changed)
        {
          if (pw.Page.RestartRequired && !m_skip_restart.Contains(pw.Page.GetType()))
            m_restart_required = true;
          
          pw.Page.ApplySettings();
          pw.Changed = false;
          pw.Update();
        }
      }

      return true;
    }

    #endregion

    #region Form handlers

    public override void PerformLocalization(object sender, EventArgs e)
    {
      try
      {
        Text = string.Format("{0}: {1} ({2}) ", Resources.SETTINGS, 
          ApplicationInfo.MainAttributes.ProductTitle, 
          ApplicationInfo.MainAttributes.Version);

        m_validate_button.Text = Resources.SETTINGS_VALIDATE;
        m_cancel_btn.Text = Resources.CANCEL;
        m_ok_btn.Text = Resources.OK;

        foreach (NavBarGroup grp in m_nav_bar.Groups)
        {
          foreach (NavBarItemLink lnk in grp.ItemLinks)
          {
            if (null == lnk.Item)
              continue;

            if (null == lnk.Item.Tag)
              continue;

            var pt = lnk.Item.Tag as Type;

            if (pt == null || !m_pages.ContainsKey(pt))
              continue;

            var pg = m_pages[pt].Page;

            grp.Caption = pg.Group;
            lnk.Item.Caption = pg.PageName;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("PerformLocalization(): exception", ex);
        //m_log.Error(@"Error in SettingsForm::PerformLocalization()", ex);
      }
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      settingErrorBindingSource.DataSource = m_errors;

      if (m_pages.Count > 0)
        return;

      var frm2 = new BackgroundWorkerForm();
      var work = new LoadSettingsForm() { Form = this };
      frm2.Work = work;
      if (frm2.ShowDialog(this) != DialogResult.OK)
      {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
      }

      this.CreateMenu(work.Classes);

      this.ValidateSettings(true);

      foreach (var pw in m_pages)
      {
        pw.Value.Page.Changed += this.HandlePageChanged;
        pw.Value.Update();
      }

      if (m_pages.Any(p => p.Value.Valid != true))
        m_ok_btn.Enabled = false;
    }

    private void HandleNavDoubleClick(object sender, EventArgs e)
    {
      object o = gridView1.GetRow(gridView1.FocusedRowHandle);

      if (null == o)
        return;

      SettingError Err = o as SettingError;

      if (null == Err)
        return;

      SelectPage(m_pages[Err.PageType].Page);
    }

    private void HandleValidate(object sender, EventArgs e)
    {
      foreach (var pw in m_pages.Values)
      {
        if (pw.Changed || pw.Valid == false)
          pw.Valid = null;
      }

      this.ValidateSettings(false);

      foreach (var pw in m_pages)
        pw.Value.Update();

      m_ok_btn.Enabled = m_pages.All(kv => kv.Value.Valid == true);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (this.DialogResult == DialogResult.OK)
      {
        var apply_required = m_pages.Any(kv => kv.Value.Changed || kv.Value.Valid != true);
        if (apply_required && !this.ApplySettings())
        {
          e.Cancel = true;
          m_ok_btn.Enabled = false;
        }
        else
        {
          AppManager.Configurator.SaveSettings();

          if (m_restart_required)
          {
            MessageBoxEx box = new MessageBoxEx();

            box.StandardIcon = MessageBoxEx.Icons.Question;
            box.Message = Resources.SETTINGS_RESTART_WARNING;
            box.Caption = Resources.SETTINGS_RESTART_TITLE;
            box.Buttons = MessageBoxButtons.YesNo;

            if (box.ShowDialog(this) == DialogResult.Yes)
            {
              var timer = new Timer();
              timer.Tick += new RestartTimer().HandleTick;
              timer.Start();
            }
          }
        }
      }
    }

    #endregion

    #region Dynamic handlers

    private void HandleLinkClicked(object sender, NavBarLinkEventArgs e)
    {
      var tag = e.Link.Item.Tag as Type;

      if (tag == null || !m_pages.ContainsKey(tag))
        return;

      this.SelectPage(m_pages[tag].Page);
    }

    private void HandlePageChanged(object sender, EventArgs e)
    {
      if (sender == null || !m_pages.ContainsKey(sender.GetType()))
        return;

      var pw = m_pages[sender.GetType()];

      pw.Changed = true;

      if (pw.Page.UIThreadValidation)
      {
        var buffer = new InfoBuffer();
        pw.Valid = pw.Page.ValidateSetting(buffer);

        var removee = m_errors.Where(er => er.PageType == sender.GetType()).ToList();

        foreach (var rem in removee)
          m_errors.Remove(rem);

        if (pw.Valid == false)
        {
          foreach (var msg in buffer)
          {
            m_errors.Add(new SettingError
            {
              Message = msg.Message,
              PageType = sender.GetType()
            });
          }
        }
      }
      else
        pw.Valid = null;

      pw.Update();
      m_error_splitter.Panel2Collapsed = (m_errors.Count == 0);
      m_ok_btn.Enabled = m_pages.All(kv => kv.Value.Valid == true);
    }

    #endregion

    #region Class helpers

    private class RestartTimer
    {
      public void HandleTick(object sender, EventArgs e)
      {
        var timer = sender as Timer;

        if (timer != null)
        {
          timer.Tick -= this.HandleTick;
          timer.Stop();
          timer.Dispose();
        }

        AppManager.Instance.Restart();
      }
    }

    private class SettingError
    {
      public string Message { get; set; }

      public Type PageType { get; set; }
    }

    private class PropertyPageWrapper
    {
      private readonly IPropertyPage m_page;
      private readonly NavBarItem m_item;

      public PropertyPageWrapper(IPropertyPage page, NavBarItem item)
      {
        m_page = page;
        m_item = item;
      }

      public IPropertyPage Page
      {
        get { return m_page; }
      }

      public NavBarItem Item
      {
        get { return m_item; }
      }

      public bool Changed { get; set; }

      public bool? Valid { get; set; }

      public void Update()
      {
        if (this.Changed || this.Valid == false)
          this.Item.Appearance.Font = new Font(this.Item.Appearance.Font, FontStyle.Bold);
        else
          this.Item.Appearance.Font = new Font(this.Item.Appearance.Font, FontStyle.Regular);

        if (this.Valid == true)
        {
          if (this.Changed)
            this.Item.Appearance.ForeColor = Color.Blue;
          else
            this.Item.Appearance.ForeColor = Color.Black;
        }
        else if (this.Valid == false)
          this.Item.Appearance.ForeColor = Color.Red;
        else
          this.Item.Appearance.ForeColor = Color.Black;

        this.Item.AppearanceHotTracked.Font = new Font(this.Item.Appearance.Font,
          FontStyle.Underline | this.Item.Appearance.Font.Style);

        this.Item.AppearancePressed.AssignInternal(this.Item.AppearanceHotTracked);
      }
    }

    private class ValidationWork : IBackgroundWork
    {
      private readonly IList<PropertyPageWrapper> m_pages;
      private readonly List<SettingError> m_errors = new List<SettingError>();

      public ValidationWork(IEnumerable<PropertyPageWrapper> wrappers)
      {
        m_pages = wrappers.Where(pw => !pw.Page.UIThreadValidation && pw.Valid == null).ToList();
      }

      public bool RunRequired
      {
        get { return m_pages.Count > 0; }
      }

      public List<SettingError> Errors
      {
        get { return m_errors; }
      }

      #region IBackgroundWork Members

      public bool CloseOnFinish
      {
        get { return true; }
      }

      public bool IsMarquee
      {
        get { return true; }
      }

      public bool CanCancel
      {
        get { return false; }
      }

      public System.Drawing.Bitmap Icon
      {
        get { return Properties.Resources.p_48_info; }
      }

      public string Name
      {
        get { return "Проверка настроек"; }
      }

      public float Weight
      {
        get { return 0; }
      }

      public object Result
      {
        get { return null; }
      }

      public void Run(BackgroundWorker worker)
      {
        this.PropertyChanged += delegate { };

        foreach (var pw in m_pages)
        {
          worker.ReportProgress(0, pw.Page.PageName);
          
          var buffer = new InfoBuffer();
          pw.Valid = pw.Page.ValidateSetting(buffer);

          if (pw.Valid == false)
          {
            foreach (var info in buffer)
              m_errors.Add(new SettingError { Message = info.Message, PageType = pw.Page.GetType() });
        }}
      }

      #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion
    }

    private class LoadSettingsForm : IBackgroundWork
    {
      List<IPropertyPage> m_classes;

      #region IBackgroundWork Members

      public bool CloseOnFinish
      {
        get { return true; }
      }

      public bool IsMarquee
      {
        get { return true; }
      }

      public bool CanCancel
      {
        get { return false; }
      }

      public System.Drawing.Bitmap Icon
      {
        get { return Properties.Resources.p_48_info; }
      }

      public string Name
      {
        get { return "Загрузка страниц настроек"; }
      }

      public float Weight
      {
        get { return 0; }
      }

      public object Result
      {
        get { return m_classes; }
      }

      public List<IPropertyPage> Classes
      {
        get { return m_classes; }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      public SettingsForm Form{ get; set; }

      public void Run(BackgroundWorker worker)
      {
        this.PropertyChanged += delegate { };
        
        int i = 0;
        AssemblyCollector<IPropertyPage> PageCollector = new AssemblyCollector<IPropertyPage>(this.Form);

        foreach (Assembly list in AppManager.AssemblyClassifier.ToolboxAssemblies.ToArray())
        {
          worker.ReportProgress(i++, "Загрузка страниц");
          
          try
          {
            PageCollector.Collect(list, AssemblyCollectMode.Interface);
          }
          catch (ReflectionTypeLoadException ex)
          {
            foreach (var loaderEx in ex.LoaderExceptions)
              Log.Error(ex.Message, loaderEx);
            
            Log.Error(ex.Message, ex);
          }
          catch(Exception ex)
          {
            Log.Error(ex);
          }

          if (i > 90)
            i = 0;
        }

        var wrapper = new SynchronizeOperationWrapper(this.Form);
        
        foreach (IPropertyPage page in PageCollector.Classes)
        {
          worker.ReportProgress( i++ , "Инициализация страниц" );

          try
          {
            wrapper.Invoke(() => page.GetCurrentSettings());
          }
          catch (Exception ex)
          {
            Log.Error(ex);
          }
          
          if (i > 90)
            i = 0;
        }

        m_classes = PageCollector.Classes;
      }

    #endregion
    }

    #endregion
  }
}
