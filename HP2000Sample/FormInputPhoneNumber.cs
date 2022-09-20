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
    public partial class FormInputPhoneNumber : Form
    {
        public string phonenum = "";
        public FormInputPhoneNumber()
        {
            InitializeComponent();
        }

        private void FormInputPhoneNumber_Load(object sender, EventArgs e)
        {
            textBoxPhoneNum.Text = phonenum;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            phonenum = textBoxPhoneNum.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
