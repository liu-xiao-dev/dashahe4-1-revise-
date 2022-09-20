using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WaterMonitoring
{   
    public partial class MSGShow : Form
    {
        private object lockObj = new object();
        public bool timerstarted = false;
        public MSGShow()
        {
            InitializeComponent();
        }

        private void MSGShow_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
            this.StartPosition = FormStartPosition.CenterScreen;
           // this.TransparencyKey = Color.White;
           // this.BackColor = Color.White;
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            lock (lockObj)
            {
                timerstarted = false;
                this.Visible=false;
            }           
            
           
        }

        private void msgBoard_VisibleChanged(object sender, EventArgs e)
        {
            //Thread.Sleep(200);
            if (timerstarted == false)
            {
                if (this.Visible == true)
                {
                    lock (lockObj)
                    {
                        timerstarted = true;
                    }
                    timer1.Start();
                }
            }
        }

      
    }
}
