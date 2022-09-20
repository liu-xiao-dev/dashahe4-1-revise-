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
    public partial class PassWord : Form
    {
        public string psw = "";
        public PassWord()
        {
            InitializeComponent();
        }

        private void btnIsOk_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != psw)
            {
                MessageBoxFrm frm = new MessageBoxFrm();
                frm.setMsgContent("密码错误，请重新输入！");
                frm.ShowDialog();
                return;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnIsCancel_Click(object sender, EventArgs e)
        {
          //  this.Close();
        }

        private void btnEditPW_Click(object sender, EventArgs e)
        {
           // this.Close();
        }
    }
}
