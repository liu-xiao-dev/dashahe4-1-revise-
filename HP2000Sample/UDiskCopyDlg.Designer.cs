namespace WaterMonitoring
{
    partial class UDiskCopyDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDiskCopyDlg));
            this.btnIsOk = new CCWin.SkinControl.SkinButton();
            this.CopyData2UDisk = new CCWin.SkinControl.SkinButton();
            this.ClearComputerData = new CCWin.SkinControl.SkinButton();
            this.uDiskDiscryption = new System.Windows.Forms.Label();
            this.uDiskName = new System.Windows.Forms.Label();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.skinButton2 = new CCWin.SkinControl.SkinButton();
            this.SuspendLayout();
            // 
            // btnIsOk
            // 
            this.btnIsOk.BackColor = System.Drawing.Color.Transparent;
            this.btnIsOk.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnIsOk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnIsOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnIsOk.DownBack = null;
            this.btnIsOk.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIsOk.ForeColor = System.Drawing.Color.White;
            this.btnIsOk.Location = new System.Drawing.Point(16, 247);
            this.btnIsOk.MouseBack = null;
            this.btnIsOk.Name = "btnIsOk";
            this.btnIsOk.NormlBack = null;
            this.btnIsOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsOk.Size = new System.Drawing.Size(492, 80);
            this.btnIsOk.TabIndex = 55;
            this.btnIsOk.Text = "关闭窗口";
            this.btnIsOk.UseVisualStyleBackColor = false;
            this.btnIsOk.Click += new System.EventHandler(this.btnIsOk_Click);
            // 
            // CopyData2UDisk
            // 
            this.CopyData2UDisk.BackColor = System.Drawing.Color.Transparent;
            this.CopyData2UDisk.BaseColor = System.Drawing.Color.DarkCyan;
            this.CopyData2UDisk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.CopyData2UDisk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopyData2UDisk.DownBack = null;
            this.CopyData2UDisk.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CopyData2UDisk.ForeColor = System.Drawing.Color.White;
            this.CopyData2UDisk.Location = new System.Drawing.Point(16, 57);
            this.CopyData2UDisk.MouseBack = null;
            this.CopyData2UDisk.Name = "CopyData2UDisk";
            this.CopyData2UDisk.NormlBack = null;
            this.CopyData2UDisk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.CopyData2UDisk.Size = new System.Drawing.Size(236, 86);
            this.CopyData2UDisk.TabIndex = 55;
            this.CopyData2UDisk.Text = "将结果数据考进U盘";
            this.CopyData2UDisk.UseVisualStyleBackColor = false;
            this.CopyData2UDisk.Click += new System.EventHandler(this.CopyData2UDisk_Click);
            // 
            // ClearComputerData
            // 
            this.ClearComputerData.BackColor = System.Drawing.Color.Transparent;
            this.ClearComputerData.BaseColor = System.Drawing.Color.DarkCyan;
            this.ClearComputerData.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.ClearComputerData.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ClearComputerData.DownBack = null;
            this.ClearComputerData.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ClearComputerData.ForeColor = System.Drawing.Color.White;
            this.ClearComputerData.Location = new System.Drawing.Point(264, 57);
            this.ClearComputerData.MouseBack = null;
            this.ClearComputerData.Name = "ClearComputerData";
            this.ClearComputerData.NormlBack = null;
            this.ClearComputerData.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.ClearComputerData.Size = new System.Drawing.Size(244, 86);
            this.ClearComputerData.TabIndex = 54;
            this.ClearComputerData.Text = "清除电脑中结果数据";
            this.ClearComputerData.UseVisualStyleBackColor = false;
            this.ClearComputerData.Click += new System.EventHandler(this.ClearComputerData_Click);
            // 
            // uDiskDiscryption
            // 
            this.uDiskDiscryption.AutoSize = true;
            this.uDiskDiscryption.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uDiskDiscryption.ForeColor = System.Drawing.Color.White;
            this.uDiskDiscryption.Location = new System.Drawing.Point(133, 17);
            this.uDiskDiscryption.Name = "uDiskDiscryption";
            this.uDiskDiscryption.Size = new System.Drawing.Size(208, 27);
            this.uDiskDiscryption.TabIndex = 56;
            this.uDiskDiscryption.Text = "U盘已插入，盘符是：";
            // 
            // uDiskName
            // 
            this.uDiskName.AutoSize = true;
            this.uDiskName.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uDiskName.ForeColor = System.Drawing.Color.White;
            this.uDiskName.Location = new System.Drawing.Point(347, 17);
            this.uDiskName.Name = "uDiskName";
            this.uDiskName.Size = new System.Drawing.Size(23, 27);
            this.uDiskName.TabIndex = 56;
            this.uDiskName.Text = "F";
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.BaseColor = System.Drawing.Color.DarkCyan;
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.skinButton1.DownBack = null;
            this.skinButton1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton1.ForeColor = System.Drawing.Color.White;
            this.skinButton1.Location = new System.Drawing.Point(16, 152);
            this.skinButton1.MouseBack = null;
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinButton1.Size = new System.Drawing.Size(236, 86);
            this.skinButton1.TabIndex = 55;
            this.skinButton1.Text = "将光谱数据考进U盘";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // skinButton2
            // 
            this.skinButton2.BackColor = System.Drawing.Color.Transparent;
            this.skinButton2.BaseColor = System.Drawing.Color.DarkCyan;
            this.skinButton2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.skinButton2.DownBack = null;
            this.skinButton2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton2.ForeColor = System.Drawing.Color.White;
            this.skinButton2.Location = new System.Drawing.Point(264, 152);
            this.skinButton2.MouseBack = null;
            this.skinButton2.Name = "skinButton2";
            this.skinButton2.NormlBack = null;
            this.skinButton2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinButton2.Size = new System.Drawing.Size(244, 86);
            this.skinButton2.TabIndex = 54;
            this.skinButton2.Text = "清除电脑中光谱数据";
            this.skinButton2.UseVisualStyleBackColor = false;
            this.skinButton2.Click += new System.EventHandler(this.skinButton2_Click);
            // 
            // UDiskCopyDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(527, 346);
            this.Controls.Add(this.uDiskName);
            this.Controls.Add(this.uDiskDiscryption);
            this.Controls.Add(this.skinButton2);
            this.Controls.Add(this.skinButton1);
            this.Controls.Add(this.ClearComputerData);
            this.Controls.Add(this.CopyData2UDisk);
            this.Controls.Add(this.btnIsOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UDiskCopyDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDiskCopyDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinButton btnIsOk;
        private CCWin.SkinControl.SkinButton CopyData2UDisk;
        private CCWin.SkinControl.SkinButton ClearComputerData;
        private System.Windows.Forms.Label uDiskDiscryption;
        private System.Windows.Forms.Label uDiskName;
        private CCWin.SkinControl.SkinButton skinButton1;
        private CCWin.SkinControl.SkinButton skinButton2;
    }
}