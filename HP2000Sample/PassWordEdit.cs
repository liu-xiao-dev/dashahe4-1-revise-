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
    public partial class PassWordEdit : Form
    {
        public string oldPassWord = "";
        public string newPassWord = "";
        public PassWordEdit()
        {
            InitializeComponent();
        }

        private void btnIsOk_Click(object sender, EventArgs e)
        {
            if (txtOldPassword.Text.Trim() != oldPassWord)
            {
                MessageBoxFrm frm = new MessageBoxFrm();
                frm.setMsgContent("原密码输入错误，请重新输入！");
                frm.ShowDialog();
            }
            else
            {
                if(txtNewPassword.Text.Trim()=="")
                {
                    MessageBoxFrm frm = new MessageBoxFrm();
                    frm.setMsgContent("输入的新密码是空的，请重新输入！");
                    frm.ShowDialog();
                }
                else
                {
                    this.newPassWord = txtNewPassword.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
           
        }

        private void btnIsCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
