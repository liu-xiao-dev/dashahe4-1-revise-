namespace WaterMonitoring
{
    partial class MessageBoxFrm
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
            this.btnIsOk = new CCWin.SkinControl.SkinButton();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.msgContet = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnIsOk
            // 
            this.btnIsOk.BackColor = System.Drawing.Color.Transparent;
            this.btnIsOk.BaseColor = System.Drawing.Color.DarkCyan;
            this.btnIsOk.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnIsOk.DownBack = null;
            this.btnIsOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnIsOk.ForeColor = System.Drawing.Color.White;
            this.btnIsOk.Location = new System.Drawing.Point(54, 90);
            this.btnIsOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnIsOk.MouseBack = null;
            this.btnIsOk.Name = "btnIsOk";
            this.btnIsOk.NormlBack = null;
            this.btnIsOk.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnIsOk.Size = new System.Drawing.Size(118, 56);
            this.btnIsOk.TabIndex = 57;
            this.btnIsOk.Text = "确定";
            this.btnIsOk.UseVisualStyleBackColor = false;
            this.btnIsOk.Click += new System.EventHandler(this.btnIsOk_Click);
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.BaseColor = System.Drawing.Color.DarkCyan;
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DownBack = null;
            this.skinButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton1.ForeColor = System.Drawing.Color.White;
            this.skinButton1.Location = new System.Drawing.Point(200, 90);
            this.skinButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.skinButton1.MouseBack = null;
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinButton1.Size = new System.Drawing.Size(118, 56);
            this.skinButton1.TabIndex = 57;
            this.skinButton1.Text = "取消";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // msgContet
            // 
            this.msgContet.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.msgContet.ForeColor = System.Drawing.Color.White;
            this.msgContet.Location = new System.Drawing.Point(-1, -1);
            this.msgContet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.msgContet.Name = "msgContet";
            this.msgContet.Size = new System.Drawing.Size(374, 73);
            this.msgContet.TabIndex = 59;
            this.msgContet.Text = "信息输出";
            this.msgContet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageBoxFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(374, 180);
            this.Controls.Add(this.msgContet);
            this.Controls.Add(this.skinButton1);
            this.Controls.Add(this.btnIsOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MessageBoxFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageBoxFrm";
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinButton btnIsOk;
        private CCWin.SkinControl.SkinButton skinButton1;
        public System.Windows.Forms.Label msgContet;
    }
}