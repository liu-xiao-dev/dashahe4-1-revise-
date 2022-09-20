using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Timers;
using System.IO.Ports;

namespace WaterMonitoring
{
    public partial class Form3 : Form
    {
        //Form1 form1 = new Form1()
        private float fPH_offset = 0;
        private float fConductivity_offset = 0;
         private float fTemperature_offset = 0;
         private float fDO_offset = 0;
         private float fCorre = 0;
         public float[] fReceiveData;
         private string redata3;
         public string redata4;
         private string redata5;
         private string redata6;
         private string redata7;
        private dynamic Form1 ;
        private string strfile_error = @"errorlog.txt";
        private string strfile_test = @"test.txt";
        private string strfile_corre = @"corre.txt";
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        System.Timers.Timer timer2;//抓取串口数据
        //定义委托
        public delegate void SetControlValue(string strValue);
        private int currentCount1 = 0;//串口
        public Form3()
        {
            InitializeComponent();
        }
        //public Form3(Form1 f1)
        //{
        //    InitializeComponent();
        //    form1 = f1;
        //}

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = "电极校正";
        }
        //PH偏移校正
        private void btn_PHoffsetcorrect_Click(object sender, EventArgs e)
        {
            //Form1.fPH_offset = float.Parse(textBox_PHcorrect.Text.Trim());//传值
            fPH_offset = float.Parse(textBox_PHcorrect.Text.Trim());
            send_directive_PHoffset(null,null);

        }
        //PH两点校正
        private void button1_Click(object sender, EventArgs e)
        {
            send_directive_PHtwoPt1(null,null);
            Thread.Sleep(2000);
            send_directive_PHtwoPt2(null,null);
        }
        //电导率偏移校正
        private void btn_ConductOffsetCorrect_Click(object sender, EventArgs e)
        {
            //Form1.fConductivity_offset = float.Parse(textBox_ConductCorrect.Text.Trim());//传值
            fConductivity_offset = float.Parse(textBox_ConductCorrect.Text.Trim());            
            send_directive_ConductOffset(null,null);
        }
        //电导率电极系数校正
        private void button2_Click(object sender, EventArgs e)
        {
            send_directive_correctcorre(null,null);
        }            
        //温度偏移校正
        private void btn_TemperatureCorrect_Click(object sender, EventArgs e)
        {
            //Form1.fTemperature_offset = float.Parse(textBox_TemperatureCorrect.Text.Trim());//传值
            fTemperature_offset = float.Parse(textBox_TemperatureCorrect.Text.Trim());
            send_directive_TemperatureOffset(null,null);
        }
        //打开串口
        private void OpenSerialPort(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)//未打开
            {
                string strCOM = "COM3";//
                serialPort1.PortName = strCOM;
                //波特率
                int iBaudRate = 9600; string strBaudRate = iBaudRate.ToString();
                serialPort1.BaudRate = Convert.ToInt32(strBaudRate, 10);
                //停止位
                serialPort1.StopBits = StopBits.One;
                //设置数据位
                int iDataBits = 8; string strDataBits = iDataBits.ToString();
                serialPort1.DataBits = Convert.ToInt32(strDataBits);
                //设置奇偶校验位,偶
                serialPort1.Parity = Parity.None;
                //流控制
                serialPort1.Handshake = Handshake.None;
                serialPort1.ReceivedBytesThreshold = 8;
                try
                {
                    serialPort1.Open();     //打开串口
                    toolStripStatusLabel1.Text = "串口已打开";
                    btn_OpenPort.Text = "关闭串口";
                }
                catch (Exception ex)
                {
                    try
                    {
                        //日志记录
                        WriteErrorLog(ex);                       
                    }
                    catch (Exception ex1)
                    {
                    }                    
                }
            }
            else//如果串口是打开的则将其关闭
            {
                serialPort1.Close();
                timer2.Stop();
                btn_OpenPort.Text = "打开串口";
                toolStripStatusLabel1.Text = "串口已关闭";
            }
        }
        private void WriteErrorLog(Exception ex)
        {
            try
            {
                LogWriteLock.EnterWriteLock();
                using (StreamWriter sw_error = File.AppendText(strfile_error))
                {
                    sw_error.WriteLine("出现应用程序未处理的异常：" + System.DateTime.Now.ToString());
                    sw_error.WriteLine("异常信息：" + ex.Message);
                    sw_error.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                    sw_error.WriteLine("异常对象：" + ex.Source);
                    sw_error.WriteLine("触发方法：" + ex.TargetSite);
                    sw_error.WriteLine("---------------------------------------------------------");
                    sw_error.Close();
                }
            }
            catch (Exception ex1)
            {
            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }
        }
        //打开串口
        private void btn_OpenPort_Click(object sender, EventArgs e)
        {
            //在C盘建立文件夹
            string path = @"C:\测试数据";
            DirectoryInfo dir = new DirectoryInfo(path);
            dir.Create();
           
            OpenSerialPort(null,null);
        }
        //修改从机位址
        private void button1_Click_1(object sender, EventArgs e)
        {
            send_directive_condtAddr(null, null);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(450);
                if (serialPort1.BytesToRead == 8)//偏移修正值的返回
                {
                    //Thread.Sleep(1000);
                    byte[] receiveData = new byte[8];
                    serialPort1.Read(receiveData, 0, receiveData.Length);
                      using(StreamWriter sw1=File.AppendText(strfile_test))
                      {
                          for (int i = 0; i < receiveData.Length;i++ )
                          {
                              sw1.Write("{0}",receiveData[i]);
                          }
                          sw1.Write("\r\n\r\n");
                          sw1.Close();
                      }
                    receiveData = null;
                    GC.Collect();                                                         
                    serialPort1.DiscardInBuffer();
                }
                else if (serialPort1.BytesToRead == 9)//返回修正的电极系数 //返回叶绿素的测量值
                {
                    byte[] receiveData = new byte[9];
                    serialPort1.Read(receiveData, 0, receiveData.Length);
                    string strRcv = null;
                    //using (StreamWriter sw2 = File.AppendText(strfile))
                    //{
                    //    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次接收字节完成且字节完整");
                    //    sw2.Close();
                    //}
                    int Temp = receiveData.Length - 1;
                    for (int i = 0; i < Temp; i++) //窗体显示 
                    {
                        string temp_ = receiveData[i].ToString("X2") + " ";
                        strRcv += temp_; //16进制显示  
                    }

                    //需要校验的字符串
                    int correctlength = receiveData.Length - 3;
                    string strCorrect = null;
                    for (int i = 0; i < correctlength; i++) //窗体显示 
                    {
                        string temp10 = receiveData[i].ToString("X2") + " ";
                        strCorrect += temp10; //16进制显示  
                    }
                    string temp11 = receiveData[6].ToString("X2");
                    strCorrect += temp11;
                    CRCcal(strCorrect, out redata3);//
                    string temp01 = redata3.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int j = 0; j < temp05.Length; j++)
                    {
                        string temp08 = temp06[j].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    //redata3 = temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    redata3 = temp07.Trim();

                    string temp12 = receiveData[7].ToString("X2") + " ";
                    string temp_1 = receiveData[8].ToString("X2");
                    string compare = temp12 + temp_1;//最后两个字节的字符串
                    strRcv += temp_1;
                    string temp4 = strRcv;//去空白
                    string temp5 = temp4.Replace(",", " ");//去英文逗号
                    string temp6 = temp5.Replace("，", " ");//去中文逗号
                    string temp7 = temp6.Replace("0x", " ");//去0x
                    temp7.Replace("0X", " ");//去0X
                    string[] temp8 = temp7.Split(' ');

                    
                    //byte[] byte_corre = new byte[4]; //电极系数
                    byte[] byte_chlorophyll = new byte[4];
                    for (int j = 3; j < 7; j++)
                    {
                        //byte_corre[j - 3] = receiveData[j];//电极系数
                        byte_chlorophyll[j - 3] = receiveData[j];
                    }
                    ////交换电极系数后两个字节
                    //byte tempcorre1 = byte_corre[0];
                    //byte_corre[0] = byte_corre[1];
                    //byte_corre[1] = tempcorre1;
                    //byte tempcorre2 = byte_corre[2];
                    //byte_corre[2] = byte_corre[3];
                    //byte_corre[3] = tempcorre2;                                        
                    //float fCorre = BitConverter.ToSingle(byte_corre, 0);

                    //交换溶解氧测量值后两个字节
                    byte tempDO1 = byte_chlorophyll[0];
                    byte_chlorophyll[0] = byte_chlorophyll[1];
                    byte_chlorophyll[1] = tempDO1;
                    byte tempDO2 = byte_chlorophyll[2];
                    byte_chlorophyll[2] = byte_chlorophyll[3];
                    byte_chlorophyll[3] = tempDO2;
                    float fChlorophy = BitConverter.ToSingle(byte_chlorophyll, 0); 
                    //if (compare == redata3)  //电极系数
                    //{
                    //    SaveData(fCorre);                  
                    //}
                    if (compare == redata3) //溶解氧
                    {
                        SaveData5(fChlorophy);
                    }           
                    receiveData = null;
                    GC.Collect();
                    strRcv = null;
                    GC.Collect();
                    //byte_corre = null;//电极系数
                    //GC.Collect();
                    byte_chlorophyll = null;//溶解氧
                    GC.Collect();
                   

                    serialPort1.DiscardInBuffer();
                }
                else if (serialPort1.BytesToRead == 13)//PH的读取数据返回
                {
                    //Thread.Sleep(1000);
                    byte[] receiveData = new byte[13];

                    serialPort1.Read(receiveData, 0, receiveData.Length);
                    string strRcv = null;
                    //using (StreamWriter sw2 = File.AppendText(strfile))
                    //{
                    //    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次接收字节完成且字节完整");
                    //    sw2.Close();
                    //}
                    int Temp = receiveData.Length - 1;
                    for (int i = 0; i < Temp; i++) //窗体显示 
                    {
                        string temp_ = receiveData[i].ToString("X2") + " ";
                        strRcv += temp_; //16进制显示  
                    }

                    //需要校验的字符串
                    int correctlength = receiveData.Length - 3;
                    string strCorrect = null;
                    for (int i = 0; i < correctlength; i++) //窗体显示 
                    {
                        string temp10 = receiveData[i].ToString("X2") + " ";
                        strCorrect += temp10; //16进制显示  
                    }
                    string temp11 = receiveData[10].ToString("X2");
                    strCorrect += temp11;
                    CRCcal(strCorrect, out redata3);//
                    string temp01 = redata3.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int j = 0; j < temp05.Length; j++)
                    {
                        string temp08 = temp06[j].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    //redata3 = temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    redata3 = temp07.Trim();

                    string temp12 = receiveData[11].ToString("X2") + " ";
                    string temp_1 = receiveData[12].ToString("X2");
                    string compare = temp12 + temp_1;//最后两个字节的字符串
                    strRcv += temp_1;
                    string temp4 = strRcv;//去空白
                    string temp5 = temp4.Replace(",", " ");//去英文逗号
                    string temp6 = temp5.Replace("，", " ");//去中文逗号
                    string temp7 = temp6.Replace("0x", " ");//去0x
                    temp7.Replace("0X", " ");//去0X
                    string[] temp8 = temp7.Split(' ');

                    //iByteCount = temp8.Length;
                    //绘图
                    ////////////byte_ReceiveData = new byte[temp8.Length];//
                    ////////////for (int i = 0; i < iByteCount; i++) //窗体显示 
                    ////////////{
                    ////////////    byte_ReceiveData[i] = Convert.ToByte(temp8[i], 16);//
                    ////////////}
                    byte[] byte_temperature = new byte[4]; byte[] byte_PH = new byte[4];
                    for (int j = 3; j < 7; j++)
                    {
                        byte_PH[j - 3] = receiveData[j];
                        byte_temperature[j - 3] = receiveData[j + 4];
                    }
                    //交换wendu后两个字节
                    byte temptemperature1 = byte_temperature[0];
                    byte_temperature[0] = byte_temperature[1];
                    byte_temperature[1] = temptemperature1;
                    byte temptemperature2 = byte_temperature[2];
                    byte_temperature[2] = byte_temperature[3];
                    byte_temperature[3] = temptemperature2;
                    //交换PH后两个字节
                    byte tempPH_ = byte_PH[0];
                    byte_PH[0] = byte_PH[1];
                    byte_PH[1] = tempPH_;
                    byte tempPH = byte_PH[2];
                    byte_PH[2] = byte_PH[3];
                    byte_PH[3] = tempPH;
                    fReceiveData = new float[2];//
                    fReceiveData[0] = BitConverter.ToSingle(byte_PH, 0);
                    fReceiveData[1] = BitConverter.ToSingle(byte_temperature, 0);
                    if (compare == redata3)
                    {
                        SaveData(fReceiveData);
                        //iCount2++;
                        //////using (StreamWriter sw2 = File.AppendText(strfile))
                        //////{
                        //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次接收字节完整通过校验保存数据完成");
                        //////    sw2.Close();
                        //////}
                    }
                    //SaveData(fReceiveData);                
                    receiveData = null;
                    GC.Collect();
                    strRcv = null;
                    GC.Collect();
                    byte_temperature = null;
                    GC.Collect();
                    byte_PH = null;
                    GC.Collect();

                    //////////byte_ReceiveData = null;
                    //////////GC.Collect();
                    fReceiveData = null;
                    GC.Collect();
                    serialPort1.DiscardInBuffer();
                }
                else if (serialPort1.BytesToRead == 17)//电导率的读取数据返回
                {
                    //Thread.Sleep(1000);
                    byte[] receiveData = new byte[17];

                    serialPort1.Read(receiveData, 0, receiveData.Length);
                    string strRcv = null;
                    //using (StreamWriter sw2 = File.AppendText(strfile))
                    //{
                    //    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "电导率与温度串口第" + iCount3.ToString() + "次接收字节完成且字节完整");
                    //    sw2.Close();
                    //}
                    int Temp = receiveData.Length - 1;
                    for (int i = 0; i < Temp; i++) //窗体显示 
                    {
                        string temp_ = receiveData[i].ToString("X2") + " ";
                        strRcv += temp_; //16进制显示  
                    }

                    //需要校验的字符串
                    int correctlength = receiveData.Length - 3;
                    string strCorrect = null;
                    for (int i = 0; i < correctlength; i++) //窗体显示 
                    {
                        string temp10 = receiveData[i].ToString("X2") + " ";
                        strCorrect += temp10; //16进制显示  
                    }
                    string temp11 = receiveData[14].ToString("X2");
                    strCorrect += temp11;
                    CRCcal(strCorrect, out redata4);
                    string temp01 = redata4.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int j = 0; j < temp05.Length; j++)
                    {
                        string temp08 = temp06[j].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    //redata4 = temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    redata4 = temp07.Trim();

                    string temp12 = receiveData[15].ToString("X2") + " ";

                    string temp_1 = receiveData[16].ToString("X2");
                    string compare = temp12 + temp_1;//最后两个字节的字符串
                    strRcv += temp_1;
                    string temp4 = strRcv;//去空白
                    string temp5 = temp4.Replace(",", " ");//去英文逗号
                    string temp6 = temp5.Replace("，", " ");//去中文逗号
                    string temp7 = temp6.Replace("0x", " ");//去0x
                    temp7.Replace("0X", " ");//去0X
                    string[] temp8 = temp7.Split(' ');


                    //iByteCount = temp8.Length;
                    //绘图
                    //////byte_ReceiveData = new byte[temp8.Length];//
                    //////for (int i = 0; i < iByteCount; i++) //窗体显示 
                    //////{
                    //////    byte_ReceiveData[i] = Convert.ToByte(temp8[i], 16);//
                    //////}
                    byte[] byte_conductivity = new byte[4]; byte[] byte_temperature = new byte[4];
                    byte[] byte_conductsignal = new byte[4];
                    for (int j = 3; j < 7; j++)
                    {
                        byte_conductivity[j - 3] = receiveData[j];//改动
                        byte_temperature[j - 3] = receiveData[j + 4];
                        byte_conductsignal[j - 3] = receiveData[j + 8];//
                    }
                    //交换电导率第一二个字节，第三四个字节
                    byte tempcondt1 = byte_conductivity[0];
                    byte_conductivity[0] = byte_conductivity[1];
                    byte_conductivity[1] = tempcondt1;
                    byte tempcondt2 = byte_conductivity[2];
                    byte_conductivity[2] = byte_conductivity[3];
                    byte_conductivity[3] = tempcondt2;
                    //交换温度第一二个字节，第三四个字节
                    byte temptemperature1 = byte_temperature[0];
                    byte_temperature[0] = byte_temperature[1];
                    byte_temperature[1] = temptemperature1;
                    byte temptemperature2 = byte_temperature[2];
                    byte_temperature[2] = byte_temperature[3];
                    byte_temperature[3] = temptemperature2;
                    //交换电导率信号第一二个字节，第三四个字节
                    byte signal1 = byte_conductsignal[0];
                    byte_conductsignal[0] = byte_conductsignal[1];
                    byte_conductsignal[1] = signal1;
                    byte signal2 = byte_conductsignal[2];
                    byte_conductsignal[2] = byte_conductsignal[3];
                    byte_conductsignal[3] = signal2;
                    fReceiveData = new float[3];//
                    fReceiveData[0] = BitConverter.ToSingle(byte_conductivity, 0);
                    fReceiveData[1] = BitConverter.ToSingle(byte_temperature, 0);
                    fReceiveData[2] = BitConverter.ToSingle(byte_conductsignal, 0);
                    if (compare == redata4)
                    {
                        SaveData2(fReceiveData);
                        //iCount3++;
                        //////using (StreamWriter sw2 = File.AppendText(strfile))
                        //////{
                        //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "电导率与温度串口第" + iCount3.ToString() + "次接收完整通过校验保存数据完成");
                        //////    sw2.Close();
                        //////}
                    }
                    //SaveData(fReceiveData);                
                    receiveData = null;
                    GC.Collect();
                    strRcv = null;
                    GC.Collect();
                    byte_conductivity = null;
                    GC.Collect();
                    byte_temperature = null;
                    GC.Collect();
                    byte_conductsignal = null;
                    GC.Collect();

                    ////////byte_ReceiveData = null;
                    ////////GC.Collect();
                    fReceiveData = null;
                    GC.Collect();
                    serialPort1.DiscardInBuffer();
                }
                else if (serialPort1.BytesToRead == 21) //返回溶解氧的测量值
                {
                    byte[] receiveData = new byte[21];
                    serialPort1.Read(receiveData, 0, receiveData.Length);
                    string strRcv = null;
                    //using (StreamWriter sw2 = File.AppendText(strfile))
                    //{
                    //    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次接收字节完成且字节完整");
                    //    sw2.Close();
                    //}
                    int Temp = receiveData.Length - 1;
                    for (int i = 0; i < Temp; i++) //窗体显示 
                    {
                        string temp_ = receiveData[i].ToString("X2") + " ";
                        strRcv += temp_; //16进制显示  
                    }

                    //需要校验的字符串
                    int correctlength = receiveData.Length - 3;
                    string strCorrect = null;
                    for (int i = 0; i < correctlength; i++) //窗体显示 
                    {
                        string temp10 = receiveData[i].ToString("X2") + " ";
                        strCorrect += temp10; //16进制显示  
                    }
                    string temp11 = receiveData[18].ToString("X2");
                    strCorrect += temp11;
                    CRCcal(strCorrect, out redata3);//
                    string temp01 = redata3.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int j = 0; j < temp05.Length; j++)
                    {
                        string temp08 = temp06[j].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    //redata3 = temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    redata3 = temp07.Trim();

                    string temp12 = receiveData[19].ToString("X2") + " ";
                    string temp_1 = receiveData[20].ToString("X2");
                    string compare = temp12 + temp_1;//最后两个字节的字符串
                    strRcv += temp_1;
                    string temp4 = strRcv;//去空白
                    string temp5 = temp4.Replace(",", " ");//去英文逗号
                    string temp6 = temp5.Replace("，", " ");//去中文逗号
                    string temp7 = temp6.Replace("0x", " ");//去0x
                    temp7.Replace("0X", " ");//去0X
                    string[] temp8 = temp7.Split(' ');


                    //byte[] byte_corre = new byte[4]; //电极系数
                    byte[] byte_DO = new byte[4];
                    for (int j = 3; j < 7; j++)
                    {
                        //byte_corre[j - 3] = receiveData[j];//电极系数
                        byte_DO[j - 3] = receiveData[j];
                    }
                    ////交换电极系数后两个字节
                    //byte tempcorre1 = byte_corre[0];
                    //byte_corre[0] = byte_corre[1];
                    //byte_corre[1] = tempcorre1;
                    //byte tempcorre2 = byte_corre[2];
                    //byte_corre[2] = byte_corre[3];
                    //byte_corre[3] = tempcorre2;                                        
                    //float fCorre = BitConverter.ToSingle(byte_corre, 0);

                    //交换溶解氧测量值后两个字节
                    byte tempDO1 = byte_DO[0];
                    byte_DO[0] = byte_DO[1];
                    byte_DO[1] = tempDO1;
                    byte tempDO2 = byte_DO[2];
                    byte_DO[2] = byte_DO[3];
                    byte_DO[3] = tempDO2;
                    float fDO = BitConverter.ToSingle(byte_DO, 0);
                    //if (compare == redata3)  //电极系数
                    //{
                    //    SaveData(fCorre);                  
                    //}
                    if (compare == redata3) //溶解氧
                    {
                        SaveData4(fDO);
                    }
                    receiveData = null;
                    GC.Collect();
                    strRcv = null;
                    GC.Collect();
                    //byte_corre = null;//电极系数
                    //GC.Collect();
                    byte_DO = null;//溶解氧
                    GC.Collect();


                    serialPort1.DiscardInBuffer();
                }
            
                else
                {
                    toolStripStatusLabel1.Text = "字节数返回有误";

                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    
                }
                catch (Exception ex1)
                {
                }
            }      
        }
        private void SaveData(float fReceiveData)//保存电极系数
        {
            try
            {                
                FileStream fs = new FileStream(strfile_corre, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(System.DateTime.Now.ToString()+fReceiveData);                
                sw.Close();
                fs.Close();               
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);                    
                }
                catch (Exception ex1)
                {
                }
            }

        }
        private void SaveData(float[] fReceiveData)
        {
            try
            {
                string fileName1 = @"C:\测试数据\" + "PixelData_PH.txt";
                FileStream fs = new FileStream(fileName1, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < 2; i++)
                {
                    sw.WriteLine(fReceiveData[i]);
                }
                sw.Close();
                fs.Close();

                //System.DateTime currentTime = new System.DateTime();
                //currentTime = System.DateTime.Now;
                //string time1 = currentTime.ToLongDateString();
                //string filename2 = @"C:\测试数据\" + "PHandCL" + time1 + ".txt";
                //string time2 = "时分秒" + " " + currentTime.ToString("HH") + " " + currentTime.ToString("mm") + " " + currentTime.ToString("ss");
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档                
                //    using (StreamWriter sw1 = File.AppendText(filename2))
                //    {
                //        sw1.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                    + fReceiveData[0].ToString("f4") + "," + fReceiveData[1].ToString("f4"));
                //        sw1.Close();
                //    }
                //}

                ////追加文档，不标注日期，超过24小时需清理
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档     
                //    string filename3 = @"C:\测试数据\" + "PHandCL" + ".txt";
                //    if (iCount2 % 7200 == 0)
                //    {
                //        FileStream fs1 = File.Open(filename3, FileMode.OpenOrCreate, FileAccess.Write);
                //        fs1.Seek(0, SeekOrigin.Begin);
                //        fs1.SetLength(0);
                //        fs1.Close();
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData[0].ToString("f4") + "," + fReceiveData[1].ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //    else
                //    {
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData[0].ToString("f4") + "," + fReceiveData[1].ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }

        }
        private void SaveData2(float[] fReceiveData)
        {
            try
            {
                string fileName1 = @"C:\测试数据\" + "PixelData_Conductivity.txt";
                FileStream fs = new FileStream(fileName1, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < 3; i++)
                {
                    sw.WriteLine(fReceiveData[i]);
                }
                sw.Close();
                fs.Close();

                //System.DateTime currentTime = new System.DateTime();
                //currentTime = System.DateTime.Now;
                //string time1 = currentTime.ToLongDateString();
                //string filename2 = @"C:\测试数据\" + "ConductandTemperature" + time1 + ".txt";
                //string time2 = "时分秒" + " " + currentTime.ToString("HH") + " " + currentTime.ToString("mm") + " " + currentTime.ToString("ss");
                //if (iCount3 % 50 == 0) //60,12
                //{
                //    //追加文档                
                //    using (StreamWriter sw1 = File.AppendText(filename2))
                //    {
                //        sw1.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                    + fReceiveData[0].ToString("f4") + "," + fReceiveData[1].ToString("f4") + "," + fReceiveData[2].ToString("f4"));
                //        sw1.Close();
                //    }
                //}

                ////追加文档，不标注日期，超过24小时需清理
                //if (iCount3 % 50 == 0) //60,12
                //{
                //    //追加文档     
                //    string filename3 = @"C:\测试数据\" + "ConductandTemperature" + ".txt";
                //    if (iCount3 % 7200 == 0)
                //    {
                //        FileStream fs1 = File.Open(filename3, FileMode.OpenOrCreate, FileAccess.Write);
                //        fs1.Seek(0, SeekOrigin.Begin);
                //        fs1.SetLength(0);
                //        fs1.Close();
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData[0].ToString("f1") + "," + fReceiveData[1].ToString("f1") + "," + fReceiveData[2].ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //    else
                //    {
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData[0].ToString("f1") + "," + fReceiveData[1].ToString("f1") + "," + fReceiveData[2].ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //读取余氯和PH值
        private void readCorre(out float fReceiveData)
        {
            fReceiveData = 0;
            try
            {                
                if (File.Exists(strfile_corre))
                {
                    FileStream fs = new FileStream(strfile_corre, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);                 
                    string line = "";
                    line = sr.ReadLine().Trim();
                    int i = 0;
                    while (line != null)
                    {

                        fReceiveData = float.Parse(line);
                        i++;
                        line = sr.ReadLine();
                    }                   
                    sr.Close();
                    fs.Close();                   
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);                    
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //校验函数
        public static void CRCcal(string data, out string redata3)
        {
            //try 
            //{
            string[] datas = data.Split(' ');
            List<byte> bytedata = new List<byte>();

            foreach (string str in datas)
            {
                bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }

            }
            string[] redata = new string[2];
            redata[1] = Convert.ToString((byte)((crc >> 8) & 0xff), 16);
            redata[0] = Convert.ToString((byte)((crc & 0xff)), 16);
            string redata2 = "";
            //return FormatHEX(redata[0]) + " " + FormatHEX(redata[1]);
            for (int i = 0; i < redata.Length; i++)
            {
                string temp61 = redata[i].ToUpper() + " ";
                redata2 += temp61;
            }
            redata3 = redata2.Trim();
            redata = null;
            //}
            //catch(Exception ex)
            //{
            //    WriteErrorLog(ex);
            //}           
        }
        //PH偏移校正
        public void send_directive_PHoffset(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {                    
                    serialPort1.DiscardOutBuffer();
                    //将偏移校正值转化
                    byte[] byte_PHoffset = BitConverter.GetBytes(fPH_offset);
                    byte temp1 = byte_PHoffset[0];
                    byte_PHoffset[0] = byte_PHoffset[1];
                    byte_PHoffset[1] = temp1;
                    byte temp2 = byte_PHoffset[2];
                    byte_PHoffset[2] = byte_PHoffset[3];
                    byte_PHoffset[3] = temp2;
                    string temp3 = "01 10 00 0C 00 02 04 ";
                    //int temp4 = byte_PHoffset.Length - 1;
                    string temp5 = null;
                    for (int i = 0; i < byte_PHoffset.Length; i++)
                    {
                        string temp6 = byte_PHoffset[i].ToString("X2") + " ";
                        temp5 += temp6;
                    }
                    //temp5+=byte_PHoffset[3].ToString("X2");
                    string temp7 = temp3 + temp5;
                    CRCcal(temp7, out redata5);
                    string temp01 = redata5.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int k = 0; k < temp05.Length; k++)
                    {
                        string temp08 = temp06[k].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    redata5 = temp07.Trim();
                    //redata5=temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    string strTemp = temp7 + redata5;

                    //string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "PH偏移校正已发送";
                    //iCount2++;                    
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";

                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    //异常次数超过阈值，程序复位
                   
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //PH两点校正:第一点
        public void send_directive_PHtwoPt1(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {                    
                    string strTemp = "01 10 00 2F 00 03 06 00 11 00 00 40 E0 1B 91";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "PH第一点校正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //PH两点校正:第二点
        public void send_directive_PHtwoPt2(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {                   
                    string strTemp = "01 10 00 32 00 03 06 00 21 51 EC 40 80 1A DB";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "PH第二点校正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开"; 
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //电导率偏移校正
        public void send_directive_ConductOffset(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    
                    serialPort1.DiscardOutBuffer();
                    //将偏移校正值转化
                    byte[] byte_Condtoffset = BitConverter.GetBytes(fConductivity_offset);
                    byte temp1 = byte_Condtoffset[0];
                    byte_Condtoffset[0] = byte_Condtoffset[1];
                    byte_Condtoffset[1] = temp1;
                    byte temp2 = byte_Condtoffset[2];
                    byte_Condtoffset[2] = byte_Condtoffset[3];
                    byte_Condtoffset[3] = temp2;
                    string temp3 = "01 10 00 0C 00 02 04 ";
                    //int temp4 = byte_PHoffset.Length - 1;
                    string temp5 = null;
                    for (int i = 0; i < byte_Condtoffset.Length; i++)
                    {
                        string temp6 = byte_Condtoffset[i].ToString("X2") + " ";
                        temp5 += temp6;
                    }
                    //temp5+=byte_PHoffset[3].ToString("X2");
                    string temp7 = temp3 + temp5;
                    CRCcal(temp7, out redata5);
                    string temp01 = redata5.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int k = 0; k < temp05.Length; k++)
                    {
                        string temp08 = temp06[k].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    redata5 = temp07.Trim();
                    //redata5=temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    string strTemp = temp7 + redata5;

                    //string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "电导率偏移校正校正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //温度偏移校正(电导率电极)
        public void send_directive_TemperatureOffset(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    
                    serialPort1.DiscardOutBuffer();
                    //将偏移校正值转化
                    byte[] byte_Tempoffset = BitConverter.GetBytes(fTemperature_offset);
                    byte temp1 = byte_Tempoffset[0];
                    byte_Tempoffset[0] = byte_Tempoffset[1];
                    byte_Tempoffset[1] = temp1;
                    byte temp2 = byte_Tempoffset[2];
                    byte_Tempoffset[2] = byte_Tempoffset[3];
                    byte_Tempoffset[3] = temp2;
                    string temp3 = "01 10 00 0E 00 02 04 ";
                    //int temp4 = byte_PHoffset.Length - 1;
                    string temp5 = null;
                    for (int i = 0; i < byte_Tempoffset.Length; i++)
                    {
                        string temp6 = byte_Tempoffset[i].ToString("X2") + " ";
                        temp5 += temp6;
                    }
                    //temp5+=byte_PHoffset[3].ToString("X2");
                    string temp7 = temp3 + temp5;
                    CRCcal(temp7, out redata5);
                    string temp01 = redata5.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int k = 0; k < temp05.Length; k++)
                    {
                        string temp08 = temp06[k].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    redata5 = temp07.Trim();
                    //redata5=temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    string strTemp = temp7 + redata5;

                    //string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "温度校正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                   
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //电极系数读取(电导率电极)
        public void send_directive_corre(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    
                    serialPort1.DiscardOutBuffer();

                    string strTemp = "01 03 00 14 00 02 84 0F";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "电极系数获取指令发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //电极系数发送
        //温度偏移校正(电导率电极)
        public void send_directive_correctcorre(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {

                    serialPort1.DiscardOutBuffer();
                    //将偏移校正值转化
                    float temp=float.Parse(textBox_signal.Text.Trim());
                    readCorre(out fCorre);
                    fCorre=1413*fCorre/temp;
                    byte[] byte_Tempoffset = BitConverter.GetBytes(fCorre);
                    byte temp1 = byte_Tempoffset[0];
                    byte_Tempoffset[0] = byte_Tempoffset[1];
                    byte_Tempoffset[1] = temp1;
                    byte temp2 = byte_Tempoffset[2];
                    byte_Tempoffset[2] = byte_Tempoffset[3];
                    byte_Tempoffset[3] = temp2;
                    string temp3 = "01 10 00 14 00 02 04 ";
                    //int temp4 = byte_PHoffset.Length - 1;
                    string temp5 = null;
                    for (int i = 0; i < byte_Tempoffset.Length; i++)
                    {
                        string temp6 = byte_Tempoffset[i].ToString("X2") + " ";
                        temp5 += temp6;
                    }
                    //temp5+=byte_PHoffset[3].ToString("X2");
                    string temp7 = temp3 + temp5;
                    CRCcal(temp7, out redata5);
                    string temp01 = redata5.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int k = 0; k < temp05.Length; k++)
                    {
                        string temp08 = temp06[k].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    redata5 = temp07.Trim();
                    //redata5=temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    string strTemp = temp7 + redata5;

                    //string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "电导率电极系数校正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);

                }
                catch (Exception ex1)
                {
                }
            }
        }
        //从机位址修正
        public void send_directive_condtAddr(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    
                    serialPort1.DiscardOutBuffer();

                    string strTemp = "01 06 00 2B 00 02 78 03";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    toolStripStatusLabel1.Text = "从机位址修正已发送";
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    OpenSerialPort(null, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    //异常次数超过阈值，程序复位
                    
                }
                catch (Exception ex1)
                {
                }
            }
        }

        private void btn_sendcorre_Click(object sender, EventArgs e)
        {
            send_directive_corre(null,null);
        }
        private void InitialTimer1(int interval)
        {
            //设置定时间隔(毫秒为单位)
            timer2 = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer2.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer2.Enabled = true;
            //绑定Elapsed事件
            timer2.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp1);
            timer2.Stop();
        }
        private void TimerUp1(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                currentCount1 += 1;
                this.Invoke(new SetControlValue(CaptureData1), currentCount1.ToString());
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        private void CaptureData1(string strValue)
        {
            Thread.Sleep(800);//850
            //iPort1Count = 0;
            send_directive(null, null);
            readClandPH(null, null);
            Thread.Sleep(1000);
            //iPort2Count = 0;
            send_directive2(null, null);
            readCondtandTemp(null, null);
            //Thread.Sleep(800);
            //iPort3Count = 0;
            //send_directive3(null, null);
            //readflux(null, null);
            Thread.Sleep(1000);
            send_directive4(null, null);
            readDO(null, null);
            Thread.Sleep(1000);
            send_directive5(null, null);
            readChlorophyll(null, null);
        }

        public void send_directive(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    //label_cl_value.ForeColor = Color.Black;
                    textBox_PH.ForeColor = Color.Black;
                    serialPort1.DiscardOutBuffer();
                    string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    //iCount2++;
                    //toolStripStatusLabel1.Text = "串口数据已发送";
                    //////using (StreamWriter sw2 = File.AppendText(strfile))
                    //////{
                    //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次发送字节完成");
                    //////    sw2.Close();
                    //////}
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    //label_cl_value.ForeColor = Color.Red;
                    textBox_PH.ForeColor = Color.Red;
                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //读取余氯和PH值
        private void readClandPH(object sender, EventArgs e)
        {
            try
            {
                string strFileName = @"C:\测试数据\" + "PixelData_PH.txt";
                if (File.Exists(strFileName))
                {
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    fReceiveData = new float[2];
                    string line = "";
                    line = sr.ReadLine().Trim();
                    int i = 0;
                    while (line != null)
                    {

                        fReceiveData[i] = float.Parse(line);
                        i++;
                        line = sr.ReadLine();
                    }
                    textBox_PH.Text = fReceiveData[0].ToString("f4");
                    //label_temperature_value.Text = fReceiveData[1].ToString("f1");
                    sr.Close();
                    fs.Close();

                    fReceiveData = null;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        public void send_directive2(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    textBox_temperature.ForeColor = Color.Black;
                    textBox_Conductivity.ForeColor = Color.Black;
                    serialPort1.DiscardOutBuffer();
                    string strTemp = "02 03 00 02 00 06 64 3B";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    //iCount3++;
                    //////using (StreamWriter sw2 = File.AppendText(strfile))
                    //////{
                    //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "电导率与温度串口第" + iCount3.ToString() + "次发送字节完成");
                    //////    sw2.Close();
                    //////}

                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel3.Text = "电导率与温度串口未打开";
                    textBox_temperature.ForeColor = Color.Red;
                    textBox_Conductivity.ForeColor = Color.Red;
                    //openConductivity();
                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //读取电导率值
        private void readCondtandTemp(object sender, EventArgs e)
        {
            try
            {
                string strFileName = @"C:\测试数据\" + "PixelData_Conductivity.txt";
                if (File.Exists(strFileName))
                {
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    fReceiveData = new float[3];
                    string line = "";
                    line = sr.ReadLine().Trim();
                    int i = 0;
                    while (line != null)
                    {

                        fReceiveData[i] = float.Parse(line);
                        i++;
                        line = sr.ReadLine();
                    }
                    textBox_Conductivity.Text = fReceiveData[0].ToString("f1");
                    textBox_temperature.Text = fReceiveData[1].ToString("f1");
                    sr.Close();
                    fs.Close();

                    fReceiveData = null;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            InitialTimer1(8000);
            if (serialPort1.IsOpen)
            {
                //Thread.Sleep(800);
                send_directive(null, null);
                readClandPH(null, null);
                Thread.Sleep(1000);                
                send_directive2(null, null);
                readCondtandTemp(null, null);
                Thread.Sleep(1000);
                send_directive4(null,null);
                readDO(null,null);
                Thread.Sleep(1000);
                send_directive5(null, null);
                readChlorophyll(null, null);
                timer2.Start();                
            }            
        }
        //获取溶解氧的测量值
        public void send_directive4(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    textBox_DO.ForeColor = Color.Black;
                    //label_PH_value.ForeColor = Color.Black;
                    serialPort1.DiscardOutBuffer();
                    string strTemp = "03 03 00 02 00 08 E4 2E";// 01 03 00 02 00 08 E5 CC
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    //iCount2++;
                    //toolStripStatusLabel1.Text = "串口数据已发送";
                    //////using (StreamWriter sw2 = File.AppendText(strfile))
                    //////{
                    //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次发送字节完成");
                    //////    sw2.Close();
                    //////}
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    textBox_DO.ForeColor = Color.Red;
                    //label_PH_value.ForeColor = Color.Red;
                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //溶解氧的偏移校正
        public void send_directive_DOoffset(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    textBox_DOcorrect.ForeColor = Color.Black;
                    //label_Conductivity_value.ForeColor = Color.Black;
                    serialPort1.DiscardOutBuffer();
                    //将偏移校正值转化
                    byte[] byte_Tempoffset = BitConverter.GetBytes(fDO_offset);
                    byte temp1 = byte_Tempoffset[0];
                    byte_Tempoffset[0] = byte_Tempoffset[1];
                    byte_Tempoffset[1] = temp1;
                    byte temp2 = byte_Tempoffset[2];
                    byte_Tempoffset[2] = byte_Tempoffset[3];
                    byte_Tempoffset[3] = temp2;
                    string temp3 = "01 10 00 0C 00 02 04 ";//03 10 00 0C 00 02 04
                    //int temp4 = byte_PHoffset.Length - 1;
                    string temp5 = null;
                    for (int i = 0; i < byte_Tempoffset.Length; i++)
                    {
                        string temp6 = byte_Tempoffset[i].ToString("X2") + " ";
                        temp5 += temp6;
                    }
                    //temp5+=byte_PHoffset[3].ToString("X2");
                    string temp7 = temp3 + temp5;
                    CRCcal(temp7, out redata5);
                    string temp01 = redata5.Trim();
                    string temp02 = temp01.Replace(',', ' ');
                    string temp03 = temp02.Replace('，', ' ');
                    string temp04 = temp03.Replace("0x", "");
                    temp04.Replace("0X", "");
                    string[] temp05 = temp04.Split(' ');
                    byte[] temp06 = new byte[temp05.Length];
                    for (int i = 0; i < temp05.Length; i++)
                    {
                        temp06[i] = Convert.ToByte(temp05[i], 16);
                    }

                    string temp07 = "";
                    for (int k = 0; k < temp05.Length; k++)
                    {
                        string temp08 = temp06[k].ToString("X2") + " ";
                        temp07 += temp08;
                    }
                    redata5 = temp07.Trim();
                    //redata5=temp06[0].ToString("X2") + " " + temp06[1].ToString("X2");
                    string strTemp = temp7 + redata5;

                    //string strTemp = "01 03 00 02 00 04 E5 C9";
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    //iCount2++;
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //MessageBox.Show("串口未打开");
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    textBox_DOcorrect.ForeColor = Color.Red;
                    //label_Conductivity_value.ForeColor = Color.Black;
                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //保存溶解氧
        private void SaveData4(float fReceiveData)
        {
            try
            {
                string fileName1 = @"C:\测试数据\" + "PixelData_DO.txt";
                FileStream fs = new FileStream(fileName1, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);    
                sw.WriteLine(fReceiveData);
                sw.Close();
                fs.Close();

                //System.DateTime currentTime = new System.DateTime();
                //currentTime = System.DateTime.Now;
                //string time1 = currentTime.ToLongDateString();
                //string filename2 = @"C:\测试数据\" + "DO" + time1 + ".txt";
                //string time2 = "时分秒" + " " + currentTime.ToString("HH") + " " + currentTime.ToString("mm") + " " + currentTime.ToString("ss");
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档                
                //    using (StreamWriter sw1 = File.AppendText(filename2))
                //    {
                //        sw1.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                    + fReceiveData.ToString("f4"));
                //        sw1.Close();
                //    }
                //}

                ////追加文档，不标注日期，超过24小时需清理
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档     
                //    string filename3 = @"C:\测试数据\" + "DO" + ".txt";
                //    if (iCount2 % 7200 == 0)
                //    {
                //        FileStream fs1 = File.Open(filename3, FileMode.OpenOrCreate, FileAccess.Write);
                //        fs1.Seek(0, SeekOrigin.Begin);
                //        fs1.SetLength(0);
                //        fs1.Close();
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData.ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //    else
                //    {
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData.ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //读取溶解氧
        private void readDO(object sender, EventArgs e)
        {
            try
            {
                string strFileName = @"C:\测试数据\" + "PixelData_DO.txt";
                if (File.Exists(strFileName))
                {
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    float fReceiveData = 0;
                    string line = "";
                    line = sr.ReadLine().Trim();
                    int i = 0;
                    while (line != null)
                    {

                        fReceiveData = float.Parse(line);
                        i++;
                        line = sr.ReadLine();
                    }
                    textBox_DO.Text = fReceiveData.ToString("f4");
                    //label_temperature_value.Text = fReceiveData[1].ToString("f1");
                    sr.Close();
                    fs.Close();

                    //fReceiveData = null;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            send_directive_DOoffset(null,null);
        }
        //获取叶绿素的测量值
        public void send_directive5(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)  //如果串口开启
                {
                    ////////label_cl_value.ForeColor = Color.Black;
                    ////////label_PH_value.ForeColor = Color.Black;
                    serialPort1.DiscardOutBuffer();
                    string strTemp = "0A 03 00 00 00 02 C5 70";//04 03 00 00 00 02 C4 5E
                    string sendBuf = strTemp;
                    string sendnoNull = sendBuf.Trim();
                    string sendNOComma = sendnoNull.Replace(',', ' ');//去掉英文逗号
                    string sendNOComma1 = sendNOComma.Replace('，', ' ');//去掉中文逗号
                    string sendNOComma2 = sendNOComma1.Replace("0x", "");//去掉0x
                    sendNOComma2.Replace("0X", "");//去掉0X
                    string[] strArray = sendNOComma2.Split(' ');
                    int byteBufferLength = strArray.Length;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
                        //int len = byteOfStr.Length;
                        int decNum = 0;
                        if (strArray[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //
                        }

                        byteBuffer[j] = Convert.ToByte(decNum); //

                        j++;
                    }
                    serialPort1.Write(byteBuffer, 0, byteBuffer.Length);
                    //iCount2++;
                    //toolStripStatusLabel1.Text = "串口数据已发送";
                    //////using (StreamWriter sw2 = File.AppendText(strfile))
                    //////{
                    //////    sw2.WriteLine(System.DateTime.Now.ToString() + "," + "余氯与PH串口第" + iCount2.ToString() + "次发送字节完成");
                    //////    sw2.Close();
                    //////}
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    
                    //////label_cl_value.ForeColor = Color.Red;
                    //////label_PH_value.ForeColor = Color.Red;
                    OpenSerialPort(null,null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        //保存叶绿素
        private void SaveData5(float fReceiveData)
        {
            try
            {
                string fileName1 = @"C:\测试数据\" + "PixelData_chlorophyll.txt";
                FileStream fs = new FileStream(fileName1, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);                
                sw.WriteLine(fReceiveData);               
                sw.Close();
                fs.Close();

                //System.DateTime currentTime = new System.DateTime();
                //currentTime = System.DateTime.Now;
                //string time1 = currentTime.ToLongDateString();
                //string filename2 = @"C:\测试数据\" + "DO" + time1 + ".txt";
                //string time2 = "时分秒" + " " + currentTime.ToString("HH") + " " + currentTime.ToString("mm") + " " + currentTime.ToString("ss");
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档                
                //    using (StreamWriter sw1 = File.AppendText(filename2))
                //    {
                //        sw1.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                    + fReceiveData.ToString("f4"));
                //        sw1.Close();
                //    }
                //}

                ////追加文档，不标注日期，超过24小时需清理
                //if (iCount2 % 50 == 0) //60,12
                //{
                //    //追加文档     
                //    string filename3 = @"C:\测试数据\" + "DO" + ".txt";
                //    if (iCount2 % 7200 == 0)
                //    {
                //        FileStream fs1 = File.Open(filename3, FileMode.OpenOrCreate, FileAccess.Write);
                //        fs1.Seek(0, SeekOrigin.Begin);
                //        fs1.SetLength(0);
                //        fs1.Close();
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData.ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //    else
                //    {
                //        using (StreamWriter sw2 = File.AppendText(filename3))
                //        {
                //            sw2.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                //                        + fReceiveData.ToString("f4"));
                //            sw2.Close();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }

        }
        //读取溶解氧
        private void readChlorophyll(object sender, EventArgs e)
        {
            try
            {
                string strFileName = @"C:\测试数据\" + "PixelData_chlorophyll.txt";
                if (File.Exists(strFileName))
                {
                    FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    float fReceiveData = 0;
                    string line = "";
                    line = sr.ReadLine().Trim();
                    int i = 0;
                    while (line != null)
                    {

                        fReceiveData = float.Parse(line);
                        i++;
                        line = sr.ReadLine();
                    }
                    textBox_chlorophyll.Text = fReceiveData.ToString("f4");
                    //label_temperature_value.Text = fReceiveData[1].ToString("f1");
                    sr.Close();
                    fs.Close();

                    //fReceiveData = null;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    ////异常次数超过阈值，程序复位
                    //if (iErrorTimes >= 20)
                    //{
                    //    restart++;
                    //    using (StreamWriter sw2 = File.AppendText(strfile))
                    //    {
                    //        sw2.WriteLine(System.DateTime.Now.ToString() + "," + "第" + restart.ToString() + "次设备重启");
                    //        sw2.Close();
                    //    }
                    //    CloseUI(null, null);
                    //    InitVariable();
                    //    btn_openDevice_Click(null, null);
                    //    button1_Click(null, null);
                    //    return;
                    //}
                }
                catch (Exception ex1)
                {
                }
            }
        }
        


        
        
    }
}
