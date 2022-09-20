using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterMonitoring
{
    public partial class UDiskCopyDlg : Form
    {       
        public UDiskCopyDlg()
        {
            InitializeComponent();
        }

        private void btnIsOk_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        public void SetTextContent(string descryption,string uname)
        {
            uDiskDiscryption.Text = descryption;
            uDiskName.Text = uname;
        }

        private void CopyData2UDisk_Click(object sender, EventArgs e)
        {
            CopyDirectory(@"C:\测试数据", uDiskName.Text);
        }

        private void ClearComputerData_Click(object sender, EventArgs e)
        {
            MessageBoxFrm mf = new MessageBoxFrm();
            mf.setMsgContent("你确定要把本机存的数据全部删除掉吗？");
            DialogResult result = mf.ShowDialog();
            if(result==DialogResult.OK)
            {
                if (Directory.Exists(@"C:\测试数据"))
                {
                    DeleteDir(@"C:\测试数据");
                }
            }
        }

        public static void DeleteDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
                MessageBoxFrm mf = new MessageBoxFrm();
                mf.setMsgContent("本机所存数据已经全部清空！");
                mf.ShowDialog();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = desdir + "\\" +"123";

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }


                    try
                    {
                        File.Copy(file, srcfileName);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                }
            }//foreach 
            //MessageBox.Show("数据拷贝完毕！");
            MessageBoxFrm mf = new MessageBoxFrm();
            mf.setMsgContent("数据拷贝完毕！");
            mf.ShowDialog();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            CopyDirectory(@"C:\SaveData", uDiskName.Text);
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            MessageBoxFrm mf = new MessageBoxFrm();
            mf.setMsgContent("你确定要把本机存的光谱数据全部删除掉吗？");
            DialogResult result = mf.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Directory.Exists(@"C:\SaveData"))
                {
                    DeleteDir(@"C:\SaveData");
                }
            }
        }
    }
}
