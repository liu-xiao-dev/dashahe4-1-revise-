namespace WaterMonitoring
{
    partial class MSGShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSGShow));
            this.msgBoard = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // msgBoard
            // 
            this.msgBoard.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.msgBoard.ForeColor = System.Drawing.Color.White;
            this.msgBoard.Location = new System.Drawing.Point(-7, -2);
            this.msgBoard.Name = "msgBoard";
            this.msgBoard.Size = new System.Drawing.Size(542, 62);
            this.msgBoard.TabIndex = 0;
            this.msgBoard.Text = "信息输出";
            this.msgBoard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.msgBoard.VisibleChanged += new System.EventHandler(this.msgBoard_VisibleChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MSGShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(534, 59);
            this.Controls.Add(this.msgBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(250, 450);
            this.Name = "MSGShow";
            this.Opacity = 0.7D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MSGShow";
            this.Load += new System.EventHandler(this.MSGShow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label msgBoard;
        private System.Windows.Forms.Timer timer1;

    }
}