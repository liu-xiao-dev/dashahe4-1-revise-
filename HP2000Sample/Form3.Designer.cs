namespace WaterMonitoring
{
    partial class Form3
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_chlorophyll = new System.Windows.Forms.TextBox();
            this.textBox_DO = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_temperature = new System.Windows.Forms.TextBox();
            this.textBox_Conductivity = new System.Windows.Forms.TextBox();
            this.textBox_PH = new System.Windows.Forms.TextBox();
            this.btn_Addr = new System.Windows.Forms.Button();
            this.btn_OpenPort = new System.Windows.Forms.Button();
            this.btn_PHoffsetcorrect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_PHcorrect = new System.Windows.Forms.Button();
            this.textBox_PHcorrect = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_signal = new System.Windows.Forms.TextBox();
            this.btn_sendcorre = new System.Windows.Forms.Button();
            this.btn_TemperatureCorrect = new System.Windows.Forms.Button();
            this.btn_ConductOffsetCorrect = new System.Windows.Forms.Button();
            this.textBox_TemperatureCorrect = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_ConductCorrect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_ConductCorrect = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox_DOcorrect = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox1.Controls.Add(this.textBox_chlorophyll);
            this.groupBox1.Controls.Add(this.textBox_DO);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBox_temperature);
            this.groupBox1.Controls.Add(this.textBox_Conductivity);
            this.groupBox1.Controls.Add(this.textBox_PH);
            this.groupBox1.Controls.Add(this.btn_Addr);
            this.groupBox1.Controls.Add(this.btn_OpenPort);
            this.groupBox1.Controls.Add(this.btn_PHoffsetcorrect);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btn_PHcorrect);
            this.groupBox1.Controls.Add(this.textBox_PHcorrect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 304);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PH校准";
            // 
            // textBox_chlorophyll
            // 
            this.textBox_chlorophyll.Location = new System.Drawing.Point(113, 201);
            this.textBox_chlorophyll.Name = "textBox_chlorophyll";
            this.textBox_chlorophyll.Size = new System.Drawing.Size(73, 23);
            this.textBox_chlorophyll.TabIndex = 13;
            // 
            // textBox_DO
            // 
            this.textBox_DO.Location = new System.Drawing.Point(113, 173);
            this.textBox_DO.Name = "textBox_DO";
            this.textBox_DO.Size = new System.Drawing.Size(76, 23);
            this.textBox_DO.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(113, 227);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "采集";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // textBox_temperature
            // 
            this.textBox_temperature.Location = new System.Drawing.Point(113, 146);
            this.textBox_temperature.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_temperature.Name = "textBox_temperature";
            this.textBox_temperature.Size = new System.Drawing.Size(76, 23);
            this.textBox_temperature.TabIndex = 10;
            // 
            // textBox_Conductivity
            // 
            this.textBox_Conductivity.Location = new System.Drawing.Point(113, 122);
            this.textBox_Conductivity.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Conductivity.Name = "textBox_Conductivity";
            this.textBox_Conductivity.Size = new System.Drawing.Size(76, 23);
            this.textBox_Conductivity.TabIndex = 9;
            // 
            // textBox_PH
            // 
            this.textBox_PH.Location = new System.Drawing.Point(113, 94);
            this.textBox_PH.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_PH.Name = "textBox_PH";
            this.textBox_PH.Size = new System.Drawing.Size(76, 23);
            this.textBox_PH.TabIndex = 8;
            // 
            // btn_Addr
            // 
            this.btn_Addr.Location = new System.Drawing.Point(10, 263);
            this.btn_Addr.Name = "btn_Addr";
            this.btn_Addr.Size = new System.Drawing.Size(91, 23);
            this.btn_Addr.TabIndex = 7;
            this.btn_Addr.Text = "修正从机位址";
            this.btn_Addr.UseVisualStyleBackColor = true;
            this.btn_Addr.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_OpenPort
            // 
            this.btn_OpenPort.Location = new System.Drawing.Point(11, 227);
            this.btn_OpenPort.Name = "btn_OpenPort";
            this.btn_OpenPort.Size = new System.Drawing.Size(75, 23);
            this.btn_OpenPort.TabIndex = 6;
            this.btn_OpenPort.Text = "打开串口";
            this.btn_OpenPort.UseVisualStyleBackColor = true;
            this.btn_OpenPort.Click += new System.EventHandler(this.btn_OpenPort_Click);
            // 
            // btn_PHoffsetcorrect
            // 
            this.btn_PHoffsetcorrect.Location = new System.Drawing.Point(11, 79);
            this.btn_PHoffsetcorrect.Name = "btn_PHoffsetcorrect";
            this.btn_PHoffsetcorrect.Size = new System.Drawing.Size(91, 23);
            this.btn_PHoffsetcorrect.TabIndex = 5;
            this.btn_PHoffsetcorrect.Text = "PH偏移校正";
            this.btn_PHoffsetcorrect.UseVisualStyleBackColor = true;
            this.btn_PHoffsetcorrect.Click += new System.EventHandler(this.btn_PHoffsetcorrect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "方法二：有标准液";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "方法一：无标准液或已装好";
            // 
            // btn_PHcorrect
            // 
            this.btn_PHcorrect.Location = new System.Drawing.Point(9, 159);
            this.btn_PHcorrect.Name = "btn_PHcorrect";
            this.btn_PHcorrect.Size = new System.Drawing.Size(99, 23);
            this.btn_PHcorrect.TabIndex = 2;
            this.btn_PHcorrect.Text = "PH两点校正法";
            this.btn_PHcorrect.UseVisualStyleBackColor = true;
            this.btn_PHcorrect.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_PHcorrect
            // 
            this.textBox_PHcorrect.Location = new System.Drawing.Point(102, 50);
            this.textBox_PHcorrect.Name = "textBox_PHcorrect";
            this.textBox_PHcorrect.Size = new System.Drawing.Size(84, 23);
            this.textBox_PHcorrect.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "PH偏移修正值：";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textBox_signal);
            this.groupBox2.Controls.Add(this.btn_sendcorre);
            this.groupBox2.Controls.Add(this.btn_TemperatureCorrect);
            this.groupBox2.Controls.Add(this.btn_ConductOffsetCorrect);
            this.groupBox2.Controls.Add(this.textBox_TemperatureCorrect);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btn_ConductCorrect);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_ConductCorrect);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(197, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 304);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "电导率校准";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 125);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(128, 17);
            this.label11.TabIndex = 12;
            this.label11.Text = "稳定的电导率信号值：";
            // 
            // textBox_signal
            // 
            this.textBox_signal.Location = new System.Drawing.Point(128, 121);
            this.textBox_signal.Name = "textBox_signal";
            this.textBox_signal.Size = new System.Drawing.Size(79, 23);
            this.textBox_signal.TabIndex = 11;
            // 
            // btn_sendcorre
            // 
            this.btn_sendcorre.Location = new System.Drawing.Point(6, 144);
            this.btn_sendcorre.Name = "btn_sendcorre";
            this.btn_sendcorre.Size = new System.Drawing.Size(131, 23);
            this.btn_sendcorre.TabIndex = 10;
            this.btn_sendcorre.Text = "电导率电极系数获取";
            this.btn_sendcorre.UseVisualStyleBackColor = true;
            this.btn_sendcorre.Click += new System.EventHandler(this.btn_sendcorre_Click);
            // 
            // btn_TemperatureCorrect
            // 
            this.btn_TemperatureCorrect.Location = new System.Drawing.Point(11, 263);
            this.btn_TemperatureCorrect.Name = "btn_TemperatureCorrect";
            this.btn_TemperatureCorrect.Size = new System.Drawing.Size(98, 23);
            this.btn_TemperatureCorrect.TabIndex = 9;
            this.btn_TemperatureCorrect.Text = "温度偏移校正";
            this.btn_TemperatureCorrect.UseVisualStyleBackColor = true;
            this.btn_TemperatureCorrect.Click += new System.EventHandler(this.btn_TemperatureCorrect_Click);
            // 
            // btn_ConductOffsetCorrect
            // 
            this.btn_ConductOffsetCorrect.Location = new System.Drawing.Point(11, 79);
            this.btn_ConductOffsetCorrect.Name = "btn_ConductOffsetCorrect";
            this.btn_ConductOffsetCorrect.Size = new System.Drawing.Size(103, 23);
            this.btn_ConductOffsetCorrect.TabIndex = 8;
            this.btn_ConductOffsetCorrect.Text = "电导率偏移校正";
            this.btn_ConductOffsetCorrect.UseVisualStyleBackColor = true;
            this.btn_ConductOffsetCorrect.Click += new System.EventHandler(this.btn_ConductOffsetCorrect_Click);
            // 
            // textBox_TemperatureCorrect
            // 
            this.textBox_TemperatureCorrect.Location = new System.Drawing.Point(113, 224);
            this.textBox_TemperatureCorrect.Name = "textBox_TemperatureCorrect";
            this.textBox_TemperatureCorrect.Size = new System.Drawing.Size(87, 23);
            this.textBox_TemperatureCorrect.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 230);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "温度偏移修正值：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 5;
            this.label7.Text = "温度校正";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "方法一：";
            // 
            // btn_ConductCorrect
            // 
            this.btn_ConductCorrect.Location = new System.Drawing.Point(9, 173);
            this.btn_ConductCorrect.Name = "btn_ConductCorrect";
            this.btn_ConductCorrect.Size = new System.Drawing.Size(100, 23);
            this.btn_ConductCorrect.TabIndex = 3;
            this.btn_ConductCorrect.Text = "电极系数校正";
            this.btn_ConductCorrect.UseVisualStyleBackColor = true;
            this.btn_ConductCorrect.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "方法二：电极系数法";
            // 
            // textBox_ConductCorrect
            // 
            this.textBox_ConductCorrect.Location = new System.Drawing.Point(120, 50);
            this.textBox_ConductCorrect.Name = "textBox_ConductCorrect";
            this.textBox_ConductCorrect.Size = new System.Drawing.Size(80, 23);
            this.textBox_ConductCorrect.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "电导率偏移修正值:";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 313);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(612, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.textBox_DOcorrect);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(409, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 302);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "溶解氧校准";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "溶解氧偏移校正";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // textBox_DOcorrect
            // 
            this.textBox_DOcorrect.Location = new System.Drawing.Point(117, 48);
            this.textBox_DOcorrect.Name = "textBox_DOcorrect";
            this.textBox_DOcorrect.Size = new System.Drawing.Size(80, 23);
            this.textBox_DOcorrect.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 17);
            this.label10.TabIndex = 1;
            this.label10.Text = "溶解氧偏移修正值：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "方法一：";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 335);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_PHcorrect;
        private System.Windows.Forms.TextBox textBox_PHcorrect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_TemperatureCorrect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_ConductCorrect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_ConductCorrect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_PHoffsetcorrect;
        private System.Windows.Forms.Button btn_TemperatureCorrect;
        private System.Windows.Forms.Button btn_ConductOffsetCorrect;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button btn_OpenPort;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btn_Addr;
        private System.Windows.Forms.Button btn_sendcorre;
        private System.Windows.Forms.TextBox textBox_signal;
        private System.Windows.Forms.TextBox textBox_temperature;
        private System.Windows.Forms.TextBox textBox_Conductivity;
        private System.Windows.Forms.TextBox textBox_PH;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox_DOcorrect;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_DO;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_chlorophyll;
    }
}