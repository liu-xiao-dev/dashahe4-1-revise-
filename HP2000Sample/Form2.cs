using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace WaterMonitoring
{
    
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
            timerAuth.Interval = 100000;//10s一次
        }

        private int errorConut = 0; //错误累计
        private int maxError = 3; //最大错误次数
        int AuthCodeErrorFlag = 0;

        /*
        public static string encode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 8);
            }
            return htext;
        }

        public static string decode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
        */
        public  string encode(string str)//编码
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);

        }


        public string decode(string str)//解码
        {
            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.Default.GetString(bytes);

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string inputtxt = txtAuthcode.Text;//输入授权码

            string decodetxt = decode(inputtxt);//解码
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (decodetxt == ni.GetPhysicalAddress().ToString())
                {
                    errorConut = 0;
                    AuthCodeErrorFlag = 1;
                    string AuthCodePath = @"C:\authcode.txt";//写入C盘没有权限?
                    try
                    {
                        File.WriteAllText(AuthCodePath, inputtxt, Encoding.Default);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            if (AuthCodeErrorFlag==0)
            {
                MessageBoxFrm frm = new MessageBoxFrm();
                errorConut++;
                if (errorConut >= maxError)
                {
 //                   timerAuth.Enabled = true;//定时器开启
                    btnOk.Enabled = false;//禁用确认按钮
 //                   frm.setMsgContent("操作频繁，请稍后再试！");
 //                   frm.ShowDialog();
                }
                else
                {
                    frm.setMsgContent("授权码错误，请重新输入！");
                    frm.ShowDialog();

                }
                return;               
            }
                   
         }
       
        private void btnCancel_Click(object sender, EventArgs e)
        {
          //this.Close();
            Application.Exit();

        }

        private void timerAuth_Tick(object sender, EventArgs e)//定时器开启状态时，每60s自动触发的事件
        {
            timerAuth.Enabled = false;
            btnOk.Enabled = true;
            errorConut = 0;

        }


    }
}
