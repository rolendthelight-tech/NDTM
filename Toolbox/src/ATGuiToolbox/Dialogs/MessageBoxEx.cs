using System;
using System.Drawing;
using System.Windows.Forms;
using AT.Toolbox.Properties;
using DevExpress.XtraEditors;
using AT.Toolbox.Base;

namespace AT.Toolbox.Dialogs
{
  /// <summary>
  /// Немного расширеный MessageBox
  /// </summary>
  public partial class MessageBoxEx : LocalizableForm
  {
    //TODO: Локализация

    #region Icons Enumeration ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Стандартные иконки
    /// 
    /// <list type="table">
    /// <item>
    /// <term>Ok</term>
    /// <description>Всё OK</description>
    /// </item>
    /// <item>
    /// <term>Error</term>
    /// <description>Ошибка пользователя</description>
    /// </item>
    /// <item>
    /// <term>Warning</term>
    /// <description>Предупреждение</description>
    /// </item>
    /// <item>
    /// <term>Question</term>
    /// <description>Вопрос</description>
    /// </item>
    /// <item>
    /// <term>Information</term>
    /// <description>Информация</description>
    /// </item>
    /// <item>
    /// <term>Stop</term>
    /// <description>Знак стоп</description>
    /// </item>
    /// <item>
    /// <term>Denied</term>
    /// <description>Действие невозможно</description>
    /// </item>
    /// <item>
    /// <term>Custom</term>
    /// <description>Ни одна из вышеперечисленых</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum Icons
    {
      Ok,
      Error,
      Warning,
      Question,
      Information,
      Stop,
      Denied,
      Custom
    }

    #endregion

    #region Private Variables ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Набор кнопок
    /// </summary>
    private MessageBoxButtons m_buttons;

    /// <summary>
    /// Иконка
    /// </summary>
    private Icons m_icon;

    #endregion

    public MessageBoxEx()
    {
      InitializeComponent();

      if (! DesignMode)
      {
        StandardIcon = Icons.Information;
      }
    }

    public override void PerformLocalization(object sender, EventArgs e)
    {
      base.PerformLocalization(sender, e);
      m_no_btn.Text = Resources.NO;
      m_ok_btn.Text = Resources.OK;
      m_cancel_btn.Text = Resources.CANCEL;
      m_yes_btn.Text = Resources.YES;
      m_abort_btn.Text = Resources.ABORT;
      m_retry_btn.Text = Resources.RETRY;
      m_ignore_btn.Text = Resources.IGNORE;
    }

    public MessageBoxEx(string caption, string message, MessageBoxButtons buttons)
      :this()
    {
      Caption = caption;
      Message = message;
      Buttons = buttons;
    }

    public MessageBoxEx(string caption, string message, MessageBoxButtons buttons, Icons icon )
      : this()
    {
      Caption = caption;
      Message = message;
      Buttons = buttons;
      StandardIcon = icon;
    }

    public MessageBoxEx(string caption, string message)
      : this()
    {
      Caption = caption;
      Message = message;
    }

    #region Public Properties ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Заголовок
    /// </summary>
    public string Caption
    {
      get { return m_group.Text; }
      set { m_group.Text = value; }
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message
    {
      get { return m_message.Text; }
      set { m_message.Text = value; }
    }

    /// <summary>
    /// Набор кнопок для отображения
    /// </summary>
    public MessageBoxButtons Buttons
    {
      get { return m_buttons; }
      set
      {
        m_buttons = value;

        m_ok_btn.Visible = false;
        m_cancel_btn.Visible = false;
        m_yes_btn.Visible = false;
        m_no_btn.Visible = false;
        m_abort_btn.Visible = false;
        m_retry_btn.Visible = false;
        m_ignore_btn.Visible = false;

        switch (value)
        {
          case MessageBoxButtons.OK:
            m_ok_btn.Visible = true;

            AcceptButton = m_ok_btn;
            m_ok_btn.Focus();
            break;
          case MessageBoxButtons.OKCancel:
            m_ok_btn.Visible = true;
            m_cancel_btn.Visible = true;

            CancelButton = m_cancel_btn;
            AcceptButton = m_ok_btn;
            m_ok_btn.Focus();
            break;
          case MessageBoxButtons.AbortRetryIgnore:
            m_abort_btn.Visible = true;
            m_retry_btn.Visible = true;
            m_ignore_btn.Visible = true;

            AcceptButton = m_ok_btn;
            CancelButton = m_cancel_btn;
            m_ok_btn.Focus();
            break;
          case MessageBoxButtons.YesNoCancel:
            m_yes_btn.Visible = true;
            m_no_btn.Visible = true;
            m_cancel_btn.Visible = true;

            CancelButton = m_cancel_btn;
            AcceptButton = m_yes_btn;
            m_yes_btn.Focus();
            break;
          case MessageBoxButtons.YesNo:
            m_yes_btn.Visible = true;
            m_no_btn.Visible = true;

            AcceptButton = m_yes_btn;
            CancelButton = m_no_btn;
            m_yes_btn.Focus();
            break;
          case MessageBoxButtons.RetryCancel:
            m_retry_btn.Visible = true;
            m_cancel_btn.Visible = true;

            AcceptButton = m_retry_btn;
            CancelButton = m_cancel_btn;
            m_retry_btn.Focus();
            break;
        }
      }
    }

    /// <summary>
    /// Заданая иконка
    /// </summary>
    public Image CustomIcon
    {
      get { return m_picture.Image; }
      set { m_picture.Image = value; }
    }

    /// <summary>
    /// Стандартная иконка из набора
    /// </summary>
    public Icons StandardIcon
    {
      get { return m_icon; }
      set
      {
        m_icon = value;

        switch (value)
        {
          case Icons.Error:
            m_picture.Image = Resources.p_48_user_error;
            break;
          case Icons.Warning:
            m_picture.Image = Resources.p_48_warning;
            break;
          case Icons.Question:
            m_picture.Image = Resources.p_48_question;
            break;
          case Icons.Information:
            m_picture.Image = Resources.p_48_info;
            break;
          case Icons.Stop:
            m_picture.Image = Resources.p_48_stop;
            break;
          case Icons.Denied:
            m_picture.Image = Resources.p_48_forbidden;
            break;
          case Icons.Ok:
            m_picture.Image = Resources.p_48_ok;
            break;
          case Icons.Custom:
            break;
        }
      }
    }

    #endregion

    #region Private Methods -----------------------------------------------------------------------------------------------------

    private void HandleShown(object sender, EventArgs e)
    {
      // Fix для корретной установки клавиши по умолчанию -- иначе всегда ставится Cancel
      Buttons = Buttons;
      AcceptButton.NotifyDefault(true);

      Size sz = m_message.GetPreferredSize( m_message.Size );

      this.Height += sz.Height - m_message.Size.Height;
    }

    #endregion

    public static void ShowError(string message, IWin32Window parent)
    {
        var box = new MessageBoxEx
                      {
                          Caption = Resources.FAIL,
                          StandardIcon = Icons.Error,
                          Message = message,
                          Buttons = MessageBoxButtons.OK,
                          StartPosition = FormStartPosition.CenterParent,
                          TopMost = true
                      };

        box.ShowDialog(parent);
    }

    public static void ShowInfo(string message, IWin32Window parent)
    {
      var box = new MessageBoxEx
      {
        Caption = Resources.INFO_TITLE,
        StandardIcon = Icons.Information,
        Message = message,
        Buttons = MessageBoxButtons.OK,
        StartPosition = FormStartPosition.CenterParent,
        TopMost = true
      };

      box.ShowDialog(parent);
    }

    public new void Close()
    {
      base.Close(  );
    }
  }
}