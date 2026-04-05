using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT.Toolbox.Files;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using DevExpress.XtraBars;
using AT.Toolbox.Misc;
using AT.Toolbox.Base;
using AT.Toolbox.Dialogs;

namespace AT.Toolbox.MSSQL
{
  public interface IDataImporter
  {
    bool Import(DatabaseBrowserControl control);
  }

  /// <summary>
  /// Почему-то через анонимный метод не работает, этот класс его заменяет
  /// </summary>
  internal class ImportWrapper
  {
    public IDataImporter Importer { get; set; }
    public DatabaseBrowserControl Control { get; set; }
    public void HandleItemClickEvent(object sender, ItemClickEventArgs e)
    {
      if (this.Control == null || string.IsNullOrEmpty(this.Control.CurrentDatabase))
        return;

      if (this.Importer.Import(this.Control))
      {
        //this.Control.logger.Info(Properties.Resources.DB_BROWSER_IMPORT_DATA_COMPLETE); // TODO: MessageService
      }
    }
  }

  internal class ImportWorkWrapper
  {
    public Type ImportType { get; set; }

    public DatabaseBrowserControl Control { get; set; }

    public string Caption { get; set; }

    public void HandleItemClickEvent(object sender, ItemClickEventArgs e)
    {
      if (this.Control == null || string.IsNullOrEmpty(this.Control.CurrentDatabase))
        return;

      IDataImportWork wrk = (IDataImportWork)Activator.CreateInstance(this.ImportType);

      wrk.ConnectionString = this.Control.CheckConnectionString(this.Control.CurrentDatabase);

      if (!wrk.Init())
        return;

      ImportDialogFormBase dialog = wrk.GetPromptForm();
      dialog.Work = wrk;

      if (dialog.ShowDialog(this.Control) == DialogResult.OK)
      {
        wrk.GetDataFormDialog(dialog);
        Form workForm = null;
        if (wrk.SupportsPercentNotification)
        {
          workForm = new BackgroundWorkerForm();
          ((BackgroundWorkerForm)workForm).Work = wrk;
        }
        else
        {
          workForm = new BackgroundWorkerForm();
          ((BackgroundWorkerForm)workForm).Work = wrk;
        }

        if (workForm.ShowDialog(this.Control) == DialogResult.OK)
        {
          //this.Control.logger.Info(Properties.Resources.DB_BROWSER_IMPORT_DATA_COMPLETE, new Exception(wrk.Name)); // TODO: MessageService
        }
      }
    }
  }

  public class ImportDialogFormBase : LocalizableForm
  {
    public IBackgroundWork Work { get; set; }
  }

  public abstract class DataImportWorkBase : IDataImportWork
  {
    #region IDataImportWork Members

    public string ConnectionString { get; set; }

    public virtual bool SupportsPercentNotification
    {
      get { return false; }
    }

    public virtual bool Init()
    {
      return true;
    }

    public abstract ImportDialogFormBase GetPromptForm();

    #endregion

    #region IBackgroundWork Members

    public virtual bool CloseOnFinish
    {
      get { return true; }
    }

    public virtual bool CanCancel
    {
      get { return false; }
    }

    public bool IsMarquee
    {
      get { return false; }
    }

    public object Result
    {
      get { return null; }
    }


    public virtual System.Drawing.Bitmap Icon
    {
      get { return null; }
    }

    public virtual string Name
    {
      get
      {
        if (this.GetType().IsDefined(typeof(DisplayNameAttribute), true))
        {
          return (this.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
            as DisplayNameAttribute).DisplayName;
        }

        return this.GetType().Name;
      }
    }

    public virtual float Weight
    {
      get { return 1; }
    }

    public abstract void Run(BackgroundWorker worker);

    public virtual void GetDataFormDialog(ImportDialogFormBase dialog) { }

    protected void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, e);
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }

  public interface IDataImportWork : IBackgroundWork
  {
    string ConnectionString { get; set; }

    bool SupportsPercentNotification { get; }

    bool Init();

    void GetDataFormDialog(ImportDialogFormBase dialog);

    ImportDialogFormBase GetPromptForm();
  }
}
