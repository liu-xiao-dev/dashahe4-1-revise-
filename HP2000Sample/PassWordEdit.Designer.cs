namespace WaterMonitoring
{
    partial class PassWordEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassWordEdit));
            this.btnIsCancel = new CCWin.SkinControl.SkinButton();
            this.btnIsOk = new CCWin.SkinControl.SkinButton();
            this.txtOldPassword = new System.Windows.Forms.TextBox();
            this.lbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
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
            this.btnIsCancel.Location = new System.Drawing.Point(181, 120);
            this.btnIsCancel.MouseBack = null;
            this.btnIsCancel.Name = "btnIsCancel";
            this.btnIsCancel.NormlBack = null;
            this.btnIsCancel.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsCancel.Size = new System.Drawing.Size(154, 59);
            this.btnIsCancel.TabIndex = 3;
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
            this.btnIsOk.Location = new System.Drawing.Point(15, 120);
            this.btnIsOk.MouseBack = null;
            this.btnIsOk.Name = "btnIsOk";
            this.btnIsOk.NormlBack = null;
            this.btnIsOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsOk.Size = new System.Drawing.Size(154, 59);
            this.btnIsOk.TabIndex = 2;
            this.btnIsOk.Text = "确定";
            this.btnIsOk.UseVisualStyleBackColor = false;
            this.btnIsOk.Click += new System.EventHandler(this.btnIsOk_Click);
            // 
            // txtOldPassword
            // 
            this.txtOldPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOldPassword.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOldPassword.ForeColor = System.Drawing.Color.SeaGreen;
            this.txtOldPassword.Location = new System.Drawing.Point(102, 25);
            this.txtOldPassword.Name = "txtOldPassword";
            this.txtOldPassword.Size = new System.Drawing.Size(233, 32);
            this.txtOldPassword.TabIndex = 0;
            this.txtOldPassword.Text = "123";
            this.txtOldPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtOldPassword.UseSystemPasswordChar = true;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl.ForeColor = System.Drawing.Color.White;
            this.lbl.Location = new System.Drawing.Point(14, 25);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(92, 27);
            this.lbl.TabIndex = 52;
            this.lbl.Text = "原密码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 27);
            this.label1.TabIndex = 52;
            this.label1.Text = "新密码：";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNewPassword.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtNewPassword.ForeColor = System.Drawing.Color.SeaGreen;
            this.txtNewPassword.Location = new System.Drawing.Point(102, 73);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(233, 32);
            this.txtNewPassword.TabIndex = 1;
            this.txtNewPassword.Text = "123";
            this.txtNewPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PassWordEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(360, 201);
            this.Controls.Add(this.btnIsCancel);
            this.Controls.Add(this.btnIsOk);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOldPassword);
            this.Controls.Add(this.lbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PassWordEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PassWordEdit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinButton btnIsCancel;
        private CCWin.SkinControl.SkinButton btnIsOk;
        private System.Windows.Forms.TextBox txtOldPassword;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNewPassword;

    }
}