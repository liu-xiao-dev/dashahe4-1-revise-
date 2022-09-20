using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterMonitoring
{
    public partial class mask : Form
    {
        
        public mask()
        {
            InitializeComponent();
           
        }

        private void mask_Load(object sender, EventArgs e)
        {
          
            this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
            this.WindowState = FormWindowState.Maximized;    //最大化窗体 
        }


    }
}
