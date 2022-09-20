namespace WaterMonitoring
{
    partial class PassWord
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassWord));
            this.btnIsCancel = new CCWin.SkinControl.SkinButton();
            this.btnIsOk = new CCWin.SkinControl.SkinButton();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lbl = new System.Windows.Forms.Label();
            this.btnEditPW = new CCWin.SkinControl.SkinButton();
            this.SuspendLayout();
            // 
            // btnIsCancel
            // 
            this.btnIsCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnIsCancel.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnIsCancel.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnIsCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIsCancel.DownBack = null;
            this.btnIsCancel.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIsCancel.ForeColor = System.Drawing.Color.White;
            this.btnIsCancel.Location = new System.Drawing.Point(276, 159);
            this.btnIsCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnIsCancel.MouseBack = null;
            this.btnIsCancel.Name = "btnIsCancel";
            this.btnIsCancel.NormlBack = null;
            this.btnIsCancel.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsCancel.Size = new System.Drawing.Size(231, 88);
            this.btnIsCancel.TabIndex = 49;
            this.btnIsCancel.Text = "取消";
            this.btnIsCancel.UseVisualStyleBackColor = false;
            this.btnIsCancel.Click += new System.EventHandler(this.btnIsCancel_Click);
            // 
            // btnIsOk
            // 
            this.btnIsOk.BackColor = System.Drawing.Color.Transparent;
            this.btnIsOk.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnIsOk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnIsOk.DownBack = null;
            this.btnIsOk.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIsOk.ForeColor = System.Drawing.Color.White;
            this.btnIsOk.Location = new System.Drawing.Point(32, 158);
            this.btnIsOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnIsOk.MouseBack = null;
            this.btnIsOk.Name = "btnIsOk";
            this.btnIsOk.NormlBack = null;
            this.btnIsOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsOk.Size = new System.Drawing.Size(231, 88);
            this.btnIsOk.TabIndex = 50;
            this.btnIsOk.Text = "确定";
            this.btnIsOk.UseVisualStyleBackColor = false;
            this.btnIsOk.Click += new System.EventHandler(this.btnIsOk_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPassword.ForeColor = System.Drawing.Color.SeaGreen;
            this.txtPassword.Location = new System.Drawing.Point(32, 88);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(476, 48);
            this.txtPassword.TabIndex = 48;
            this.txtPassword.Text = "123";
            this.txtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl.ForeColor = System.Drawing.Color.White;
            this.lbl.Location = new System.Drawing.Point(26, 26);
            this.lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(197, 40);
            this.lbl.TabIndex = 47;
            this.lbl.Text = "请输入密码：";
            // 
            // btnEditPW
            // 
            this.btnEditPW.BackColor = System.Drawing.Color.Transparent;
            this.btnEditPW.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnEditPW.BorderColor = System.Drawing.Color.Transparent;
            this.btnEditPW.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnEditPW.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnEditPW.DownBack = null;
            this.btnEditPW.FadeGlow = false;
            this.btnEditPW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditPW.Font = new System.Drawing.Font("微软雅黑", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEditPW.ForeColor = System.Drawing.Color.White;
            this.btnEditPW.IsDrawBorder = false;
            this.btnEditPW.IsDrawGlass = false;
            this.btnEditPW.Location = new System.Drawing.Point(384, 26);
            this.btnEditPW.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnEditPW.MouseBack = null;
            this.btnEditPW.Name = "btnEditPW";
            this.btnEditPW.NormlBack = null;
            this.btnEditPW.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnEditPW.Size = new System.Drawing.Size(129, 40);
            this.btnEditPW.TabIndex = 50;
            this.btnEditPW.Text = "修改密码";
            this.btnEditPW.UseVisualStyleBackColor = false;
            this.btnEditPW.Click += new System.EventHandler(this.btnEditPW_Click);
            // 
            // PassWord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(540, 280);
            this.ControlBox = false;
            this.Controls.Add(this.btnIsCancel);
            this.Controls.Add(this.btnEditPW);
            this.Controls.Add(this.btnIsOk);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PassWord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PassWord";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinButton btnIsCancel;
        private CCWin.SkinControl.SkinButton btnIsOk;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lbl;
        private CCWin.SkinControl.SkinButton btnEditPW;
    }
}