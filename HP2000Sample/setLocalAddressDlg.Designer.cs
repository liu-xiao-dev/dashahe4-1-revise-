namespace WaterMonitoring
{
    partial class setLocalAddressDlg
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
            this.btnIsCancel = new CCWin.SkinControl.SkinButton();
            this.btnIsOk = new CCWin.SkinControl.SkinButton();
            this.localAddressName = new System.Windows.Forms.TextBox();
            this.lbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.localAddressCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
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
            this.btnIsCancel.Location = new System.Drawing.Point(234, 151);
            this.btnIsCancel.MouseBack = null;
            this.btnIsCancel.Name = "btnIsCancel";
            this.btnIsCancel.NormlBack = null;
            this.btnIsCancel.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsCancel.Size = new System.Drawing.Size(218, 59);
            this.btnIsCancel.TabIndex = 53;
            this.btnIsCancel.Text = "取消";
            this.btnIsCancel.UseVisualStyleBackColor = false;
            // 
            // btnIsOk
            // 
            this.btnIsOk.BackColor = System.Drawing.Color.Transparent;
            this.btnIsOk.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnIsOk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnIsOk.DownBack = null;
            this.btnIsOk.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIsOk.ForeColor = System.Drawing.Color.White;
            this.btnIsOk.Location = new System.Drawing.Point(12, 151);
            this.btnIsOk.MouseBack = null;
            this.btnIsOk.Name = "btnIsOk";
            this.btnIsOk.NormlBack = null;
            this.btnIsOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsOk.Size = new System.Drawing.Size(218, 59);
            this.btnIsOk.TabIndex = 54;
            this.btnIsOk.Text = "确定";
            this.btnIsOk.UseVisualStyleBackColor = false;
            this.btnIsOk.Click += new System.EventHandler(this.btnIsOk_Click);
            // 
            // localAddressName
            // 
            this.localAddressName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.localAddressName.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.localAddressName.ForeColor = System.Drawing.Color.SeaGreen;
            this.localAddressName.Location = new System.Drawing.Point(148, 86);
            this.localAddressName.Name = "localAddressName";
            this.localAddressName.Size = new System.Drawing.Size(304, 32);
            this.localAddressName.TabIndex = 52;
            this.localAddressName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl.ForeColor = System.Drawing.Color.White;
            this.lbl.Location = new System.Drawing.Point(15, 19);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(132, 27);
            this.lbl.TabIndex = 51;
            this.lbl.Text = "本站点编号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 27);
            this.label1.TabIndex = 51;
            this.label1.Text = "本站点名称：";
            // 
            // localAddressCode
            // 
            this.localAddressCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.localAddressCode.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.localAddressCode.ForeColor = System.Drawing.Color.SeaGreen;
            this.localAddressCode.Location = new System.Drawing.Point(148, 19);
            this.localAddressCode.Name = "localAddressCode";
            this.localAddressCode.Size = new System.Drawing.Size(304, 32);
            this.localAddressCode.TabIndex = 52;
            this.localAddressCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(17, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(436, 20);
            this.label2.TabIndex = 51;
            this.label2.Text = "*只能输入从1开始的数字，数值对应了云服务器数据库中站点表 id 号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(17, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(435, 20);
            this.label3.TabIndex = 51;
            this.label3.Text = "*此处输入的站点名称，将会显示在本程序用户第一页面左上角的位置";
            // 
            // setLocalAddressDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(475, 230);
            this.Controls.Add(this.btnIsCancel);
            this.Controls.Add(this.btnIsOk);
            this.Controls.Add(this.localAddressCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.localAddressName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "setLocalAddressDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "setLocalAddressDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinButton btnIsCancel;
        private CCWin.SkinControl.SkinButton btnIsOk;
        private System.Windows.Forms.TextBox localAddressName;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox localAddressCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}