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
    public partial class setLocalAddressDlg : Form
    {
        public string newLocalAddressName = "";
        public string newLocalAddressCode = "";
        public setLocalAddressDlg()
        {
            InitializeComponent();
        }

        private void btnIsOk_Click(object sender, EventArgs e)
        {
            if (localAddressName.Text.Trim() == "")
            {
                MessageBoxFrm frm = new MessageBoxFrm();
                frm.setMsgContent("输入的地址是空的，请重新输入！");
                frm.ShowDialog();
            }
            else
            {
                this.newLocalAddressName = localAddressName.Text;
                this.newLocalAddressCode = localAddressCode.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        public void setLocalAddress(string code ,string name)
        {
            localAddressName.Text = name;
            localAddressCode.Text = code;
        }
    }
}
