namespace Toolbox.GUI.Dialogs
{
  partial class LoginForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
		private global::System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing"><code>true</code> if managed resources should be disposed; otherwise, <code>false</code>.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support — do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			this.labelLogin = new global::System.Windows.Forms.Label();
			this.labelPassword = new global::System.Windows.Forms.Label();
			this.textLogin = new global::System.Windows.Forms.TextBox();
			this.textPassword = new global::System.Windows.Forms.TextBox();
			this.buttonOK = new global::System.Windows.Forms.Button();
			this.buttonCancel = new global::System.Windows.Forms.Button();
			this.checkBoxSave = new global::System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // labelLogin
      // 
      this.labelLogin.AutoSize = true;
			this.labelLogin.Location = new global::System.Drawing.Point(14, 22);
      this.labelLogin.Name = "labelLogin";
			this.labelLogin.Size = new global::System.Drawing.Size(32, 15);
      this.labelLogin.TabIndex = 0;
      this.labelLogin.Text = "Имя";
      // 
      // labelPassword
      // 
      this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new global::System.Drawing.Point(14, 51);
      this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new global::System.Drawing.Size(51, 15);
      this.labelPassword.TabIndex = 1;
      this.labelPassword.Text = "Пароль";
      // 
      // textLogin
      // 
			this.textLogin.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left)
									| global::System.Windows.Forms.AnchorStyles.Right)));
			this.textLogin.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 9F, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textLogin.Location = new global::System.Drawing.Point(86, 18);
      this.textLogin.Name = "textLogin";
			this.textLogin.Size = new global::System.Drawing.Size(240, 21);
      this.textLogin.TabIndex = 2;
      // 
      // textPassword
      // 
			this.textPassword.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left)
									| global::System.Windows.Forms.AnchorStyles.Right)));
			this.textPassword.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 9F, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textPassword.Location = new global::System.Drawing.Point(86, 47);
      this.textPassword.Name = "textPassword";
      this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new global::System.Drawing.Size(240, 21);
      this.textPassword.TabIndex = 3;
      // 
      // buttonOK
      // 
			this.buttonOK.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new global::System.Drawing.Point(145, 83);
      this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new global::System.Drawing.Size(87, 27);
      this.buttonOK.TabIndex = 4;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      // 
      // buttonCancel
      // 
			this.buttonCancel.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new global::System.Drawing.Point(239, 83);
      this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new global::System.Drawing.Size(87, 27);
      this.buttonCancel.TabIndex = 5;
      this.buttonCancel.Text = "Отмена";
      this.buttonCancel.UseVisualStyleBackColor = true;
      // 
      // checkBoxSave
      // 
      this.checkBoxSave.AutoSize = true;
			this.checkBoxSave.Location = new global::System.Drawing.Point(17, 86);
      this.checkBoxSave.Name = "checkBoxSave";
			this.checkBoxSave.Size = new global::System.Drawing.Size(89, 19);
      this.checkBoxSave.TabIndex = 6;
      this.checkBoxSave.Text = "Сохранить";
      this.checkBoxSave.UseVisualStyleBackColor = true;
      // 
      // LoginForm
      // 
      this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new global::System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.buttonCancel;
			this.ClientSize = new global::System.Drawing.Size(341, 123);
      this.Controls.Add(this.checkBoxSave);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.textPassword);
      this.Controls.Add(this.textLogin);
      this.Controls.Add(this.labelPassword);
      this.Controls.Add(this.labelLogin);
			this.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 9F, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "LoginForm";
			this.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Введите имя пользователя и пароль";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

		private global::System.Windows.Forms.Label labelLogin;
		private global::System.Windows.Forms.Label labelPassword;
		private global::System.Windows.Forms.TextBox textLogin;
		private global::System.Windows.Forms.TextBox textPassword;
		private global::System.Windows.Forms.Button buttonOK;
		private global::System.Windows.Forms.Button buttonCancel;
		private global::System.Windows.Forms.CheckBox checkBoxSave;
  }
}