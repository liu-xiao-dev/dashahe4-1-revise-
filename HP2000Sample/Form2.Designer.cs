namespace WaterMonitoring
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.label1 = new System.Windows.Forms.Label();
            this.txtAuthcode = new System.Windows.Forms.TextBox();
            this.btnCancel = new CCWin.SkinControl.SkinButton();
            this.btnOk = new CCWin.SkinControl.SkinButton();
            this.timerAuth = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "请输入授权码：";
            // 
            // txtAuthcode
            // 
            this.txtAuthcode.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAuthcode.Location = new System.Drawing.Point(19, 93);
            this.txtAuthcode.Multiline = true;
            this.txtAuthcode.Name = "txtAuthcode";
            this.txtAuthcode.Size = new System.Drawing.Size(347, 48);
            this.txtAuthcode.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnCancel.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.DownBack = null;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(213, 165);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.MouseBack = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormlBack = null;
            this.btnCancel.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnCancel.Size = new System.Drawing.Size(144, 50);
            this.btnCancel.TabIndex = 53;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnOk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnOk.DownBack = null;
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Location = new System.Drawing.Point(19, 165);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.MouseBack = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.NormlBack = null;
            this.btnOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnOk.Size = new System.Drawing.Size(144, 50);
            this.btnOk.TabIndex = 55;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // timerAuth
            // 
            this.timerAuth.Tick += new System.EventHandler(this.timerAuth_Tick);
            // 
            // Form2
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(390, 258);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtAuthcode);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Text = "获取权限";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAuthcode;
        private CCWin.SkinControl.SkinButton btnCancel;
        private CCWin.SkinControl.SkinButton btnOk;
        private System.Windows.Forms.Timer timerAuth;
    }
}