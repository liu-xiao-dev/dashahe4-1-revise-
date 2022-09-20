using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterMonitoring
{
    public partial class Form5 : Form
    {
        private Form1 f1;
        public Form5()
        {
            InitializeComponent();
        }
        public Form5(Form1 f1_)
        {
            InitializeComponent();
            this.f1 = f1_;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (textBox_password.Text.Trim() == "123")
            {               
                
                f1.Is_UIstatus_lock = false;
              //  f1.btn_StopCapture.Text = "界面锁定";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("您输入的密码有误，请重新输入！");
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }
    }
}
