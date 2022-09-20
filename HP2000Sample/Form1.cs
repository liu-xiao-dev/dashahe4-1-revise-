using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Timers;
using System.IO.Ports;
using System.Diagnostics;
using Microsoft.Win32;
using NSMatrix;
using CCWin;


//-------------------
using Hweny.TabSlider.TabSlideControl;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

//向保存A路、B路、AB路线程传递参数是所用到的结构体--------
struct saveStruct               
        {
            public string routeAorB;
            public UInt16[] value;
        }
//向保存传感器数值线程传递参数是所用到的结构体--------
struct saveValueStruct               
{
    public string title;
    public float value;
}
//向保存A路、B路、AB路线程传递参数是所用到的结构体--------
struct uv254Struct
{
    public UInt16[] valueA;
    public UInt16[] valueB;
}
namespace WaterMonitoring
{
    public partial class Form1 : Form
    {


        float value1 = 0f; float value2 = 0f; float value3 = 0f; float value4 = 0f; float value5 = 0f; float value6 = 0f; float value7 = 0f; float value8 = 0f; float value9 = 0f;//测试用，等删

        string[] savedHistoryFileTitle = { "COD", "NH4N", "SC", "Conductivity", "NH4N_1" };//COD 氨氮 悬浮物 电导率 氨氮(箱涵)
        int currentUnitIndex = 0;//配合上面的savedHistoryFileTitle，表示当前历史记录显示的是哪个单元的内容1:COD、2:氨氮、3:SC、4:电导率

        private int mTabCurrentIndex = 0;                       //用于指示目前的tab页处于哪个位置。
        private object lockObj = new object();
        static AutoResetEvent mAutoQuestOverEvent = new AutoResetEvent(false);          //用于通知本次查询结果已经返回

        static AutoResetEvent mServerSendOverEvent = new AutoResetEvent(false);//用于通知本次服务器查询已经返回结果


        public bool bAutoCaptureSensorData = false;                //是否开机自动连续采集传感器数据
        public bool bAutoOpenPLCCOM = false;                       //是否开机自动打开PLC端口
        public bool bAutoSendData2Cloud = false;                  //是否自动开启上传到云端
        public bool IsConnectedFlag = false;                      //成功连接我方平台的标志位


        public static string mCloudServerIPAddress = "tcp.dtuip.com";   //云端服务器的IP地址
        public static string mCloudServerPortNum = "6647";               //云端服务器上传的端口号
        public static string mSerialNumber = "";                         //云端服务器上传的序列号
        public static string HeartBeat = "Q";



        //INI文件配置所需代码----------------------------------------------------------------------------------------------------------------------
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);                             //write函数为写ini文件 参数依次为字段、键名（变量名）、键值（变量值）、路径
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);//get函数为读ini文件
        string INIfilePathName = @"C:\LOG\Config.ini";//保存ini数据的文件名称
        //------------------------------------------------------------------------------------------------------------------------------------------
        [DllImport("User32.dll")]//控制鼠标位置
        private static extern bool SetCursorPos(int x, int y);
        [DllImport("wininet.dll")]//检查网络是否连接
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        
        private SlideableTabControl slideTab;    //利用SlideableTabControl动态链接库定义滑动tab控件

     //  HP2000_wrapper.HP2000Wrapper wrapper = new HP2000_wrapper.HP2000Wrapper();

       static int setreceivelength = 8198;

       private List<byte> mReceiveSensorBufferPool = new List<byte>(4096);                      //传感器接收数据的缓冲池
       private List<byte> mReceiveSpectrumBufferPool = new List<byte>(setreceivelength * 5);    //传感器接收数据的缓冲池
       private List<byte> mReceivePLCBufferPool = new List<byte>(4096);                         //PLC接收数据的缓冲池

       private List<DateTime>   _24HourCurveTime            = new List<DateTime>(4096);                 //存放24小时曲线的时间轴
       private List<float>      _24HourCurveCOD             = new List<float>(4096);                    //存放24小时曲线的COD
       private List<float>      _24HourCurveNH4_N           = new List<float>(4096);                    //存放24小时曲线的NH4-N
       private List<float>      _24HourCurveSC              = new List<float>(4096);                    //存放24小时曲线的SC悬浮物
       private List<float>      _24HourCurveConductivity    = new List<float>(4096);                    //存放24小时曲线的电导率
       private List<float>      _24HourCurveNH4_N_1         = new List<float>(4096);                    //存放24小时曲线的NH4-N_1


       float _24ValueNH4_N              = 0f;           //存放每次采集到的NH4-N数据
       float _24ValueCOD                = 0f;           //存放每次采集到的COD数据
       float _24ValueSC                 = 0f;           //存放每次采集到的悬浮物SC数据
       float _24ValueConductivity       = 0f;           //存放每次采集到的电导率数据
       float _24ValueNH4_N_1            = 0f;           //存放每次采集到的NH4-N_1数据


       byte[] receivedOneFrameSpectrumData;                  //用于记录每次回传的数据（包括光谱数据以及帧头帧尾数据在内）的内存
       UInt16[] receiveDataA;               //用于记录A路光谱数据的内存
       UInt16[] receiveDataB;               //用于记录B路光谱数据的内存
       int pixelNUM = 4096;                 //光谱数据的个数是4096个
       bool QuerySpectrumDoubleRouteStop =false;   //用于关闭双路查询的线程的变量
       bool QuerySpectrumSingleRouteStop = false;  //用于关闭单路查询的线程的变量
       bool QuerySpeedStop = false;  //用于关闭单路查询的线程的变量
       bool QuerySensorStop = false;               //用于关闭查询传感器的线程的变量
       bool QueryUv254 = false;

       int currentCaptureRoute = 1;         //目前接收的光路 1为A路 2为B路
       int showWhitchRoute = 0;             //显示哪路光路，1为显示A路、2为显示B路、3为显示A-B路、4为同时显示A路和B路，0为两路都显示，其中1-4位双路连续采集模式，0是单路连续采集模式
    //   int receiveLength = 0;               //用于记录光谱串口每次接收到的数据长度  
       bool bTranslateToFluxSpeed = false;  //是否将流速的DN值转换成流速数值(L/min),true:转化(显示L/min的流速值)；false:不转化(直接显示DN值) 
       float fluxSpeedK = 1;                //流速转换的斜率K(y=kx+b)
       float fluxSpeedB = 0;                //流速转换的b(y=kx+b)
     
       bool  bCorrectuv254 = false;          //是否校正UV254；
       float correctValueuv254B = 0f;       //单点偏移校正，因此只有B没有K

       bool bCorrectChlorophyll = false;           //叶绿素Chlorophyll是否校正；
       float correctValueChlorophyllK = 0f;         //叶绿素的矫正系数K
       float correctValueChlorophyllB = 0f;         //叶绿素的矫正系数B

       bool  bCorrectDO = false;           //溶解氧D0是否校正；
       float correctValueDOK = 0f;         //溶解氧D0的矫正系数K
       float correctValueDOB = 0f;         //溶解氧D0的矫正系数B

       bool  bCorrectNH4_N = false;           //NH4-N是否校正；
       float correctValueNH4_NK = 0f;         //NH4-N的矫正系数K
       float correctValueNH4_NB = 0f;         //NH4-N的矫正系数B

       bool  bCorrectCOD = false;            //是否校正COD；
       float correctValueCODK = 0f;         //COD的矫正系数K
       float correctValueCODB = 0f;         //COD的矫正系数B

       bool  bCorrectTOC = false;           //TOC是否校正；
       float correctValueTOCK = 0f;         //TOC的矫正系数K
       float correctValueTOCB = 0f;         //TOC的矫正系数B

       bool  bCorrectCroma = false;           //是否校正色度；
       float correctValueCromaK = 0f;         //色度的矫正系数K
       float correctValueCromaB = 0f;         //色度的矫正系数B

       bool  bCorrectNO3_N = false;           //NO3-N是否校正；
       float correctValueNO3_NK = 0f;         //NO3-N的矫正系数K
       float correctValueNO3_NB = 0f;         //NO3-N的矫正系数B

       bool  bCorrectNO2_N = false;           //NO2-N是否校正；
       float correctValueNO2_NK = 0f;         //NO2-N的矫正系数K
       float correctValueNO2_NB = 0f;         //NO2-N的矫正系数B

       bool  bCorrectPO4_P= false;            //PO4-P是否校正；
       float correctValuePO4_PK = 0f;         //PO4-P的矫正系数K
       float correctValuePO4_PB = 0f;         //PO4-P的矫正系数B

       bool  bCorrectBOD = false;           //BOD是否校正；
       float correctValueBODK = 0f;         //BOD矫正系数K
       float correctValueBODB = 0f;         //BOD矫正系数B

      
       int iIntervalSpectrum = 12000;       //光谱仪查询间隔
       int iIntervalSpeed =    12000;           //流速查询间隔
       int iIntervalSensor =   12000;         //传感器查询间隔

       float mSpecifiedSpeedValue=0;          //指定流速的数值

       //U盘插入所需要的信号量--------------------------------------
       public const int WM_DEVICECHANGE = 0x219;
       public const int DBT_DEVICEARRIVAL = 0x8000;
       public const int DBT_CONFIGCHANGECANCELED = 0x0019;
       public const int DBT_CONFIGCHANGED = 0x0018;
       public const int DBT_CUSTOMEVENT = 0x8006;
       public const int DBT_DEVICEQUERYREMOVE = 0x8001;
       public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
       public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
       public const int DBT_DEVICEREMOVEPENDING = 0x8003;
       public const int DBT_DEVICETYPESPECIFIC = 0x8005;
       public const int DBT_DEVNODES_CHANGED = 0x0007;
       public const int DBT_QUERYCHANGECONFIG = 0x0017;
       public const int DBT_USERDEFINED = 0xFFFF;
       //----------------------------------------------------------

       public string mLocalAddressName = "";    //本机用户界面左上角对应显示的名称
       public string mLocalAddressCode = "1";   //本机的站点地址，此处的站点地址从1开始，对应了数据库中的站点表的id号


       private static bool Is_Device_Open = false;
       public static int iIntegrationTime = 10;//积分时间
      
       public static float fPH_offset = 0;
       public static float fConductivity_offset = 0;
       public static float fTemperature_offset = 0;
       public static float fDO_offset = 0;
       public static int CCD_DATA_PACK_SIZE = 2048;
       public static float[] spctrum_power;            //最终光谱数据
       public static float[] wavelength;
       public static float[] show_spectrum_power ;  //专业模式显示
       public static float[] Water_Spctrum_Data;  //水质数据（归一化用）
       private float[]       Water_WaveLength;
       private string        strWaterDataFile = @"0_waterdataparameter.txt";
       public  bool          m_bInMSGMode = false;

       private ulong currentCount = 0;//光谱仪
       private ulong currentCount1 = 0;//串口
       private int restart = 0;//重启次数
       private ulong restartPC = 0;//重启电脑次数
       private float[] fParameter;
       public float[] fReceiveData;
       public string redata3;
       public string redata4;
       public string redata5;//PH偏移校验
       public string redata6;//电导率
       public string redata7;//温度
       //定义Timer类
       System.Timers.Timer timer;                   //抓取光谱仪的串口数据---已经改成定时读取10分钟存储值-10分钟一次上传
       System.Timers.Timer timer1;                  //定时更新右上角时间1s更新一次
       System.Timers.Timer timer2ShowMask;          //定时20分钟后打开mask
       System.Timers.Timer timer3ShowSpecifiedSpeed;//定时修改指定流速进行小范围波动界面自定义间隔
       System.Timers.Timer timer4SendData2Cloud;    //定时用于定时上传数据至云端
       System.Timers.Timer timer5Heartbeat;         //定时用于定时发送心跳包
       System.Timers.Timer timer6ReconnectSocket;   //定时用于重新连接socket
       //System.Timers.Timer timer2;//抓取传感器的串口数据
       //System.Timers.Timer timer3;//专区流速传感器的串口数据
       long timer2Count = 0;       //用于记录
       long timer0Count = 0;
       long timer3Count = 0;

       MSGShow msgShowFrm;//主界面下面的灰色信息显示面板
       mask maskFrm;
       UDiskCopyDlg uDiskFrm;
       bool bMaskFrmisVisible = false; 
       //三个很重要的委托函数--------------------------------------------------------------------------
       //定义委托
       public delegate void SetControlValue(string strValue);           //信息提示委托
       //定义委托
       public delegate void UpdateValueDelegate(int index, float value);//PH值等界面显示数据的更新委托
       //定义委托
       public delegate void UpdateSpectrumCurveDelegate(int length, float[] fxParaData, double[] fyParaData);//光谱曲线绘图委托
       //定义委托
       public delegate void UpdateHistoryCurveDelegate(int length, float[] fxParaData, float[] fyParaData);//绘制历史数据委托
       //定义委托
       public delegate void Update24HoursCurveDelegate(int length, List<DateTime> fxParaData, List<float> fyParaData);//绘制24小时数据委托
       private int historyIndex;
       //定义委托
       public delegate void SetDarkOrLightDelegate(int dnValue);           //信息提示委托
       //定义委托
       public delegate void SetHistoryMessageDelegate(string left,DateTime start,DateTime end);           //信息提示委托
       public delegate void SetResidualDiskMessageDelegate(bool bfull);           //信息提示委托

       public static Socket socketSend;//设置一个属性，取属性的值时

       public string strFileName_historical;//历史文档
       public string strFileName_historical1;
       public int length;//光谱仪




       private int pageCount = 7;//总页数
       private int pageCurrent = 0;//当前页数

       private string strfile = @"log.txt";
       private string strfile_error = @"errorlog.txt";

       private int iErrorTimes = 0;
       public bool Is_UIstatus_lock = false;
       static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
       public string m_phonenum = "137********";
       //Form2 f2 = null;
       //Form5 f5 = null;
       FormInputPhoneNumber fInputPhoneNum = null;

       System.DateTime dtStart;                 //界面打开的时间      
       System.DateTime dtHistory;               //日期翻页的历史点击时间
       System.DateTime dtNow;                   //日期翻页的此刻点击时间
       System.DateTime dtCurrentShow;
       public int iTotalDays = 0;               //运行的总天数
       public int iIntevalDays = 1;             //点击时从1开始计数
       public int iIndex = 0;                   //画图的索引号

       public int iIndexSaveSpectrumdataA = 0;   //用于将保存光谱数据的间隔放大100倍，即100个光谱曲线数据只存一个
       public int iIndexSaveSpectrumdataB = 0;   //用于将保存光谱数据的间隔放大100倍，即100个光谱曲线数据只存一个

       public int m_spectrumCommunicationGeneration = 1;//光谱仪通信协议的代数，默认是第一代通信协议；

        //拖动窗口所是使用的函数--------------
        private static bool IsDragging = false;	//用于指示当前是不是在拖拽状态
		private Point StartPoint = new Point(0, 0);	//记录鼠标按下去的坐标, new是为了拿到空间, 两个0无所谓的
		//记录动了多少距离,然后给窗体Location赋值,要设置Location,必须用一个Point结构体,不能直接给Location的X,Y赋值
		private Point OffsetPoint = new Point(0, 0);
        //------------------
        public Form1()
        {           
            InitializeComponent();

            msgShowFrm = new MSGShow();
            maskFrm = new mask();
            maskFrm.Owner = this;
            uDiskFrm = new UDiskCopyDlg();
            //创建左右滑动的tab控件---------------------
            slideTab = new SlideableTabControl(content);
            slideTab.SlidePageEamless = true;
            //将专业模式下的导航条隐藏（只在专业模式下显示）---
            //professionalMenu.Hide();

        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            int authcodeFlag = 0;
            string PATH = @"C:\authcode.txt";
            if (!File.Exists(PATH))
            {
                authcodeFlag = 1;
                Form2 rm2 = new Form2();
                rm2.StartPosition = FormStartPosition.CenterParent;
                rm2.ShowDialog();
            }
            else
            {
                StreamReader sr = new StreamReader(PATH, Encoding.Default);
                string line = sr.ReadLine();
                sr.Close();
                Form2 rm2 = new Form2();
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    try
                    {
                        string tmpstring = rm2.decode(line);
                        if (tmpstring == ni.GetPhysicalAddress().ToString())
                            authcodeFlag = 1;
                    }
                    catch (Exception)
                    {
                        Application.Exit();
                    }

                }
            }
            if (authcodeFlag == 0)
                Application.Exit();
            //logopng.Parent = this;

            msgShowFrm.Show();
            msgShowFrm.Visible = false;
            maskFrm.Show();
            maskFrm.Visible = false;
            uDiskFrm.Show();
            uDiskFrm.Visible = false;
            //创建用于保存文件的目录------
            CreateFolder(@"C:\SaveData");       //保存光谱数据
            CreateFolder(@"C:\SaveData\A路");   //保存光谱数据
            CreateFolder(@"C:\SaveData\B路");   //保存光谱数据
            CreateFolder(@"C:\SaveData\AB路");  //保存光谱数据

            CreateFolder(@"C:\测试数据");       //保存结果文件

            CreateFolder(@"C:\LOG");            //保存配置文件
            //搜索注册表中的串口号--------
            RefreshComList(true,true,true,true);//搜索端口号            
            ReadAllINIFile();                         //读取上次保存的INI参数




            //全局数组----------------------------------------------------------------------------------------------------
            receivedOneFrameSpectrumData = new byte[setreceivelength];//用于存储每次接收到的返回数据（包括帧头+4096个光谱数据+验证码+帧尾）
            receiveDataA = new UInt16[pixelNUM];      //用于存储A路的光谱数据（4096个Uint16型的数据）
            receiveDataB = new UInt16[pixelNUM];      //用于存储B路的光谱数据（4096个Uint16型的数据）


            content.SelectedIndex = 0;                //将tab控件默认首先展示第一个页面


             mTabCurrentIndex = 0;

            

            this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
            this.WindowState = FormWindowState.Maximized;    //最大化窗体 

            System.DateTime dt = System.DateTime.Now;
            dtStart = dt;
            dtHistory = dt;
            dtNow = dt;
            dtCurrentShow = dt;//初始化            

            this.Text = "多参数水体组份原位光谱分析系统";


            InitialTimer(10*60*1000);         //初始化10分钟读取一次存储的数据文件

            //用于获取当前时间，显示在右上角----------------
            InitialTimer1(1000);                //1S刷新一次
            timer1.Start();                     //默认开启
            //手动点击屏幕，屏幕变量20分钟后通过下面计时器再次将屏幕变暗----------------
            InitialTimer2ShowMask(20*60*1000);                //20分钟后执行     20*60* 
            //用于定时将数据上传至云端----------------
            InitialTimer4SendData2Cloud(180*1000);      //3min上传一次云 默认关闭
            Initialtimer5Heartbeat(30 * 1000);           //30s给服务器发送一次心跳包          

            historyIndex = 0;                               //查询历史数据时需要用这个参数

            this.ShowAandBRoute.Checked = true;             //双路显示中默认显示两路 

          //  btnOpenComSensor_Click(null, null);             //打开传感器查询串口
          //  btnOpenComSpectrum_Click(null, null);           //打开光谱仪查询串口
           // btnOpenComSpeed_Click(null, null);              //打开流量计的查询串口

            DateTime dtstart = System.DateTime.Now;
            systemStartTime.Text = dtstart.ToString("(系统启动时间：yyyy年MM月dd日HH时mm分)");
            WriteSensorLog("启动时间：" + systemStartTime.Text);

            if (bAutoCaptureSensorData)                              // 是否设置自动打开（从INI配置文件读取，参考ReadAllINIFile()函数）
                CaptureSensorDataContinuous_Click(null,null);       //自动打开传感器连续采集按钮  
      

            if (bAutoOpenPLCCOM)                                    // 是否设置自动打开（从INI配置文件读取，参考ReadAllINIFile()函数）
                btnOpenComPLC_Click(null, null);                    //自动打开PLC外部查询串口 

            if (bAutoSendData2Cloud)                                // 是否设置自动打开（从INI配置文件读取，参考ReadAllINIFile()函数）
                cloudUpload_Click(null, null);                      //自动开启将数据上传至云服务器            
            SetCursorPos(this.Width, this.Height);

            
        }


  
        private static bool NetworkIsConnected()
        {
            int I = 0;
            bool state = InternetGetConnectedState(out I, 0);
            return state;
        }
    
      
        private void clearFile(string path)
        {
            if (File.Exists(path))
            {
                FileAttributes attr1 = File.GetAttributes(path);
                if (attr1 == FileAttributes.Directory)
                {
                    // 删除文件夹
                    Directory.Delete(path, true);
                }
                else
                {
                    // 删除文件
                    File.Delete(path);
                }
            }
        }
        //开启设备
   
     
        private void WriteErrorLog(Exception ex)
        {
            try 
            {
                LogWriteLock.EnterWriteLock();
                using (StreamWriter sw_error = File.AppendText(@"C:\LOG\ErrLog.txt"))
                {
                    iErrorTimes++;
                    sw_error.WriteLine("出现应用程序未处理的异常：" + System.DateTime.Now.ToString());
                    sw_error.WriteLine("异常信息：" + ex.Message);
                    sw_error.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                    sw_error.WriteLine("异常对象：" + ex.Source);
                    sw_error.WriteLine("触发方法：" + ex.TargetSite);
                    sw_error.WriteLine("---------------------------------------------------------");
                    sw_error.Close();
                }
            }
            catch
            {
            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }            
        }

                private void WriteSocketLog(string msg)
        {
            Thread thread = new Thread(threadWriteLogSocket);
            thread.Start(msg);//启动新线程          
        }
        public void threadWriteLogSocket(object obj)
        {

            try
            {
                string msg = obj.ToString();
                using (StreamWriter sw2 = new StreamWriter(@"C:\LOG\log_socket.txt",true))
                {
                    sw2.WriteLine(System.DateTime.Now.ToString() + "," + msg);
                    sw2.Close();
                }

            }
            catch (Exception)
            {
                
            }
        }
        private void threadreconnect()//启动我方重连线程
        {
            Thread thread = new Thread(Reconnect);
            thread.Start();
        }
       
        private void InitialTimer(int interval)
        {
            //设置定时间隔(毫秒为单位)
            timer = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer.Enabled = true;
            //绑定Elapsed事件
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
            timer.Stop();
        }
        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {                      
                DateTime dt = System.DateTime.Now;
                _24HourCurveTime.               Add(dt);
                //第一页数据存入数据曲线
                _24HourCurveCOD.                Add(_24ValueCOD);
                _24HourCurveNH4_N.              Add(_24ValueNH4_N);
                _24HourCurveSC.                 Add(_24ValueSC);
                _24HourCurveConductivity.       Add(_24ValueConductivity);
                _24HourCurveNH4_N_1.            Add(_24ValueNH4_N_1);

                this.Invoke(new SetHistoryMessageDelegate(setHistoryDateMessage), "最近24小时数据曲线", _24HourCurveTime[0], _24HourCurveTime[_24HourCurveTime.Count-1]);
                
                TimeSpan span = dt.Subtract(_24HourCurveTime[0]);
                if (span.Days >=1)
                {
                    _24HourCurveTime.               RemoveAt(0);
                    _24HourCurveCOD.                RemoveAt(0);
                    _24HourCurveNH4_N.              RemoveAt(0);
                    _24HourCurveSC.                 RemoveAt(0);  
                    _24HourCurveConductivity.       RemoveAt(0);
                    _24HourCurveNH4_N_1.            RemoveAt(0);
                }
                switch(currentUnitIndex)
                {

                    case 0://COD
                        this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveCOD);
                        break;
                    case 1://氨氮
                        this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveNH4_N);
                        break;
                    case 2://SC
                        this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveSC);
                        break;
                    case 3://电导率
                        this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveConductivity);
                        break;
                    case 4://氨氮-1
                        this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveNH4_N_1);
                        break;

                }

                Save10MinitusData(savedHistoryFileTitle[0], _24ValueCOD);               //每10分钟保存一次COD数据
                Save10MinitusData(savedHistoryFileTitle[1], _24ValueNH4_N);             //每10分钟保存一次氨氮数据
                Save10MinitusData(savedHistoryFileTitle[2], _24ValueSC);                //每10分钟保存一次悬浮物数据
                Save10MinitusData(savedHistoryFileTitle[3], _24ValueConductivity);      //每10分钟保存一次电导率数据
                Save10MinitusData(savedHistoryFileTitle[4], _24ValueNH4_N_1);           //每10分钟保存一次氨氮_1数据
            }
            catch (Exception ex)
            {
                WriteSaveValueLog("每10分钟更新曲线以及数据保存出错:"+ex.ToString());
            }
        }
        private void CaptureData(string strValue)//进行光谱仪查询
        {
           // receiveLength = 0;
            timer0Count++;
            if (timer0Count > Math.Pow(2, 60)) timer0Count = 0;//防止计数器溢出。
           // sendStartLighttingCMDtoSpectrum();
           // Thread.Sleep(200);
            sendSerialPortCMDSpectrum(1, "EB 99 7C 00");//请求光谱数据回传           
           // CaptureSpectrumData(null,null);           
        }
        private void InitialTimer1(int interval)
        {
            //设置定时间隔(毫秒为单位)
            timer1 = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer1.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer1.Enabled = true;
            //绑定Elapsed事件
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp1);
            timer1.Stop();
        }
        private void InitialTimer2ShowMask(int interval)
        {
            //设置定时间隔(毫秒为单位)
            timer2ShowMask = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer2ShowMask.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer2ShowMask.Enabled = true;
            //绑定Elapsed事件
            timer2ShowMask.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp2ShowMask);
            timer2ShowMask.Stop();
        }
        private void InitialTimer4SendData2Cloud(int interval)
        {
            //设置定时间隔(毫秒为单位)
            timer4SendData2Cloud = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer4SendData2Cloud.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer4SendData2Cloud.Enabled = true;
            //绑定Elapsed事件
            timer4SendData2Cloud.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp4SendData2Cloud);
            timer4SendData2Cloud.Stop();
        }
        private void Initialtimer5Heartbeat(int interval)//定时发送心跳包 --> 触发KeepAlive函数
        {
            //设置定时间隔(毫秒为单位)
            timer5Heartbeat = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer5Heartbeat.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer5Heartbeat.Enabled = true;
            //绑定Elapsed事件
            timer5Heartbeat.Elapsed += new System.Timers.ElapsedEventHandler(KeepAlive);
            timer5Heartbeat.Stop();//默认关闭        

        }

        private void connect()
        {
            try
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPHostEntry hostinfo = Dns.GetHostEntry(cloudServerIP.Text);//域名形式
                IPAddress[] aryIP = hostinfo.AddressList;
                IPAddress address = aryIP[0];
                //IPAddress ip = IPAddress.Parse("120.77.88.252");//ip形式
                IPEndPoint point = new IPEndPoint(address, Convert.ToInt32(cloudServerPort.Text));
                socketSend.Connect(point);
                IsConnectedFlag = true;
            }
            catch (Exception ex)
            {
                WriteSocketLog("连接服务器失败！" + "网络通断:" + NetworkIsConnected().ToString() + "  " + ex.ToString());
                IsConnectedFlag = false;
            }

        }
        private void Reconnect()//限制重新连接次数
        {
            while (true)
            {
                if (NetworkIsConnected() && socketSend == null)//如果网络连接,并且socketSend为null
                {
                    try
                    {
                        Thread.Sleep(4 * 60 * 1000);
                        socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//难道这里频繁连接服务器被拒绝？？？
                        IPHostEntry hostinfo = Dns.GetHostEntry(cloudServerIP.Text);//域名形式
                        IPAddress[] aryIP = hostinfo.AddressList;
                        IPAddress address = aryIP[0];
                        IPEndPoint point = new IPEndPoint(address, Convert.ToInt32(cloudServerPort.Text));
                        socketSend.Connect(point);
                        byte[] serialbuffer = System.Text.Encoding.UTF8.GetBytes(mSerialNumber);//序列号字符串转化为字节数组
                        socketSend.Send(serialbuffer);
                        timer4SendData2Cloud.Start();//开启上传数据
                        timer5Heartbeat.Start();     //开启心跳包
                        WriteSocketLog("重连平台成功！");
                        IsConnectedFlag = true;
                        break;

                    }
                    catch (Exception ex)
                    {
                        WriteSocketLog("Reconnect出现错误 当前线程ID：" + Thread.CurrentThread.ManagedThreadId.ToString() + "  " + ex.ToString());
                        if (socketSend != null)
                        {
                            socketSend.Close();
                            socketSend = null;
                        }
                        IsConnectedFlag = false;

                    }
                }
                Thread.Sleep(60 * 1000);
            }
        }
        private void KeepAlive(object sender, System.Timers.ElapsedEventArgs e)//发送心跳包检查socket是否还连接正常，如果没有则重新连接
        {
            byte[] heartbeatBuffer = System.Text.Encoding.UTF8.GetBytes(HeartBeat);//心跳包字符串Q转化为字节数组
            byte[] recvBuffer = new byte[10];

            try//试图发送心跳包
            {
                socketSend.Send(heartbeatBuffer);//断线时再发送，就会出错！！！！！！！！！！！
                //Thread.Sleep(1000);
                //socketSend.ReceiveTimeout = 5000;
                //socketSend.BeginReceive()
                //int len = socketSend.Receive(recvBuffer);
                //string recvstr = Encoding.UTF8.GetString(recvBuffer, 0, len);
                //if (recvstr == "A")
                //    return;                                                
            }
            catch (Exception ex)//发送心跳包失败或接收服务器数据失败，此时通信异常
            {
                WriteSocketLog("KeepAlive出错！" + ex.ToString()); 
                timer4SendData2Cloud.Stop();
                timer5Heartbeat.Stop();
                if (socketSend != null)/////////////////是否需要&& socketSend.Connected？？
                {
                    socketSend.Close();//关闭上一次的socket
                    socketSend = null;
                    //MessageBox.Show("成功关闭socket,等待中...");
                }
                //Thread.Sleep(2 * 60 * 1000);//2min后等TIME_WAIT过去后，重新创建socket对象
                threadreconnect();              
            }   
        }
                    
        private void TimerUp2ShowMask(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new SetDarkOrLightDelegate(UpdateDarkOrLight), (Int32)5000);
            timer2ShowMask.Stop();
        }

        private void TimerUp1(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
              //  currentCount1 += 1;
                this.Invoke(new SetControlValue(CaptureData1),"1");
            }
            catch (Exception ex)
            {
                WriteSpeedLog("右上角时间更新出现错误！"+ex.ToString());  
               
            }
        }
        private void CaptureData1(string strValue)
        {
            DateTime dt = System.DateTime.Now;
            label_Time.Text = dt.ToString("yyyy年MM月dd日") + DateTime.Now.ToString(" HH时mm分ss秒");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //描绘分隔线
          //  Graphics g = e.Graphics;
          //  Pen p = new Pen(Color.Black, 1);
          //  p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
          //  Point point1 = new Point(0, this.panel1.Location.Y - 2);
          //  Point point2 = new Point(this.Width, this.panel1.Location.Y - 2);
          //  g.DrawLine(p, point1, point2);

         //   Point point3 = new Point(0, this.chart1.Location.Y - 15);
         //   Point point4 = new Point(this.Width, this.chart1.Location.Y - 15);
          //  g.DrawLine(p, point3, point4);

        }

        //打开串口
        private bool OpenSerialPortSensor(string strCOM, string strBaudRate)
        {
            if (!serialPortSensor.IsOpen)//未打开
            {
               // string strCOM1 = "COM3";
                serialPortSensor.PortName = strCOM;
                //波特率
               // int iBaudRate = 9600; string strBaudRate1 = iBaudRate.ToString();
                serialPortSensor.BaudRate = Convert.ToInt32(strBaudRate, 10);
                //停止位
                serialPortSensor.StopBits = StopBits.One;
                //设置数据位
                int iDataBits = 8; string strDataBits = iDataBits.ToString();
                serialPortSensor.DataBits = Convert.ToInt32(strDataBits);
                //设置奇偶校验位,偶
                serialPortSensor.Parity = Parity.None;
                //流控制
                serialPortSensor.Handshake = Handshake.None;
                serialPortSensor.ReceivedBytesThreshold =1;
                try
                {
                    serialPortSensor.Open();     //打开串口
                    return true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        showHintMsg("传感器串口打开错误，请查看端口是否被占用！");
                        WriteSensorLog("传感器串口打开错误，请查看端口是否被占用！"+ex.ToString());                      
                        return false;
                    }
                    catch (Exception ex1)
                    {
                    }                
                }
            }
            else//如果串口是打开的则将其关闭
            {
                serialPortSensor.Close();
                //toolStripStatusLabel1.Text = "串口已关闭";
            }
            return true;
        }
        private void OpenSerialPortSpectrum(string strCOM, string strBaudRate)
        {
            if (!serialPortSpectrum.IsOpen)//未打开
            {
                //string strCOM1 = "COM8";
                serialPortSpectrum.PortName = strCOM;
                //波特率
               // int iBaudRate =115200; string strBaudRate1 = iBaudRate.ToString();
                serialPortSpectrum.BaudRate = Convert.ToInt32(strBaudRate, 10);
                //停止位
                serialPortSpectrum.StopBits = StopBits.One;
                //设置数据位
                int iDataBits = 8; string strDataBits = iDataBits.ToString();
                serialPortSpectrum.DataBits = Convert.ToInt32(strDataBits);
                //设置奇偶校验位,无
                serialPortSpectrum.Parity = Parity.None;
                //流控制
                serialPortSpectrum.Handshake = Handshake.None;
                serialPortSpectrum.ReceivedBytesThreshold = 4;
                try
                {
                    serialPortSpectrum.Open();     //打开串口                    
                }
                catch (Exception ex)
                {
                    showHintMsg("端口" + strCOM+"被占用");
                    return;                    
                }
            }
           
        }
        private void OpenSerialPortSpeed(string strCOM, string strBaudRate)
        {
            if (!serialPortSpeed.IsOpen)//未打开
            {
                //string strCOM1 = "COM8";
                serialPortSpeed.PortName = strCOM;
                //波特率
                // int iBaudRate =115200; string strBaudRate1 = iBaudRate.ToString();
                serialPortSpeed.BaudRate = Convert.ToInt32(strBaudRate, 10);
                //停止位
                serialPortSpeed.StopBits = StopBits.One;
                //设置数据位
                int iDataBits = 8; string strDataBits = iDataBits.ToString();
                serialPortSpeed.DataBits = Convert.ToInt32(strDataBits);
                //设置奇偶校验位,无
                serialPortSpeed.Parity = Parity.None;
                //流控制
                serialPortSpeed.Handshake = Handshake.None;
                serialPortSpeed.ReceivedBytesThreshold = 4;
                try
                {
                    serialPortSpeed.Open();     //打开串口                    
                }
                catch (Exception ex)
                {
                    showHintMsg("端口" + strCOM + "被占用");
                    return;                  
                }
            }

        }
        private void OpenSerialPortPLC(string strCOM, string strBaudRate)
        {
            if (!serialPortPLC.IsOpen)//未打开
            {
                //string strCOM1 = "COM8";
                serialPortPLC.PortName = strCOM;
                //波特率
                // int iBaudRate =115200; string strBaudRate1 = iBaudRate.ToString();
                serialPortPLC.BaudRate = Convert.ToInt32(strBaudRate, 10);
                //停止位
                serialPortPLC.StopBits = StopBits.One;
                //设置数据位
                int iDataBits = 8; string strDataBits = iDataBits.ToString();
                serialPortPLC.DataBits = Convert.ToInt32(strDataBits);
                //设置奇偶校验位,无
                serialPortPLC.Parity = Parity.None;
                //流控制
                serialPortPLC.Handshake = Handshake.None;
                serialPortPLC.ReceivedBytesThreshold = 4;
                try
                {
                    serialPortPLC.Open();     //打开串口                    
                }
                catch (Exception ex)
                {
                    showHintMsg("端口" + strCOM + "被占用");
                    return;
                }
            }

        }
        //发送指令,PH
   
        public void sendSerialPortCMDSpectrum(int lablelocation, string cmd)
        {
            try
            {
                if (serialPortSpectrum.IsOpen)  //如果串口开启
                {
                   // label_PH_value.ForeColor = Color.DarkGreen;
                  //  serialPortSpectrum.DiscardOutBuffer();
                    //string strTemp = cmd;
                    string sendBuf = cmd;//strTemp;
                    //sendBuf = sendBuf.Trim();
                    //sendBuf = sendBuf.Replace(',', ' ');//去掉英文逗号
                    //sendBuf = sendBuf.Replace('，', ' ');//去掉中文逗号
                    //sendBuf = sendBuf.Replace("0x", "");//去掉0x
                    //sendBuf.Replace("0X", "");//去掉0X
                    string[] strArray = sendBuf.Split(' ');
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
                    serialPortSpectrum.Write(byteBuffer, 0, byteBuffer.Length);
                    // iCount2++;
                    // WriteSensorLog(System.DateTime.Now.ToString() + "," + "查询串口第" + iCount2.ToString() + "次发送字节完成");
                    byteBuffer = null;
                    strArray = null;
                    GC.Collect();
                }
                else
                {
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    //label_PH_value.ForeColor = Color.Red;
                    // openPHandCl();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //日志记录
                    WriteErrorLog(ex);
                    WriteSpectrumLog("光谱命令发送出现错误:"+ex.ToString());
                  
                }
                catch (Exception ex1)
                {
                }
            }

        }
        public void sendSerialPortCMD(int lablelocation,string cmd)
        {
            try
            {
                if (serialPortSensor.IsOpen)  //如果串口开启
                {
                    //label_PH_value.ForeColor = Color.DarkGreen;
                   // serialPortSensor.DiscardOutBuffer();
                    string strTemp = cmd;
                    string sendBuf = strTemp;
                    sendBuf = sendBuf.Trim();
                    sendBuf = sendBuf.Replace(',', ' ');//去掉英文逗号
                    sendBuf = sendBuf.Replace('，', ' ');//去掉中文逗号
                    sendBuf = sendBuf.Replace("0x", "");//去掉0x
                    sendBuf.Replace("0X", "");//去掉0X
                    string[] strArray = sendBuf.Split(' ');
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
                    serialPortSensor.Write(byteBuffer, 0, byteBuffer.Length);
                   // iCount2++;
                   // WriteSensorLog(System.DateTime.Now.ToString() + "," + "查询串口第" + iCount2.ToString() + "次发送字节完成");
                    byteBuffer = null;
                    GC.Collect();
                }
                else
                {
                    //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                    //label_PH_value.ForeColor = Color.Red;
                    // openPHandCl();
                }
            }
            catch (Exception ex)
            {
                WriteSensorLog("传感器数据发送出现错误:" + ex.ToString());
              
            }

        }
    
        private float getFloat(byte[] data)
        {
            //data为CDAB格式
            byte[] datatmp = new byte[4];
            datatmp[0] = data[1];//datatmp[0]为低地址存D
            datatmp[1] = data[0];
            datatmp[2] = data[3];
            datatmp[3] = data[2];
            float value = BitConverter.ToSingle(datatmp, 0);
            datatmp = null;
            GC.Collect();
            return value;
        }
        private float calculateAverage30Times(List<float> list)
        {
            float ave = 0f;
            int num=list.Count;
            int left, right;
            left = right = num / 3;
            int usingnum = num - left - right;
            float[] buffer=new float[num];
            list.CopyTo(buffer);
            BubbleSort(buffer);
            for(int i=left;i<num-right;i++)
            {
                ave += buffer[i] / usingnum;
            }
            buffer = null;
            GC.Collect();
            return ave;
        }
        public void BubbleSort(float[] R)
        {
            float temp; //交换标志
            int i, j;
            bool exchange;
            for (i = 0; i < R.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假
                for (j = R.Length - 2; j >= i; j--)
                {
                    if (R[j + 1] < R[j])　//交换条件
                    {
                        temp = R[j + 1];
                        R[j + 1] = R[j];
                        R[j] = temp;
                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }
                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
        }
       
        public void SaveEveryData(string valueTitle, float value)
        {
            saveValueStruct sstruct=new saveValueStruct();
            sstruct.title=valueTitle;
            sstruct.value=value;
            Thread thread = new Thread(SaveEveryDataThread);
            thread.Start((object)sstruct);           
        }
        public void SaveEveryDataThread(object obj)
        {
            saveValueStruct sstruct = (saveValueStruct)obj;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string date = currentTime.ToLongDateString();//只到日期
            string filename = "";
            try
            {
                //该文件只存放每天的数据信息---------------------------------------------------------------12s查询一次，每10分钟存一次。               
                filename = @"C:\测试数据\" + "Every_" + sstruct.title + "_" + date + ".txt";//日期文档,_PHandCL_ 
                //追加文档                
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(currentTime.ToString() + " " + sstruct.value.ToString("f4"));
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                WriteSaveValueLog("保存每次采集数据错误：" + ex.ToString());
                WriteErrorLog(ex);
            }
        }
        private void UpdateValueFunction(int index, float value)
        {
            try
            {
                switch (index)
                {
                    case 0://COD       位置0
                        if (mTabCurrentIndex==3)//UV254
                        {
                            txbUV254Read.Text = value.ToString("f2");
                            break;                            
                        }
                        else
                        {
                            _24ValueCOD = value;
                            label_COD_value.Text = value.ToString("f2");
                            SaveEveryData(savedHistoryFileTitle[0], value);//保存每一次COD数据
                            break;
                        }
                    case 1://NH4-N    位置1
                        _24ValueNH4_N = value;
                        label_NH4_value.Text = value.ToString("f2");
                        SaveEveryData(savedHistoryFileTitle[1], value);//保存每一次氨氮数据
                        break;
                    case 2://SC   位置2
                        _24ValueSC = value;
                        label_SC_value.Text = value.ToString("f2");
                        SaveEveryData(savedHistoryFileTitle[2], value);//保存每一次SC数据
                        break;
                    case 3://电导率 位置3
                        label_Conductivity_value.Text = value.ToString("f2");
                        _24ValueConductivity = value;
                        SaveEveryData(savedHistoryFileTitle[3], value);//保存每一次电导率数据
                        break;
                    case 4://NH4-N_1    位置4
                        _24ValueNH4_N_1 = value;
                        label_NH4_1_value.Text = value.ToString("f2");
                        SaveEveryData(savedHistoryFileTitle[4], value);//保存每一次氨氮-1数据
                        break;

                    case 5://pH    位置5
                        pH_txb.Text = value.ToString("f2");
                        break;

                    case 6://pH-1    位置5
                        pH_1_txb.Text = value.ToString("f2");
                        break;

                }
            }
            catch (Exception ex)
            {
                WriteSpeedLog("数据更新错误：" + ex.ToString());
            }
        }
        
        private void serialPortSensor_DataReceived(object sender, SerialDataReceivedEventArgs e)//待改
        {
            try
            {
              //  Thread.Sleep(50);    
                int setlength;                                  //每包数据的长度17变为9
                int receiveLength = serialPortSensor.BytesToRead;
                if (receiveLength == 0) return;
                byte[] receiveData = new byte[receiveLength];
                serialPortSensor.Read(receiveData, 0, receiveData.Length);
                mReceiveSensorBufferPool.AddRange(receiveData);
                receiveData = null; GC.Collect();
                
                while(true)                                      //纠正数据出现错漏的情况
                {
                    if (mReceiveSensorBufferPool.Count >= 2)
                    {
                        //返回数据为：地址COD 01 氨氮02 悬浮物03 电导率04 氨氮-1 05
                        if ((mReceiveSensorBufferPool[0] == 0x01 || mReceiveSensorBufferPool[0] == 0x02 || mReceiveSensorBufferPool[0] == 0x03 || mReceiveSensorBufferPool[0] == 0x04 || mReceiveSensorBufferPool[0] == 0x05) && mReceiveSensorBufferPool[1] == 0x03)
                        {
                            break;
                        }
                        else
                        {
                            mReceiveSensorBufferPool.RemoveRange(0, 1);//去除第一个元素，直至mReceiveSensorBufferPool.count<3则return
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                setlength = 9;
                int PoolDataCount = mReceiveSensorBufferPool.Count;
                if (PoolDataCount < setlength) return;//如果不足一帧，则继续接收
                int times = 0;
               
                int DevideTimes = (int)(PoolDataCount/ setlength);
                if (PoolDataCount>=setlength)          //如果已经回来一帧数据或者多帧数据
                {
                    for (int i = 0; i < DevideTimes; i++)//如果一次回来多帧数据则进行拆包处理
                    {
                        times++;
                        byte[] OneFrameData = new byte[setlength];
                        byte[] waiteforCorrect = new byte[setlength - 2];
                        byte[] crcResult = new byte[2];
                        byte[] floatArraytmp = new byte[4];//用于存放转换回来的float数据 
                        mReceiveSensorBufferPool.CopyTo(0, OneFrameData, 0, setlength);
                        mReceiveSensorBufferPool.CopyTo(0, waiteforCorrect, 0, setlength-2);
                        mReceiveSensorBufferPool.RemoveRange(0, setlength);
                        
                        byte[] crcreturn = CRC16(waiteforCorrect);
                        if (crcreturn[0] == OneFrameData[setlength - 2] && crcreturn[1] == OneFrameData[setlength - 1])//先判断是否符合校验    
                        {
                            if (OneFrameData[0] == 0x01)//COD,地址01
                            {
                                    try
                                    {
                                        Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                        float COD = getFloat(floatArraytmp);
                                        this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 0, COD);
                                    }
                                    catch(Exception ex)
                                    {
                                        WriteSensorLog("COD:"+ex.ToString());
                                    } 
                            }
                            else if (OneFrameData[0] == 0x02)//氨氮
                            {
                                if (mTabCurrentIndex == 3 && (CaptureSensorContinuesBackUp.Text == "开启传感器连续采集"))//当前为传感器矫正
                                {
                                    try
                                    {
                                        Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                        float pH = getFloat(floatArraytmp);
                                        this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 5, pH);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteSensorLog("pH:" + ex.ToString());
                                    } 
                                    
                                }

                                else
                                {
                                    try
                                    {
                                        Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                        float NH4_N = getFloat(floatArraytmp);
                                        this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 1, NH4_N);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteSensorLog("氨氮:" + ex.ToString());
                                    } 
                                }
                            }
                            else if (OneFrameData[0] == 0x03)//悬浮物
                            {
                                try
                                {
                                    Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                    float SS = getFloat(floatArraytmp);
                                    this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 2, SS);
                                }
                                catch (Exception ex) 
                                {
                                    WriteSensorLog("悬浮物:" + ex.ToString());
                                }
                            }      
                            else if (OneFrameData[0] == 0x04)//电导率
                            {
                                try
                                {
                                    Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                    float Conductivity = getFloat(floatArraytmp);
                                    this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 3, Conductivity);

                                }
                                catch (Exception ex)
                                {
                                    WriteSensorLog("Conductivity:" + ex.ToString());
                                }

                            }
                            else if (OneFrameData[0] == 0x05)//氨氮-1
                            {
                                if (mTabCurrentIndex == 3 && (CaptureSensorContinuesBackUp.Text == "开启传感器连续采集"))//当前为传感器矫正
                                {
                                    try
                                    {
                                        Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                        float pH_1 = getFloat(floatArraytmp);
                                        this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 6, pH_1);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteSensorLog("pH-1:" + ex.ToString());
                                    }

                                }

                                else
                                {
                                    try
                                    {
                                        Array.Copy(waiteforCorrect, 3, floatArraytmp, 0, 4);
                                        float NH4_N_1 = getFloat(floatArraytmp);
                                        this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 4, NH4_N_1);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteSensorLog("氨氮-1:" + ex.ToString());
                                    } 
                                }
                            }
                            else
                            {
                                WriteSensorLog("其他情况:" + OneFrameData[0].ToString());
                            }
                            mAutoQuestOverEvent.Set();//查询线程发送查询一个传感器命令后调用mAutoQuestOverEvent.waitone（5000）等待5s，此处调用set用于通知该查询的结果已经返回，可以继续查询其他传感器
                            
                        }

                        OneFrameData = null;
                        waiteforCorrect = null;
                        crcResult = null;
                        floatArraytmp = null;
                        GC.Collect();
                    }
                }  
            }
            catch
            {
              //  MessageBox.Show("传感器接收出现错误");
                if(mTabCurrentIndex==0)this.Invoke(new SetControlValue(showHintMsg), "传感器接收出现错误！");
                WriteSensorLog("传感器接收出现错误！");
            }
        }
      
        public static byte[] CRC16(byte[] data)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置

                return new byte[] { lo, hi }; //CRC低位在前
            }
            return new byte[] { 0, 0 };
        }
        public static void crcCalculation(byte[] data, out byte[] result)
        {
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = data.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ data[n];
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
            //string[] redata = new string[2];
            result=new byte[2];
            result[1] = (byte)((crc >> 8) & 0xff);
            result[0] = (byte)(crc & 0xff);            
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
        public void Save10MinitusData(string valueTitle, float value)
        {            
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string date = currentTime.ToLongDateString();//只到日期
            string filename = "";
            try
            {
                //该文件只存放每天的数据信息---------------------------------------------------------------12s查询一次，每10分钟存一次。               
                filename = @"C:\测试数据\" + "_" + valueTitle + "_" + date + ".txt";//日期文档,_PHandCL_ 
                //追加文档                
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(currentTime.ToString() + ","  + value.ToString("f4"));
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                WriteSaveValueLog("保存10分钟数据错误："+ex.ToString());
                WriteErrorLog(ex);
            }
        }
        private void SaveSingleValue2Txt(string valueTitle,float value )
        {
            saveValueStruct sstruct=new saveValueStruct();
            sstruct.title=valueTitle;
            sstruct.value=value;
            Thread thread = new Thread(SaveValueData);
            thread.Start((object)sstruct);
        }
        public void SaveValueData(object obj)
        {            
            saveValueStruct sstruct = (saveValueStruct)obj;
            string valueTitle = sstruct.title;
            float value = sstruct.value;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string date = currentTime.ToLongDateString();//只到日期
            string filename = "";
            try
            {
                //该文件只存放每天的数据信息---------------------------------------------------------------12s查询一次，每10分钟存一次。               
                if (timer2Count % (50*2) == 0) //50,60,12  此时为10分钟
                {                    
                    filename = @"C:\测试数据\" + "_" + valueTitle + "_" + date + ".txt";//日期文档,_PHandCL_ 
                    //追加文档                
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        sw.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                                    + value.ToString("f4"));
                        sw.Close();
                    }
                }
                //该文件只存放最近24小时的数据-------------------------------------------------------------------
                //追加文档，不标注日期，超过24小时需清理，画图文档
                if (timer2Count % (50*2) == 0) //50,60,12 10分钟
                {
                    //追加文档     
                    filename= @"C:\测试数据\" + valueTitle + ".txt";
                    if (timer2Count % (7200*2) == 0)//1.2s*60*24=7200//24小时清理1次
                    {
                        FileStream fs1 = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write);
                        fs1.Seek(0, SeekOrigin.Begin);
                        fs1.SetLength(0);
                        fs1.Close();
                        using (StreamWriter sw = File.AppendText(filename))
                        {
                            sw.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                                        + value.ToString("f4"));
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(filename))//往最近24小时文件里追加数据
                        {
                            sw.WriteLine(currentTime.ToString("HH") + "," + currentTime.ToString("mm") + "," + currentTime.ToString("ss") + ","
                                        + value.ToString("f4"));
                            sw.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {  
                WriteErrorLog(ex);                
            }
        }     
        //打开电导率温度传感器的串口，确认参数！！！！！！！！！    
        private void drawRealtimeSpectrumChart(int length, float[] fxParaData, double[] fyParaData)
        {
            try
            {
                double maxy, miny;
                float maxx, minx;
                maxy = miny = fyParaData[0];
                maxx = minx = fxParaData[0];
                for (int i = 0; i < length; i++)
                {
                    if (i <2400)
                    {                       
                        if (fyParaData[i] > maxy) maxy = fyParaData[i];
                        if (fyParaData[i] < miny) miny = fyParaData[i];
                        if (fxParaData[i] > maxx) maxx = fxParaData[i];
                        if (fxParaData[i] < minx) minx = fxParaData[i];
                    }
                }
                if (maxy == 0)
                {
                    this.Invoke(new SetControlValue(showHintMsg), "错误！");
                    return;
                }
                waveShapeChart.Series.Clear();
                Series spectrumSeries = new Series("光谱");
                spectrumSeries.ChartType = SeriesChartType.Line;                
                for (int i = 0; i < length; i++)
                {
                    if (i<2400&&i%3==0)
                    {
                        spectrumSeries.Points.AddXY(fxParaData[i], fyParaData[i]);                        
                    }
                }               
                waveShapeChart.Series.Add(spectrumSeries);
                waveShapeChart.Series[0].Color = Color.White;
                waveShapeChart.Series[0].BorderWidth = 2;
                //historyChart.ChartAreas[0].AxisX.MajorGrid.Interval = (fxParaData[length-1]-fxParaData[0]) / 4;
                waveShapeChart.ChartAreas[0].AxisY.Minimum = miny;
                waveShapeChart.ChartAreas[0].AxisY.Maximum = maxy+5;
                waveShapeChart.ChartAreas[0].AxisX.Minimum = minx;
                waveShapeChart.ChartAreas[0].AxisX.Maximum = maxx;
                //historyChart.ChartAreas[0].AxisY.MajorGrid.Interval = (maxy - miny) / 5;              
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                WriteSpectrumLog("绘制光谱图出现错误:"+ex.ToString());
               
            }
        }
        private void AnalisysReceiveData(byte[] receiveData,out UInt16[] curveData,int dataNum)
        {
            //int dataNum=4096;
            int offset = 1;
            if(m_spectrumCommunicationGeneration==1)
            {
                offset = 1;
            }
            else if (m_spectrumCommunicationGeneration == 1)
            {
                offset=2;
            }
            else
            {
                offset = 1;
            }
            curveData = new UInt16[dataNum];
            for (int i = 0; i < dataNum;i++ )
            {
                curveData[i] = (UInt16)(receiveData[2 * (i+offset)] + 256 * receiveData[2 * (i+offset) + 1]);
                if(i==2000)
                {
                    int a = curveData[i];
                }
            }
            return;
        }

       //校验码计算
        private byte CRCCheckLH(byte[] auchMsg, int usDataLen)
        {
            byte uchLRC = 0; /*初始化 CRC 字符 */
            for (int i = 0; i < usDataLen; i++)
            {
                uchLRC -= auchMsg[i];
            }
            return uchLRC;
        }  

        //读取某天每间隔10min存的数据
        private bool ReadParaData_TenMin(out int length, out float[,] fParaData, out int[,] startPoint)
        {
            length = 0;
            fParaData = new float[144, 10];
            //string strFileName;
            startPoint = new int[144, 3];
            OpenFileDialog OpenFileDlg = new OpenFileDialog(); //打开文件对话框
            OpenFileDlg.Filter = "文本文件|*.txt|所有文件|*.*";   //设置文件类型
            OpenFileDlg.RestoreDirectory = false;  //保存对话框是否记忆上次的目录
            OpenFileDlg.FilterIndex = 1;   //设置默认文件类型的显示顺序
            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
                strFileName_historical = OpenFileDlg.FileName;
                try
                {
                    if (File.Exists(strFileName_historical))
                    {
                        FileStream fs = new FileStream(strFileName_historical, FileMode.Open);
                        StreamReader sr = new StreamReader(fs);
                        string line;
                        //line = sr.ReadLine();                    
                        //string[] strArray1 = line.Split(',');
                        //int hour = int.Parse(strArray1[0]);
                        //int minutes = int.Parse(strArray1[1]);
                        //int seconds = int.Parse(strArray1[2]);
                        //double temp = hour * 6 + minutes / 10 + ((minutes % 10 + seconds) / 600);
                        //startPoint = (int)Math.Floor(temp);
                        //fParaData[0, 0] = float.Parse(strArray1[3]);
                        //fParaData[0, 1] = float.Parse(strArray1[4]);
                        //fParaData[0, 2] = float.Parse(strArray1[5]);
                        //fParaData[0, 3] = float.Parse(strArray1[6]);
                        //fParaData[0, 4] = float.Parse(strArray1[7]);
                        //fParaData[0, 5] = float.Parse(strArray1[8]);
                        //fParaData[0, 6] = float.Parse(strArray1[9]);
                        //fParaData[0, 7] = float.Parse(strArray1[10]);
                        //fParaData[0, 8] = float.Parse(strArray1[11]);
                        //fParaData[0, 9] = float.Parse(strArray1[12]);
                        //fParaData[0, 10] = float.Parse(strArray1[13]);
                        line = sr.ReadLine();
                        while (line != null)
                        {
                            //line = sr.ReadLine();
                            string[] strArray2 = line.Split(',');
                            fParaData[length, 0] = float.Parse(strArray2[3]);
                            fParaData[length, 1] = float.Parse(strArray2[4]);
                            fParaData[length, 2] = float.Parse(strArray2[5]);
                            fParaData[length, 3] = float.Parse(strArray2[6]);
                            fParaData[length, 4] = float.Parse(strArray2[7]);
                            fParaData[length, 5] = float.Parse(strArray2[8]);
                            fParaData[length, 6] = float.Parse(strArray2[9]);
                            fParaData[length, 7] = float.Parse(strArray2[10]);
                            fParaData[length, 8] = float.Parse(strArray2[11]);
                            fParaData[length, 9] = float.Parse(strArray2[12]);
                            startPoint[length, 0] = int.Parse(strArray2[0]);
                            startPoint[length, 1] = int.Parse(strArray2[1]);
                            startPoint[length, 2] = int.Parse(strArray2[2]);
                            length++;
                            line = sr.ReadLine();
                        }

                        sr.Close();
                        fs.Close();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("文件不存在");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
      

        private bool ReadParaData_TenMin1(out int length, out float[,] fParaData, out int[,] startPoint)
        {
            length = 1;
            fParaData = new float[144, 10];
            startPoint = new int[144, 3];
            //System.DateTime currentTime = new System.DateTime();
            //currentTime = System.DateTime.Now;
            //string time1 = currentTime.ToLongDateString();
            //string filename = @"C:\测试数据\" + "TenMinutes_" + time1 + ".txt";
            //string filename = @"C:\测试数据\" + "TenMinutes" + ".txt";
            if (File.Exists(strFileName_historical))
            {
                FileStream fs = new FileStream(strFileName_historical, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                string line;
                //line = sr.ReadLine();
                //string[] strArray1 = line.Split(',');
                //int hour = int.Parse(strArray1[0]);
                //int minutes = int.Parse(strArray1[1]);
                //int seconds = int.Parse(strArray1[2]);
                //double temp = hour * 6 + minutes / 10 + ((minutes % 10 + seconds) / 600);
                //startPoint = (int)Math.Floor(temp);
                //fParaData[0, 0] = float.Parse(strArray1[3]);
                //fParaData[0, 1] = float.Parse(strArray1[4]);
                //fParaData[0, 2] = float.Parse(strArray1[5]);
                //fParaData[0, 3] = float.Parse(strArray1[6]);
                //fParaData[0, 4] = float.Parse(strArray1[7]);
                //fParaData[0, 5] = float.Parse(strArray1[8]);
                //fParaData[0, 6] = float.Parse(strArray1[9]);
                //fParaData[0, 7] = float.Parse(strArray1[10]);
                //fParaData[0, 8] = float.Parse(strArray1[11]);
                //fParaData[0, 9] = float.Parse(strArray1[12]);
                line = sr.ReadLine();
                while (line != null)
                {
                    //line = sr.ReadLine();
                    string[] strArray2 = line.Split(',');
                    fParaData[length, 0] = float.Parse(strArray2[3]);
                    fParaData[length, 1] = float.Parse(strArray2[4]);
                    fParaData[length, 2] = float.Parse(strArray2[5]);
                    fParaData[length, 3] = float.Parse(strArray2[6]);
                    fParaData[length, 4] = float.Parse(strArray2[7]);
                    fParaData[length, 5] = float.Parse(strArray2[8]);
                    fParaData[length, 6] = float.Parse(strArray2[9]);
                    fParaData[length, 7] = float.Parse(strArray2[10]);
                    fParaData[length, 8] = float.Parse(strArray2[11]);
                    fParaData[length, 9] = float.Parse(strArray2[12]);
                    startPoint[length, 0] = int.Parse(strArray2[0]);
                    startPoint[length, 1] = int.Parse(strArray2[1]);
                    startPoint[length, 2] = int.Parse(strArray2[2]);
                    length++;
                    line = sr.ReadLine();
                }

                sr.Close();
                fs.Close();
                return true;
            }
            else
            {
                //MessageBox.Show("文件不存在");

                return false;
            }
        }

        private string searchTxt(int itemp)
        {
            string strTemp;
            if(itemp<10)
            {
                strTemp = "_PHandCL_";
                return strTemp;
            }
            else if(itemp==10)
            {
                strTemp = "_CondtandTemper_";
                return strTemp;
            }
            else
            {
                strTemp = "_flux_";
                return strTemp;
            }
        }
        private void pictureBox_Left_Click(object sender, EventArgs e)
        {
           
            
            TimeSpan ts1 = dtNow - dtStart;
            iTotalDays = (int)Math.Floor(ts1.TotalDays);//从界面打开到此刻的运行天数
            TimeSpan ts2 = dtNow - dtHistory;
            double dIntervalSeconds = ts2.TotalSeconds;//上次点击翻页按钮到此刻点击总秒数            
            if (iTotalDays >= 1 && dIntervalSeconds > 2 && dIntervalSeconds < 10 && iIntevalDays <= iTotalDays)//第一次点击左翻页键秒数必然大于10，此处忽略了第一次
            {
                string str1=@"C:\测试数据\";
                string str2 = searchTxt(iIndex);
                dtCurrentShow = dtNow.AddDays(-iIntevalDays);
                string str3 = dtNow.AddDays(-iIntevalDays).ToLongDateString();
                strFileName_historical = str1 + str2 + str3;
                //DrawParameter_historical(iIndex);
                dtHistory = dtNow;
                iIntevalDays++;
                //return;
            }
            else
            {
                //string str1 = @"C:\测试数据\";
                //string str2 = searchTxt(iIndex);
                //string str3 = dtNow.ToLongDateString();//此刻的日期
                //strFileName_historical = str1 + str2 + str3;
                //DrawParameter_historical(iIndex);//返回到此刻，该段可要可不要
                //dtHistory = dtNow;
                //dtCurrentShow = dtNow;
                //iIntevalDays = 1;
                return;
            }
           
            return;
        }

        private void pictureBox_Right_Click(object sender, EventArgs e)
        {
            //pageCurrent++;
            //if (pageCurrent > pageCount)
            //{
            //    MessageBox.Show("已经是最后一页，请点击“上一页”查看");
            //    return;
            //}
            //else
            //{
            //    DrawParameter(pageCurrent);
            //}
            ////pageCurrent++;
            ////if (pageCurrent > pageCount)
            ////{
            ////    MessageBox.Show("已经是最后一页，请点击“上一页”查看");
            ////    return;
            ////}
            ////else
            ////{
            ////    DrawParameter_historical(pageCurrent);
            ////}
            //pictureBox_Left.Enabled = false; pictureBox_Right.Enabled = false;
            //pictureBox_up.Enabled = false; pictureBox_down.Enabled = false;
            dtNow = System.DateTime.Now;
            TimeSpan ts1 = dtNow - dtCurrentShow;
            iTotalDays = (int)Math.Floor(ts1.TotalDays);//从当前界面显示到此刻的运行天数
            TimeSpan ts2 = dtNow - dtHistory;
            double dIntervalSeconds = ts2.TotalSeconds;//上次点击翻页按钮到此刻点击总秒数            
            if (iTotalDays >= 1 && dIntervalSeconds > 2 && dIntervalSeconds < 10 && dtCurrentShow.CompareTo(dtNow) < 0 && iIntevalDays <= iTotalDays)
            {
                string str1 = @"C:\测试数据\";
                string str2 = searchTxt(iIndex);
                dtCurrentShow = dtCurrentShow.AddDays(iIntevalDays);
                string str3 = dtCurrentShow.AddDays(iIntevalDays).ToLongDateString();
                strFileName_historical = str1 + str2 + str3;
                //DrawParameter_historical(iIndex);
                dtHistory = dtNow;
                iIntevalDays++;
                //return;
            }
            else
            {
                //string str1 = @"C:\测试数据\";
                //string str2 = searchTxt(iIndex);
                //string str3 = dtNow.ToLongDateString();
                //strFileName_historical = str1 + str2 + str3;
                //DrawParameter_historical(iIndex);//返回到此刻，该段可要可不要
                //dtHistory = dtNow;
                //dtCurrentShow = dtNow;//点击到此刻就只能点击左翻页键了
                ////dtCurrentShow = dtStart;//可一直点击右翻页键
                //iIntevalDays = 1;
                return;
            }
            //pictureBox_Left.Enabled = false; pictureBox_Right.Enabled = false;
            //pictureBox_up.Enabled = false; pictureBox_down.Enabled = false;
            return;
        }
    
   
    
// 
//         private void button_CloseDevice_Click(object sender, EventArgs e)   //断开设备连接，关闭界面
//         {
//             try
//             {
//                 if (Is_Device_Open == true || serialPortSensor.IsOpen == true || serialPortSpectrum.IsOpen == true || serialPortSpeed.IsOpen == true || serialPort4.IsOpen == true)
//                 {
//                     timer.Stop();
//                     timer2.Stop();
//                     serialPortSensor.Close();
//                     //serialPort2.Close();
//                     serialPortSpeed.Close();
//                     serialPort4.Close();
//                     bool result = wrapper.closeSpectraMeter();
//                     if (result == true)
//                     {
//                         //textBox_information.Text = "设备关闭成功.";
//                         toolStripStatusLabel1.Text = "设备关闭成功.";
//                         Is_Device_Open = false;
//                         //////////btn_OpenDevice.Text = "开启设备";
//                     }
//                     else
//                     {
//                         //textBox_information.Text = "设备关闭失败!";
//                         toolStripStatusLabel1.Text = "设备关闭失败!";
//                     }
//                     this.Close();
//                 }
//                 else
//                 {
//                     this.Close();
//                 }
//             }
//             catch(Exception ex)
//             {
//                 WriteErrorLog(ex);
//                 this.Close();
//             }
//                 
//         }




        private void titleMoveBar_MouseDown(object sender, MouseEventArgs e)
        {
            //如果按下去的按钮不是左键就return,节省运算资源
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			//按下鼠标后,进入拖动状态:
			IsDragging = true;
			//保存刚按下时的鼠标坐标
			StartPoint.X = e.X;
			StartPoint.Y = e.Y;
        }

        private void titleMoveBar_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标移动时调用,检测到IsDragging为真时
            if (IsDragging == true)
            {
                //用当前坐标减去起始坐标得到偏移量Offset
                OffsetPoint.X = e.X - StartPoint.X;
                OffsetPoint.Y = e.Y - StartPoint.Y;
                //将Offset转化为屏幕坐标赋值给Location,设置Form在屏幕中的位置,如果不作PointToScreen转换,你自己看看效果就好
                this.Location = PointToScreen(OffsetPoint);
            }
        }

        private void titleMoveBar_MouseUp(object sender, MouseEventArgs e)
        {
            //左键抬起时,及时把拖动判定设置为false,否则,你也可以试试效果
            IsDragging = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // this.Width = 1024;
            // this.Height = 768;
            int width = this.Width;
            int height = this.Height;

            groupTitle.Left = -5;
            groupTitle.Width = 1024 + 5;
            groupNavigationBar.Top = 105;//挡住tabcontrol的选项卡标签
            groupNavigationBar.Left = 0;
            content.Top = 105;
            content.Left = 0;
            content.Width = 1024;
            groupHistory.Left = 0;
            groupHistory.Width = 1024;
            // content.Height = height - content.Top;
            // content.Height = 470;
            //float newx = (this.Width) / x;
            //float newy = (this.Height) / y;
            //setControls(newx, newy, this);
        }

        private void NextUser_Click(object sender, EventArgs e)
        {
            content.SelectedIndex = 2;
            mTabCurrentIndex = 2;
        }

        private void previousUser_Click(object sender, EventArgs e)
        {
            content.SelectedIndex = 0;
            mTabCurrentIndex = 0;
        }

        private void professionalModelbutton_Click(object sender, EventArgs e)
        {
            string str = IniReadValue("SYSTEM", "PassWord");
            if (str.Trim() == "")
            {
                str = "123";
                IniWriteValue("SYSTEM", "PassWord", str);
            }  
            PassWord ps = new PassWord();
            ps.psw = str;
            DialogResult dr= ps.ShowDialog();
            if (dr == DialogResult.Abort)
            {
                PassWordEdit pse = new PassWordEdit();
                pse.oldPassWord = str;
                DialogResult der = pse.ShowDialog();
                if (der==DialogResult.OK)
                {
                    str=pse.newPassWord;
                    IniWriteValue("SYSTEM", "PassWord", str);
                    MessageBoxFrm frm = new MessageBoxFrm();
                    frm.setMsgContent("密码已经修改成功！下次请输入新的密码！");
                    frm.ShowDialog();
                    return;
                }
                else
                {
                    return;
                }
            }
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            currentTitleName.Text = "专业模式";
           // content.Height = 660;
            groupHistory.Hide();
           // professionalMenu.Show();
            content.SelectedIndex = 1;
            mTabCurrentIndex = 1;
        }

        private void returnUser_Click(object sender, EventArgs e)
        {
            currentTitleName.Text = mLocalAddressName;
            //content.Height = 470;
            groupHistory.Show();
            //professionalMenu.Hide();
            content.SelectedIndex = 0;
            mTabCurrentIndex = 0;
        }

        private void enterCorrectionModel_Click(object sender, EventArgs e)
        {
            //currentTitleName.Text = "光谱校正";
            ////professionalMenu.Hide();
            //content.SelectedIndex = 3;
            //mTabCurrentIndex = 3;
            
        }
        private void enterSensorCorrectionModel_Click(object sender, EventArgs e)
        {
            currentTitleName.Text = "传感器校正";
            //professionalMenu.Hide();
            content.SelectedIndex = 3;
            mTabCurrentIndex = 3;
        }
        private void returnProfessionModel_Click(object sender, EventArgs e)
        {
            currentTitleName.Text = "专业模式";
           // professionalMenu.Show();
            content.SelectedIndex = 1;
            mTabCurrentIndex = 1;
        }
        private void comSettingBtn_Click(object sender, EventArgs e)
        {
             //GetComList();
            currentTitleName.Text = "其他参数设置";
            //professionalMenu.Hide();
            content.SelectedIndex = 2;
            mTabCurrentIndex = 2;
        }
        private void comSetting2Professional_Click(object sender, EventArgs e)
        {
            currentTitleName.Text = "专业模式";
           // professionalMenu.Show();
            content.SelectedIndex = 1;
            mTabCurrentIndex = 1;
        }
        private void returnProfessionModel2_Click(object sender, EventArgs e)
        {
            currentTitleName.Text = "专业模式";
           // professionalMenu.Show();
            content.SelectedIndex = 1;
            mTabCurrentIndex = 1;
        }

        private void WriteSensorLog(string msg)
        {
            Thread thread = new Thread(threadWriteLogSensor);
            thread.Start(msg);//启动新线程          
        }
        public void threadWriteLogSensor(object obj)
        {
            string msg = obj.ToString();
            using (StreamWriter sw2 = File.AppendText(@"C:\LOG\log_sensor.txt"))
            {
                sw2.WriteLine(System.DateTime.Now.ToString() + "," + msg);
                sw2.Close();
            }
        }
        private void WriteSpectrumLog(string msg)
        {
            Thread thread = new Thread(threadWriteLogSpectrum);
            thread.Start(msg);//启动新线程           
        }
        public void threadWriteLogSpectrum(object obj)
        {           
            string msg = obj.ToString();
            try
            {
                using (StreamWriter sw2 = File.AppendText(@"C:\LOG\log_spectrum.txt"))
                {
                    sw2.WriteLine(System.DateTime.Now.ToString() + "," + msg);
                    sw2.Close();
                }
            }
            catch(Exception ex)
            {
                WriteSaveValueLog("保存光谱日志错误：" + ex.ToString());
            }
        }
        private void WriteSpeedLog(string msg)
        {
            Thread thread = new Thread(threadWriteLogSpeed);
            thread.Start(msg);//启动新线程           
        }
        public void threadWriteLogSpeed(object obj)
        {
            string msg = obj.ToString();
            try
            {
                using (StreamWriter sw2 = File.AppendText(@"C:\LOG\log_speed.txt"))
                {
                    sw2.WriteLine(System.DateTime.Now.ToString() + "," + msg);
                    sw2.Close();
                }
            }
            catch (Exception ex)
            {
                WriteSaveValueLog("保存流速日志错误：" + ex.ToString());
            }
        }
        private void WriteSaveValueLog(string msg)
        {
            Thread thread = new Thread(threadWriteLogSaveValue);
            thread.Start(msg);//启动新线程           
        }
        public void threadWriteLogSaveValue(object obj)
        {
            try
            {
                string msg = obj.ToString();
                using (StreamWriter sw2 = File.AppendText(@"C:\LOG\log_saveValue.txt"))
                {
                    sw2.WriteLine(System.DateTime.Now.ToString() + "," + msg);
                    sw2.Close();
                }
            }
            catch (Exception ex)
            {
                WriteSpeedLog("保存流速日志错误：" + ex.ToString());
            }
        }
        private void skinTabPage1_MouseDown(object sender, MouseEventArgs e)
        {
            //如果按下去的按钮不是左键就return,节省运算资源
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //按下鼠标后,进入拖动状态:
            IsDragging = true;
            //保存刚按下时的鼠标坐标
            StartPoint.X = e.X;
            StartPoint.Y = e.Y;
        }

        private void skinTabPage1_MouseUp(object sender, MouseEventArgs e)
        {
            //左键抬起时,及时把拖动判定设置为false,否则,你也可以试试效果
            IsDragging = false;
        }

        private void skinTabPage1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging == true)
            {
                //用当前坐标减去起始坐标得到偏移量Offset
                OffsetPoint.X = e.X - StartPoint.X;
                OffsetPoint.Y = e.Y - StartPoint.Y;
                if (OffsetPoint.X < -1 || OffsetPoint.X>1)
                {
                    content.SelectedIndex = 2;
                    mTabCurrentIndex = 2;
                }
               
            }           
        }
        private void skinTabPage3_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging == true)
            {
                //用当前坐标减去起始坐标得到偏移量Offset
                OffsetPoint.X = e.X - StartPoint.X;
                OffsetPoint.Y = e.Y - StartPoint.Y;
                if (OffsetPoint.X < -1 || OffsetPoint.X > 1)
                {
                    content.SelectedIndex = 0;
                    mTabCurrentIndex = 0;
                }

            }           
        }

        private void skinTabPage3_MouseDown(object sender, MouseEventArgs e)
        {
            //如果按下去的按钮不是左键就return,节省运算资源
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //按下鼠标后,进入拖动状态:
            IsDragging = true;
            //保存刚按下时的鼠标坐标
            StartPoint.X = e.X;
            StartPoint.Y = e.Y;
        }

        private void skinTabPage3_MouseUp(object sender, MouseEventArgs e)
        {
            IsDragging = false;
        }

    

  

        private void adjustData_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void adjustData_Enter(object sender, EventArgs e)
        {
            try
            {
                dynamic file = @"TabTip.exe";
                if (!System.IO.File.Exists(file))
                    return;
                System.Diagnostics.Process.Start(file);
            }
            catch (Exception)
            {
            }
        }
                      
        //----------------------------------------------------------------COD
        private void panelCOD_Click(object sender, EventArgs e)
        {
            currentUnitIndex = 0;
            pressResultUnit(titleCOD.Text);
        }

        private void titleCOD_Click(object sender, EventArgs e)
        {
            panelCOD_Click(null, null);
        }

        private void label_COD_value_Click(object sender, EventArgs e)
        {
            panelCOD_Click(null, null);
        }

        private void unitCOD_Click(object sender, EventArgs e)
        {
            panelCOD_Click(null, null);
        }


        //----------------------------------------------------------------NH4
        private void panelNH4_Click(object sender, EventArgs e)
        {
            currentUnitIndex = 1;
            pressResultUnit(titleNH4.Text);
        }

        private void titleNH4_Click(object sender, EventArgs e)
        {
            panelNH4_Click(null, null);
        }

        private void label_NH4_value_Click(object sender, EventArgs e)
        {
            panelNH4_Click(null, null);
        }

        private void unitNH4_Click(object sender, EventArgs e)
        {
            panelNH4_Click(null, null);
        }

        //----------------------------------------------------------------SC
        private void panelSC_Click(object sender, EventArgs e)
        {
            currentUnitIndex = 2;
            pressResultUnit(titleSC.Text);
        }

        private void titleSC_Click(object sender, EventArgs e)
        {
            panelSC_Click(null, null);
        }

        private void label_SC_value_Click(object sender, EventArgs e)
        {
            panelSC_Click(null, null);
        }

        private void unitSC_Click(object sender, EventArgs e)
        {
            panelSC_Click(null, null);
        }

        //----------------------------------------------------------------电导率
        private void panelConductivity_Click(object sender, EventArgs e)
        {
            currentUnitIndex = 3;
            pressResultUnit(titleConductivity.Text);
        }

        private void titleConductivity_Click(object sender, EventArgs e)
        {
            panelConductivity_Click(null, null);
        }

        private void label_Conductivity_value_Click(object sender, EventArgs e)
        {
            panelConductivity_Click(null, null);
        }

        private void unitConductivity_Click(object sender, EventArgs e)
        {
            panelConductivity_Click(null, null);
        }



        private void titleNH4_1_Click(object sender, EventArgs e)
        {
            panelNH4_1_Click(null, null);
        }

        private void label_NH4_1_value_Click(object sender, EventArgs e)
        {
            panelNH4_1_Click(null, null);

        }

        private void unitNH4_1_Click(object sender, EventArgs e)
        {
            panelNH4_1_Click(null, null);

        }

        private void panelNH4_1_Click(object sender, EventArgs e)
        {
            currentUnitIndex = 4;
            pressResultUnit(titleNH4_1.Text);
        }










        
        //--------------------------------------------------------------
        private void pressResultUnit(string name)//XX最近24小时数据曲线
        {
            historyIndex = 0;
            historyTitleRight.Text="最近24小时数据曲线";
            historyTitleLeft.Text = name;
            //float[] y;
            //int length = readValueBeforeDraw(savedHistoryFileTitle[currentUnitIndex], out y);
            //int num = length;
            //float[] x = new float[num];           
            //Random ran = new Random();
            //for (int i = 0; i < num; i++)
            //{
            //    x[i] = i;
            //   // y[i] = ran.Next(0, 100);
            //}
            //draw24HoursChart(num,x,y);
            Thread thread = new Thread(Show24HoursThread);
            thread.Start();//启动新线程 
        }
         public void   Show24HoursThread()//辅助线程对应的回调方法中跨线程调用this.Invoke
         {
            switch (currentUnitIndex)
            {
                
                case 0://COD
                    this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveCOD);
                    break;
                case 1://NH4N
                    this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveNH4_N);
                    break;
                case 2://SC
                    this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveSC);
                    break;                
                case 3://电导率
                    this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveConductivity);
                    break;
                case 4://氨氮-1
                    this.Invoke(new Update24HoursCurveDelegate(draw24HoursChart), _24HourCurveTime.Count, _24HourCurveTime, _24HourCurveNH4_N_1);
                    break;
            }

        }
        private void showHintMsg(string msg)
        {
          //  MSGShow frm = new MSGShow();
            if (msgShowFrm != null && !msgShowFrm.IsDisposed)
            {
                msgShowFrm.msgBoard.Text = msg;
                msgShowFrm.Visible = true;
                //msgShowFrm.StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                msgShowFrm = new MSGShow();
                msgShowFrm.Show();
                msgShowFrm.msgBoard.Text = msg;
                msgShowFrm.Visible = true;
            }
          //  frm.Show();
            //Thread.Sleep(1000);
           // frm = null;
           // GC.Collect();
           // frm.Show();
           // Thread thread = new Thread(ShowHintMsgThread);
           // thread.Start(frm);//启动新线程           
        }
        public void ShowHintMsgThread(object obj)
        {           
            string msg = obj.ToString();           
            MSGShow frm = (MSGShow)obj;
            //frm.Show();
            frm.msgBoard.Text = msg;
            frm.TopMost = true;
            frm.Show();
            frm.Hide();
            Thread.Sleep(2000);
            frm = null;
            GC.Collect();          

        }
        public static int readFileLines(string path)  //这里的参数是txt所在路径
        {
            int lines = 0;  //用来统计txt行数
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            while (sr.ReadLine() != null)
            {
                lines++;
            }
            fs.Close();
            sr.Close();
            return lines;
        }

        //返回值多个，不限类型用out参数 out修饰的变量不需要return，必须在方法内为其赋值
        private int readValueBeforeDraw(string valueTitle,out DateTime[] ftime,out float[] fParaData)
        {
            string filename = @"C:\测试数据\" + valueTitle + ".txt";
            int lines   = readFileLines(filename);//读取文件行数=数据个数
            int length  = 1;
            fParaData   = new float[lines];//Y轴数据个数
            ftime       = new DateTime[lines];  //X轴时间个数       
            try
            {                
                int startPoint = 0;                 
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    string line;
                    line = sr.ReadLine();//时间，数据
                    string[] strArray1 = line.Split(',');//用逗号截取字符串 第一个元素为时间，第二个元素为数据
                    ftime[0] = DateTime.Parse(strArray1[0]);      
                    //历史文件中的第一个数赋给X,Y
                    fParaData[0] = float.Parse(strArray1[1]);
                    
                    line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] strArray2 = line.Split(',');
                        ftime[length] = DateTime.Parse(strArray2[0]);
                        fParaData[length] = float.Parse(strArray2[1]);                                              
                        length++;
                        line = sr.ReadLine();
                        strArray2 = null;//释放数组资源
                        GC.Collect();
                    }

                    sr.Close();
                    fs.Close();
                    strArray1 = null;
                    GC.Collect();
                }
                return length;
                
            }
            catch (Exception ex)
            {
                WriteSensorLog("传感器数值文件读取错误:"+ex.ToString());
               
                return -1;
            }

        }
        private void drawHistoryChart(int length, DateTime[] fxParaData, float[] fyParaData)
        {
            try
            {
                historyChart.Series.Clear();
                Series spectrumSeries = new Series("历史数据");
                spectrumSeries.ChartType = SeriesChartType.Line;
                spectrumSeries.XValueType = ChartValueType.DateTime;
                float maxy, miny;
                maxy = miny = fyParaData[0];
                for (int i = 0; i < length; i++)
                {
                    if (i % 1 == 0)
                    {
                        spectrumSeries.Points.AddXY(fxParaData[i], fyParaData[i]);
                        if (fyParaData[i] > maxy) maxy = fyParaData[i];
                        if (fyParaData[i] < miny) miny = fyParaData[i];
                    }
                }               
                historyChart.Series.Add(spectrumSeries);
                historyChart.Series[0].Color = Color.White;
                historyChart.Series[0].BorderWidth = 2;
                historyChart.Series[0].XValueType = ChartValueType.DateTime;
               //historyChart.ChartAreas[0].AxisX.MajorGrid.Interval = (fxParaData[length-1]-fxParaData[0]) / 4;
                if (miny != maxy)
                {
                    historyChart.ChartAreas[0].AxisY.Minimum = miny;
                    historyChart.ChartAreas[0].AxisY.Maximum = maxy;
                }
                if (fxParaData[0] != fxParaData[length - 1])
                {
                    historyChart.ChartAreas[0].AxisX.Minimum = fxParaData[0].ToOADate();
                    historyChart.ChartAreas[0].AxisX.Maximum = fxParaData[length - 1].ToOADate();
                }
               //historyChart.ChartAreas[0].AxisY.MajorGrid.Interval = (maxy - miny) / 5;              
            }
            catch (Exception ex)
            {
                WriteSensorLog("绘制历史曲线错误！"+ex.ToString()); 
                
            }
        }
        private void draw24HoursChart(int length, List<DateTime> fxParaData, List<float> fyParaData)
        {
            try
            {
                historyChart.Series.Clear();//清除数据序列
                Series spectrumSeries = new Series("光谱");
                spectrumSeries.ChartType = SeriesChartType.Line;//折线图
                spectrumSeries.Points.Clear();
                spectrumSeries.XValueType = ChartValueType.DateTime;
                float maxy, miny;
                maxy = miny = fyParaData[0];
                for (int i = 0; i < length; i++)
                {
                    if (i % 1 == 0)
                    {
                        spectrumSeries.Points.AddXY(fxParaData[i], fyParaData[i]);
                        if (fyParaData[i] > maxy) maxy = fyParaData[i];//遍历后最大值设置为最大
                        if (fyParaData[i] < miny) miny = fyParaData[i];//最小值设置为最小
                    }
                }
                historyChart.Series.Add(spectrumSeries);
                historyChart.Series[0].Color = Color.White;
                historyChart.Series[0].BorderWidth = 2;
                historyChart.Series[0].XValueType = ChartValueType.DateTime;
                //historyChart.ChartAreas[0].AxisX.MajorGrid.Interval = (fxParaData[length-1]-fxParaData[0]) / 4;
                if (miny != maxy)
                {
                    historyChart.ChartAreas[0].AxisY.Minimum = miny;//Y轴最小值
                    historyChart.ChartAreas[0].AxisY.Maximum = maxy;//Y轴最大值
                }
                if (fxParaData[0] != fxParaData[fxParaData.Count - 1])
                {
                    historyChart.ChartAreas[0].AxisX.Minimum = fxParaData[0].ToOADate();//X轴时间最小值
                    historyChart.ChartAreas[0].AxisX.Maximum = fxParaData[fxParaData.Count - 1].ToOADate();//X轴时间最大值
                }
                //historyChart.ChartAreas[0].AxisY.MajorGrid.Interval = (maxy - miny) / 5;              
            }
            catch (Exception ex)
            {
                WriteSensorLog("绘制24小时曲线错误:" + ex.ToString());

            }
        }

        private void previousDay_Click(object sender, EventArgs e)//前一天按钮事件，绘制历史曲线时先读取保存的历史数据文件，再调用绘图函数
        {           
            historyTitleRight.Text = "单日数据曲线";
            historyIndex--;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;            
            string time = currentTime.AddDays(historyIndex).ToLongDateString();
            string filename = @"C:\测试数据\_" + savedHistoryFileTitle[currentUnitIndex] + "_" + time + ".txt";           
            if (!File.Exists(filename))
            {
                historyIndex++;
                showHintMsg("前面数据不存在");
                return;
            }
            float[] y;
            DateTime[] x;
            int length = readValueBeforeDraw("_" + savedHistoryFileTitle[currentUnitIndex] + "_" + time, out x, out y);//在readValueBeforeDraw方法内部为 x和y数组赋值 这些参数无需return，length为数据个数

            drawHistoryChart(length, x, y);
            historyTimeSpan.Text = time;
            
        }

        private void nextDay_Click(object sender, EventArgs e)//后一天按钮事件
        {
            historyTitleRight.Text = "单日数据曲线";
            historyIndex++;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string time = currentTime.AddDays(historyIndex).ToLongDateString();
            string filename = @"C:\测试数据\_" + savedHistoryFileTitle[currentUnitIndex] + "_" + time + ".txt";
            if (!File.Exists(filename))
            {
                historyIndex--;
                showHintMsg("后面数据不存在");
                return;
            }
            float[] y;
            DateTime[] x;
            int length = readValueBeforeDraw("_" + savedHistoryFileTitle[currentUnitIndex] + "_" + time,out x, out y);

            drawHistoryChart(length, x, y);
            historyTimeSpan.Text = time;
            
        }

        public void RefreshComList(bool refreshsensor,bool refreshspectrum,bool refreshspeed,bool refreshPLC)//更新串口下拉菜单。
        {
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                if (refreshsensor)
                this.comSensor.Items.Clear();

                foreach (string sName in sSubKeys)
                {
                    string sValue = (string)keyCom.GetValue(sName);

                    if (refreshsensor)
                    this.comSensor.Items.Add(sValue);
                    if (refreshspeed)

                    this.comPLC.Items.Add(sValue);

                }
                if (refreshspectrum && refreshsensor && refreshspeed)
                {
                    try
                    {
                        this.comSensor.SelectedIndex = 0;
                        this.baudRateSensor.SelectedIndex = 0;
                                            

                        this.comPLC.SelectedIndex = 3;
                        this.baudRatePLC.SelectedIndex = 0;
                       
                    }
                    catch
                    {
                        showHintMsg("请确定串口是否正确连接!");
                    }
                }
                
            }
        }





       

        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.INIfilePathName);
        }
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.INIfilePathName);
            return temp.ToString();
        }
        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(INIfilePathName);
        }
        private void ReadAllINIFile()
        {
            string sensorCom = IniReadValue("SERIALPORTSETTING", "SensorCOM");
            if (sensorCom.Trim() == "") sensorCom = "0";
            try
            {
                comSensor.SelectedIndex = int.Parse(sensorCom);
            }
            catch
            {
                showHintMsg("传感器串口号不正确，请检查！");
            }
           

            //读取传感器查询间隔--------------------------------------------------
            string sensorinterval = IniReadValue("SENSORSETTING", "SensorInterval");
            if (sensorinterval.Trim() == "") sensorinterval = "12000";
            sensorInterval.Text = sensorinterval;
            iIntervalSensor= Convert.ToInt32(sensorinterval);
            

            //读取是否自动采集传感器数据------------------------------------------------------
            string tmp = IniReadValue("SENSORSETTING", "AutoCaptureSensorData");
            if (tmp.Trim() == "") tmp = "False";
            AutoCaptureSensorData.Checked = Convert.ToBoolean(tmp);//ini文件中AutoCaptureSensorData=true，则将字符串类型的true转化为bool类型的true后，选中下次开机自动连续采集按钮
            bAutoCaptureSensorData = AutoCaptureSensorData.Checked;


            //1读取是否校正COD------------------------------------------------------
            tmp = IniReadValue("SPECTRUMSETTING", "CorrectCOD");
            if (tmp.Trim() == "") tmp = "False";
            bCorrectCOD = Convert.ToBoolean(tmp);

            //1读取COD校正值K------------------------------------------------------
            tmp = IniReadValue("SPECTRUMSETTING", "CorrectValueCODK");
            if (tmp.Trim() == "") tmp = "0";
            correctValueCODK = Convert.ToSingle(tmp);

            //1读取COD校正值B------------------------------------------------------
            tmp = IniReadValue("SPECTRUMSETTING", "CorrectValueCODB");
            if (tmp.Trim() == "") tmp = "0";
            correctValueCODB = Convert.ToSingle(tmp);

         


            //读取本机地址名称------------------------------------------------------
            tmp = IniReadValue("SYSTEM", "LocalAddressName");
            if (tmp.Trim() == "") tmp = "";
            mLocalAddressName = tmp;
            currentTitleName.Text = mLocalAddressName;

            //读取本机地址编码------------------------------------------------------
            tmp = IniReadValue("SYSTEM", "LocalAddressCode");
            if (tmp.Trim() == "") tmp = "1";
            mLocalAddressCode = tmp;

            //读取是否自动打开PLC外部读取端口-----------------------------------
            tmp = IniReadValue("PLCSETTING", "AutoOpenPLCCOM");
            if (tmp.Trim() == "") tmp = "False";
            AutoOpenPLCCOM.Checked = Convert.ToBoolean(tmp);
            bAutoOpenPLCCOM = AutoOpenPLCCOM.Checked;



            //读取云服务器网络地址-----------------------------------------------
            tmp = IniReadValue("CLOUDSETTING", "CloudServerIP");
            if (tmp.Trim() == "") tmp = "tcp.dtuip.com";
            cloudServerIP.Text = tmp;
            mCloudServerIPAddress = tmp;

            //读取云服务器网络端口号-----------------------------------------------
            tmp = IniReadValue("CLOUDSETTING", "CloudServerPort");
            if (tmp.Trim() == "") tmp = "6647";
            cloudServerPort.Text = tmp;
            mCloudServerPortNum = tmp;

            //读取各设备序列号-----------------------------------------------
            tmp = IniReadValue("CLOUDSETTING", "SerialNumber");
            if (tmp.Trim() == "") tmp = "";
            serialNumber.Text = tmp;
            mSerialNumber = tmp;


            //读取是否自动将数据上传至云服务器-----------------------------------
            tmp = IniReadValue("CLOUDSETTING", "AutoSendData2Cloud");
            if (tmp.Trim() == "") tmp = "False";
            AutoSendData2Cloud.Checked = Convert.ToBoolean(tmp);
            bAutoSendData2Cloud = AutoSendData2Cloud.Checked;

            
            //读取PLC外部读取串口号---------------------------------------------
            tmp = IniReadValue("SERIALPORTSETTING", "PLCCOM");
            if (tmp.Trim() == "") tmp = "0";
            try
            {
                comPLC.SelectedIndex = int.Parse(tmp);
            }
            catch
            {
                showHintMsg("PLC串口号不正确，请检查！");
            }


            //读取传感器串口号---------------------------------------------
            tmp = IniReadValue("SERIALPORTSETTING", "SensorCOM");
            if (tmp.Trim() == "") tmp = "0";
            try
            {
                comSensor.SelectedIndex = int.Parse(tmp);
            }
            catch
            {
                showHintMsg("传感器串口号不正确，请检查！");
            }

            //读取传感器波特率---------------------------------------------

            tmp = IniReadValue("SERIALPORTSETTING", "SensorBaudRate");
            if (tmp.Trim() == "") tmp = "0";
            try
            {
                int sensorrate = Convert.ToInt32(tmp);
                baudRateSensor.SelectedIndex = sensorrate;
            }
            catch
            {
                baudRateSensor.SelectedIndex = 0;
            }



            //外部PLC波特率-----------------------------------------------------------
            tmp = IniReadValue("SERIALPORTSETTING", "PLCBaudRate");
            if (tmp.Trim() == "") tmp = "0";
            try
            {
                int plcrate = Convert.ToInt32(tmp);
                baudRatePLC.SelectedIndex = plcrate;//若ini文件中PLCBaudRate=2，即表示上次保存的PLC波特率的combox索引值为2即第三个（9600）
            }
            catch
            {
                 baudRatePLC.SelectedIndex = 0;
            }
                     
        }
      

        private void btnOpenComSensor_Click(object sender, EventArgs e)
        {
            IniWriteValue("SERIALPORTSETTING", "SensorCOM", comSensor.SelectedIndex.ToString());//写入传感器当前串口号至ini文件
            IniWriteValue("SERIALPORTSETTING", "SensorBaudRate", baudRateSensor.SelectedIndex.ToString());//写入传感器当前波特率至ini文件中
         //   IniWriteValue("SERIALPORTSETTING","comSensor",comSensor.Text);
            if (btnOpenComSensor.Text == "打开串口")
            {
                if (serialPortSensor == null) serialPortSensor = new SerialPort();
                bool rt=OpenSerialPortSensor(comSensor.Text, baudRateSensor.Text);
                if (rt)
                {
                    comSensor.Enabled = false;
                    baudRateSensor.Enabled = false;
                    
                    refreshCOMSensor.Enabled = false;
                    btnOpenComSensor.Text = "关闭串口";
                }
            }
            else
            {                
                if (serialPortSensor.IsOpen)
                {
                    try
                    {
                        serialPortSensor.Close();
                        comSensor.Enabled = true;
                        baudRateSensor.Enabled = true;
                       
                        refreshCOMSensor.Enabled = true;
                        btnOpenComSensor.Text = "打开串口";
                    }
                    catch
                    {
                        showHintMsg("串口关闭错误！");
                    }   
                }
                else
                {
                    comSensor.Enabled = true;
                    baudRateSensor.Enabled = true;
                   
                    refreshCOMSensor.Enabled = true;
                    btnOpenComSensor.Text = "打开串口";
                    serialPortSensor.Dispose();
                    serialPortSensor = null;
                    GC.Collect();
                }
            }
        }

        private void CaptureSensorContinuesBackUp_Click(object sender, EventArgs e)
        {
            CaptureSensorDataContinuous_Click(null,null);
            if (CaptureSensorDataContinuous.Text == "连续采集")
            {
                CaptureSensorContinuesBackUp.Text = "开启传感器连续采集";
            }
            else
            {
                CaptureSensorContinuesBackUp.Text = "停止传感器连续采集";
            }

        }
        private void CaptureSensorDataContinuous_Click(object sender, EventArgs e)
        {
            IniWriteValue("SENSORSETTING", "SensorInterval", sensorInterval.Text);
            iIntervalSensor = int.Parse(sensorInterval.Text);
            if (CaptureSensorDataContinuous.Text == "连续采集")
            {
                if (btnOpenComSensor.Text == "打开串口") btnOpenComSensor_Click(null, null);
                if (serialPortSensor.IsOpen)
                {
                    timer2Count = 0;
                    CaptureSensorDataContinuous.Text = "关闭采集";
                    CaptureSensorContinuesBackUp.Text = "停止传感器连续采集";                    
                    //  CaptureData2("1");
                   // TimerUp(null, null);
                    timer.Start();
                    QuerySensorStop = true;
                    Thread thread = new Thread(threadCaptureSensror);
                    thread.Start();//启动新线程
                }
            }
            else
            {
                QuerySensorStop = false;
                timer.Stop();
                CaptureSensorDataContinuous.Text = "连续采集";
                CaptureSensorContinuesBackUp.Text = "开启传感器连续采集";
            }
            //mask masdlg = new mask();          
            //masdlg.Show();
        }
        public void threadCaptureSensror()//待改  读取6个寄存器共12个字节
        {
            while (QuerySensorStop)
            {
                timer2Count++;
                if (timer2Count > Math.Pow(2, 60)) timer2Count = 0;//防止计数器溢出
                // Thread.Sleep(500);//850

                sendSerialPortCMD(1, "01 03 00 00 00 02 C4 0B");//COD地址01 
                mAutoQuestOverEvent.WaitOne(5000);
                Thread.Sleep(200);

                sendSerialPortCMD(2, "02 03 00 10 00 02 C5 FD");//氨氮地址02  寄存器地址16
                mAutoQuestOverEvent.WaitOne(5000);
                Thread.Sleep(200);

                sendSerialPortCMD(3, "03 03 00 02 00 02 64 29");//悬浮物SS地址03   
                mAutoQuestOverEvent.WaitOne(5000);
                Thread.Sleep(200);

                sendSerialPortCMD(4, "04 03 00 02 00 02 65 9E");//电导率地址04 
                mAutoQuestOverEvent.WaitOne(5000);
                Thread.Sleep(200);

                sendSerialPortCMD(5, "05 03 00 10 00 02 C4 4A");//氨氮-1地址05  寄存器地址16
                mAutoQuestOverEvent.WaitOne(5000);

                Thread.Sleep(iIntervalSensor);//传感器间隔12S一次
               
            }
        }
        


        private void label_Time_Click(object sender, EventArgs e)
        {
            //double[,] data = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
            //Matrix A = new Matrix(data);
            //Matrix B = new Matrix(data);
            //Matrix C = new Matrix(3,3);
            //B=A.Transpose();
            //C = A*B;
            //double c11=C[1,1];
            //double B12 = c11; 
            if (!bMaskFrmisVisible)
            {
                if (maskFrm != null && !maskFrm.IsDisposed)
                {
                    maskFrm.Visible = true;
                    bMaskFrmisVisible = true;
                }
                else
                {
                    maskFrm = new mask();
                    maskFrm.Owner = this;
                    maskFrm.Show();
                    maskFrm.Visible = true;
                    bMaskFrmisVisible = true;
                }
            }
            else 
            {
                if (maskFrm != null && !maskFrm.IsDisposed)
                {
                    maskFrm.Visible = false;
                    bMaskFrmisVisible = false;
                }
                else
                {
                    maskFrm = new mask();
                    maskFrm.Owner = this;
                    maskFrm.Show();
                    maskFrm.Visible = false;
                    bMaskFrmisVisible = false;
                }
            }                       
        }
   


       
        private void SaveSingleCurve()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "存储数据（*.txt）|*.txt|所有类型（*.*）|*.*";
            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //设置默认的文件名

            //sfd.de = "YourFileName";// in wpf is  sfd.FileName = "YourFileName";

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                MessageBox.Show(localFilePath);

            }
        }
        private void SaveWaveDataDlg(UInt16[] value)
        {
            //string localFilePath, fileNameExt, newFileName, FilePath; 
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "存储数据（*.txt）|*.txt|所有类型（*.*）|*.*";
            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //设置默认的文件名

            //sfd.de = "YourFileName";// in wpf is  sfd.FileName = "YourFileName";

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                using (StreamWriter sw2 = File.CreateText(localFilePath))//日志
                {
                    for (int i=0; i < value.Length; i++)
                    {
                        sw2.WriteLine(value[i]);
                    }
                    sw2.Close();
                }
                MessageBox.Show("保存成功！");

            }
        }
        public void SaveWaveThread(string route ,UInt16[] value)
        {
            saveStruct sstruct = new saveStruct();
            sstruct.routeAorB = route;
            sstruct.value = value;
            Thread thread = new Thread(SaveWaveDataAuto);
            thread.Start((object)sstruct);//启动新线程
        }
        private void SaveWaveDataAuto(object obj)
        {
            saveStruct sstruct=(saveStruct)obj;
            string RouteAorB=sstruct.routeAorB;
            UInt16[] value =sstruct.value;
            
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string time = currentTime.ToLongDateString() + "_" + currentTime.ToLongTimeString();
            time=time.Replace(':','_');
            string filename = @"C:\"+RouteAorB+"_"+time+".txt";
            try
            {
                using (StreamWriter sw = File.CreateText(filename))//日志
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        sw.WriteLine(value[i]);
                    }
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                WriteSpectrumLog("保存光谱曲线出现错误:" + ex.ToString());
            }
        }

        private void SaveARoute_Click(object sender, EventArgs e)
        {
           SaveWaveDataDlg(receiveDataA);
        }

        private void SaveBRoute_Click(object sender, EventArgs e)
        {
            SaveWaveDataDlg(receiveDataB);
        }
        private void CreateFolder(string path)
        {
            //string path = @"C:\测试数据";
            if (!File.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create();
            }
        }
        //private void calculateSpeedKandB(float dnMax,float dnMin,float vMax,float vMin,out float k,out float b)//用于计算流速传感器的DN值向流速(L/min)值的K和b
        //{
        //    k = 1;
        //    b = 0;
      

        //}



        private void correctCMDSend(byte id,byte address0, byte address1,string textvalue)
        {
            //10为写浮点数
            int length = 13;//pH修正指令:01为传感器地址/ 10为修正的功能码/ 00 0C为寄存器地址/ 00 02为寄存器个数/ 04为写入的字节数/ 后面4个字节为要写入的偏差值/ 最后两个字节为CRC校验码
            //string cmd = "01 10 00 0C 00 02 04";
            byte[] waiteforCorrect = new byte[length - 2];
            byte[] cmdbyte = new byte[length];
            cmdbyte[0] = id;
            cmdbyte[1] = 0x10;
            cmdbyte[2] = address0;
            cmdbyte[3] = address1;// 0x0C;
            cmdbyte[4] = 0x00;
            cmdbyte[5] = 0x02;
            cmdbyte[6] = 0x04;
            float floatValue = Convert.ToSingle(textvalue);
            byte[] bytes = BitConverter.GetBytes(floatValue);//小端模式，低字节放在低地址 ，一般为小端模式 float4个字节ABCD对应数组索引为3210，发送时按照CDAB格式发送，即索引按照1032发送
            cmdbyte[7] = bytes[1];
            cmdbyte[8] = bytes[0];
            cmdbyte[9] = bytes[3];
            cmdbyte[10] = bytes[2];
            Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
            byte[] crcreturn = CRC16(waiteforCorrect);
            cmdbyte[11] = crcreturn[0];
            cmdbyte[12] = crcreturn[1];
            string cmdstr = BitConverter.ToString(cmdbyte).Replace("-", " ");//data[i].ToString("X");
            //  MessageBox.Show(cmdstr);
            sendSerialPortCMD(101, cmdstr);
        }

        private void AutoCaptureSensorData_CheckedChanged(object sender, EventArgs e)
        {
            IniWriteValue("SENSORSETTING", "AutoCaptureSensorData", AutoCaptureSensorData.Checked.ToString());
        }


        private void refreshCOMSensor_Click(object sender, EventArgs e)
        {
            RefreshComList(true, false, false,false);//搜索端口号
        }

        private void refreshCOMSpectrum_Click(object sender, EventArgs e)
        {
            RefreshComList(false, true, false,false);//搜索端口号
        }

        private void refreshCOMSpeed_Click(object sender, EventArgs e)
        {
            RefreshComList(false, false, true,false);//搜索端口号
        }

        private void UpdateDarkOrLight(int dnValue)
        {
           
           if(dnValue>2500)
           {
               if (!bMaskFrmisVisible)
               {
                   if (maskFrm != null && !maskFrm.IsDisposed)
                   {
                       try
                       {
                           maskFrm.Visible = true;
                       }
                       catch(Exception ex)
                       {
                           WriteSpeedLog("线程中屏幕变暗错误："+ex.ToString());
                       }
                       bMaskFrmisVisible = true;
                   }
                   else
                   {
                       try
                       {
                           maskFrm = new mask();
                           maskFrm.Owner = this;
                           maskFrm.Show();
                           maskFrm.Visible = true;
                       }
                       catch(Exception ex)
                       {
                           WriteSpeedLog("线程中屏幕变暗新建错误：" + ex.ToString());
                       }
                   }
               }
           }
           else
           {
               if (bMaskFrmisVisible)
               {
                   if (maskFrm != null && !maskFrm.IsDisposed)
                   {
                       try
                       {
                           maskFrm.Visible = false;
                       }
                       catch (Exception ex)
                       {
                           WriteSpeedLog("线程中屏幕点亮错误：" + ex.ToString());
                       }
                       bMaskFrmisVisible = false;
                   }
                   else
                   {
                       maskFrm = new mask();
                       maskFrm.Owner = this;
                       maskFrm.Show();
                       maskFrm.Visible = true;
                   }
               }
           }
        }
        public void threadCalculateUV254AndCOD(UInt16[] Ia,UInt16[] Ib)//待改
        {
            uv254Struct ss = new uv254Struct();
            ss.valueA = Ia;
            ss.valueB = Ib;
            Thread thread = new Thread(CalculateUV254AndCOD);
            thread.Start(ss);//启动新线程
        }

        private void  CalculateUV254AndCOD(object obj)//待改
        {
            /*
            int offset = 0;
            if(m_spectrumCommunicationGeneration==1)
            {
                offset = 0;
            }
            else if(m_spectrumCommunicationGeneration==2)
            {
                offset = 1;
            }
            else { offset = 0; }
            uv254Struct ss=(uv254Struct)obj;
            UInt16[] Ia = ss.valueA;
            UInt16[] Ib = ss.valueB;
            double[] A = new double[Ia.Length];
            double[] B = new double[Ib.Length];
            for(int i=0;i<Ia.Length;i++)
            {
                if (Ia[i] == 0) Ia[i] = 1;
                if (Ib[i] == 0) Ib[i] = 1;
                A[i] = Math.Log10((double)1 / (double)Ia[i]) - Math.Log10((double)1 / (double)Ib[i]);
            }
            B = smoothFilter(A, A.Length);

            double UV254 = B[500 - 1 - offset];
            double COD = 119.235432818573 * B[430 - 1 - offset] - 27.8714508523335;

            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 6, (float)UV254);
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 8, (float)COD);

            double Croma = 7.83851269900049 * B[520 - 1 - offset] + 1.34165438839708;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 9, (float)Croma);

            double NO3N = 270.799758646219 * B[370 - 1 - offset] - 60.0550730129927;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 10, (float)NO3N);

            double NH4N = 607.654867736028 * B[189 - 1 - offset] + 1.37059868276756;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 11, (float)NH4N);

            double NO2N = 0f;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 12, (float)NO2N);

            double PO4P = -22.5642693768946 * B[193 - 1 - offset] + 1.80176816704141;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 13, (float)PO4P);

            double TOC = 67.9623080294772 * B[464 - 1 - offset] + 11.3398098070108;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 14, (float)TOC);

            double DO = 0;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 15, (float)DO);

            double BOD = -0.00798619313662574 * B[170 - 1 - offset] + 40.5458669806738;
            this.Invoke(new UpdateValueDelegate(UpdateValueFunction), 16, (float)BOD);*/

        }
        private double[] smoothFilter(double[] A,int N)
        {
            double[] outputSignal = new double[A.Length];
            for(int i=0;i<A.Length;i++)
            {
                if(i<(N-1))
                {
                    outputSignal[i] = sum(A, 0, i) / (i+1);
                }
                else if(i<(A.Length-N+1))
                {
                    outputSignal[i] = sum(A, i-(int)Math.Ceiling((double)(N-1)/2), i+(int)Math.Ceiling((double)(N-1)/2) )/ i;
                }
                else
                {
                    outputSignal[i] = sum(A, i, A.Length - 1) / (A.Length - i);
                }              
            }  
            return outputSignal;
        }
        private double sum(double[] array,int start,int end)
        {
            double sumnum=0;
            for(int i=start;i<end;i++)
            {
                sumnum += array[i];
            }
            return sumnum;
        }


        protected override void WndProc(ref Message m)//U盘拷贝的响应函数-----------
        {
            try
            {
                
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                if (drive.DriveType == DriveType.Removable)
                                {
                                  //  listBox1.Items.Add("U盘已插入，盘符为:" + drive.Name.ToString());
                                    if (uDiskFrm != null && !uDiskFrm.IsDisposed)
                                    {
                                        uDiskFrm.SetTextContent("U盘已插入，盘符为:", drive.Name.ToString());
                                        uDiskFrm.Visible = true;
                                    }
                                    else
                                    {
                                        uDiskFrm = new UDiskCopyDlg();
                                        uDiskFrm.Show();
                                        uDiskFrm.SetTextContent("U盘已插入，盘符为:", drive.Name.ToString());
                                        uDiskFrm.Visible = true;
                                    }
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            //listBox1.Items.Add("U盘已经拔出");
                            if (uDiskFrm != null && !uDiskFrm.IsDisposed)
                            {
                                uDiskFrm.SetTextContent("U盘已拔出!", "");
                                uDiskFrm.Visible = false;
                            }
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }

  
    
   
        public void MessageBoxOut(string str)
        {
            MessageBoxFrm frm = new MessageBoxFrm();
            frm.setMsgContent(str);
            frm.ShowDialog();
        }

       public void setHistoryDateMessage(string left,DateTime start,DateTime end)
       {
           historyTitleRight.Text = left;
           historyTimeSpan.Text = start.ToString("yyyy年MM月dd日HH时mm分") + "-" + end.ToString("yyyy年MM月dd日HH时mm分");
       }

       private void setLocalAddress_Click(object sender, EventArgs e)
       {
           setLocalAddressDlg dlg = new setLocalAddressDlg();
           dlg.setLocalAddress(mLocalAddressCode,mLocalAddressName);
           DialogResult dr = dlg.ShowDialog();
           if (dr == DialogResult.OK)
           {
               mLocalAddressName = dlg.newLocalAddressName;
               IniWriteValue("SYSTEM", "LocalAddressName", mLocalAddressName);
               mLocalAddressCode = dlg.newLocalAddressCode;
               IniWriteValue("SYSTEM", "LocalAddressCode", mLocalAddressCode);
               MessageBoxFrm frm = new MessageBoxFrm();
               frm.setMsgContent("本机地址已经修改成功！");
               frm.ShowDialog();
               return;
           }
       }

       private void refreshCOMPLC_Click(object sender, EventArgs e)
       {
           RefreshComList(false, false, false, true);//搜索端口号
       }

       private void btnOpenComPLC_Click(object sender, EventArgs e)
       {
           IniWriteValue("SERIALPORTSETTING", "PLCCOM", comPLC.SelectedIndex.ToString());
           IniWriteValue("SERIALPORTSETTING", "PLCBaudRate", baudRatePLC.SelectedIndex.ToString());
           if (btnOpenComPLC.Text == "打开串口")
           {
               if (serialPortPLC == null) serialPortPLC = new SerialPort();
               OpenSerialPortPLC(comPLC.Text, baudRatePLC.Text);
               if (serialPortPLC.IsOpen)
               {
                   comPLC.Enabled = false;
                   baudRatePLC.Enabled = false;

                   refreshCOMPLC.Enabled = false;
                   btnOpenComPLC.Text = "关闭串口";
               }
               else
               {
                   showHintMsg("打开PLC端口失败，请检查所选端口是否被占用！");
               }
           }
           else
           {

               if (serialPortPLC.IsOpen)
               {
                   try
                   {
                       serialPortPLC.Close();
                       comPLC.Enabled = true;
                       baudRatePLC.Enabled = true;

                       refreshCOMPLC.Enabled = true;
                       btnOpenComPLC.Text = "打开串口";
                   }
                   catch
                   {
                       showHintMsg("关闭PLC端口失败！");
                   }
               }
               else
               {
                   comPLC.Enabled = true;
                   baudRatePLC.Enabled = true;

                   refreshCOMPLC.Enabled = true;
                   btnOpenComPLC.Text = "打开串口";
                   serialPortPLC.Dispose();
                   serialPortPLC = null;
                   GC.Collect();
               }
           }
       }

       private void serialPortPLC_DataReceived(object sender, SerialDataReceivedEventArgs e)//待改
       {
           try
           {
               int address = 0;
               address = Convert.ToInt32(mLocalAddressCode);
               

               int setlength = 8;                                   //每包数据的长度，数据和CRC16都是低字节在前
               int receiveLength = serialPortPLC.BytesToRead;
               if (receiveLength == 0) return;
               byte[] receiveData = new byte[receiveLength];
               serialPortPLC.Read(receiveData, 0, receiveData.Length);
               mReceivePLCBufferPool.AddRange(receiveData);
               byte[] OneFrameData = new byte[setlength];
               byte[] waiteforCorrect = new byte[setlength - 2];
               mReceivePLCBufferPool.CopyTo(0, OneFrameData, 0, setlength);
               mReceivePLCBufferPool.CopyTo(0, waiteforCorrect, 0, setlength-2);
               receiveData = null; GC.Collect();
               byte[] crcreturn = CRC16(waiteforCorrect);

               while (true)
               {

                   if ((mReceivePLCBufferPool.Count == setlength) && (mReceivePLCBufferPool[0] == (byte)address % 256) && crcreturn[0] == OneFrameData[setlength - 2] && crcreturn[1] == OneFrameData[setlength - 1])//校验地址和CRC
                   {

                       if (mReceivePLCBufferPool[1] == 0x03 &&  mReceivePLCBufferPool[2]==0x00  && mReceivePLCBufferPool[4] == 0x00 && mReceivePLCBufferPool[5] == 0x02)
                       {
                           SendPLCData();
                           mReceivePLCBufferPool.RemoveRange(0, setlength);
                       }
                       else if(mReceivePLCBufferPool[1] == 0x03 &&  mReceivePLCBufferPool[2]==0x00  && mReceivePLCBufferPool[3] == 0x00 && mReceivePLCBufferPool[4] == 0x00&& mReceivePLCBufferPool[5] == 0x0A)//获取所有参数
                       {
                           SendPlcAllData();
                           mReceivePLCBufferPool.RemoveRange(0, setlength);
                       }
                      else
                       {
                           mReceivePLCBufferPool.RemoveRange(0, setlength);
                           return;
                       }
                   }
                   else
                   {
                       mReceivePLCBufferPool.RemoveRange(0, mReceivePLCBufferPool.Count);
                       return;
                   }
               }
              
           }
           catch (Exception ex)
           {
               //WriteSensorLog("PLC外部查询接收出现错误:" + ex.ToString());
               //this.Invoke(new SetControlValue(showHintMsg), "PLC外部查询接收出现错误！");
           }
       }

       private void SendPlcAllData()
       {
           int length = 25;//PLC回传数据长度

           int address = 0;
           address = Convert.ToInt32(mLocalAddressCode);

           byte[] waiteforCorrect = new byte[length-2];
           byte[] cmdbyte = new byte[length];


           
            float floatValue_COD = _24ValueCOD;
            byte[] bytes_COD = BitConverter.GetBytes(floatValue_COD);

            float floatValue_NH4_N = _24ValueNH4_N;
            byte[] bytes_NH4_N = BitConverter.GetBytes(floatValue_NH4_N);

            float floatValue_SC = _24ValueSC;
            byte[] bytes_SC = BitConverter.GetBytes(floatValue_SC);

            float floatValue_Conductivity = _24ValueConductivity;
            byte[] bytes_Conductivity = BitConverter.GetBytes(floatValue_Conductivity);

            float floatValue_NH4_N_1 = _24ValueNH4_N_1;
            byte[] bytes_NH4_N_1 = BitConverter.GetBytes(floatValue_NH4_N_1);


                cmdbyte[0] = (byte)(address%256);      //柜机地址（1个字节）
                cmdbyte[1] = 0x03;                     
                cmdbyte[2] = 0x14;                     //有效数据为20个字节

           //COD
                cmdbyte[3] = bytes_COD[1];
                cmdbyte[4] = bytes_COD[0];
                cmdbyte[5] = bytes_COD[3];
                cmdbyte[6] = bytes_COD[2];
          

           //氨氮
                cmdbyte[7] = bytes_NH4_N[1];
                cmdbyte[8] = bytes_NH4_N[0];
                cmdbyte[9] = bytes_NH4_N[3];
                cmdbyte[10] = bytes_NH4_N[2];

           

           //悬浮物SC
                cmdbyte[11] = bytes_SC[1];
                cmdbyte[12] = bytes_SC[0];
                cmdbyte[13] = bytes_SC[3];
                cmdbyte[14] = bytes_SC[2];


            //电导率
                cmdbyte[15] = bytes_Conductivity[1];
                cmdbyte[16] = bytes_Conductivity[0];
                cmdbyte[17] = bytes_Conductivity[3];
                cmdbyte[18] = bytes_Conductivity[2];

           
           //氨氮-1
                cmdbyte[19] = bytes_NH4_N_1[1];
                cmdbyte[20] = bytes_NH4_N_1[0];
                cmdbyte[21] = bytes_NH4_N_1[3];
                cmdbyte[22] = bytes_NH4_N_1[2];
                Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                byte[] crcreturn= CRC16(waiteforCorrect);
                cmdbyte[length - 2] = crcreturn[0];
                cmdbyte[length - 1] = crcreturn[1];


                try
                {
                    if (serialPortPLC.IsOpen)  //如果串口开启
                    {
                        serialPortPLC.Write(cmdbyte, 0, cmdbyte.Length);
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    WriteSensorLog("PLC数据查询发送出现错误:" + ex.ToString());
                }
                waiteforCorrect = null;
                cmdbyte = null;
                GC.Collect();


       }

       private void AutoOpenPLCCOM_CheckedChanged(object sender, EventArgs e)
       {
           IniWriteValue("PLCSETTING", "AutoOpenPLCCOM", AutoOpenPLCCOM.Checked.ToString());
       }
       private void SendPLCData()
       {
           int length = 9;//PLC回传数据长度
           //string cmd = "01 10 00 0C 00 02 04";

           int address = 0;
           address = Convert.ToInt32(mLocalAddressCode);

           byte[] waiteforCorrect = new byte[length-2];
           byte[] cmdbyte = new byte[length];


           switch (mReceivePLCBufferPool[3])
           {
           
           case 0x00: //COD
                float floatValue_COD = _24ValueCOD;
                byte[] bytes_COD = BitConverter.GetBytes(floatValue_COD);

                cmdbyte[0] = (byte)(address%256);      //柜机地址（1个字节）
                cmdbyte[1] = 0x03;                     
                cmdbyte[2] = 0x04;                     //有效数据为4个字节
                cmdbyte[3] = bytes_COD[1];
                cmdbyte[4] = bytes_COD[0];
                cmdbyte[5] = bytes_COD[3];
                cmdbyte[6] = bytes_COD[2];
                   Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length-2);
                   byte[] crcreturn_COD = CRC16(waiteforCorrect);
                   cmdbyte[length - 2] = crcreturn_COD[0];
                   cmdbyte[length - 1] = crcreturn_COD[1];
                   break;

           case 0x02: //氨氮
                   float floatValue_NH4_N = _24ValueNH4_N;
                   byte[] bytes_NH4_N = BitConverter.GetBytes(floatValue_NH4_N);

                   cmdbyte[0] = (byte)(address % 256);      //柜机地址（1个字节）
                   cmdbyte[1] = 0x03;
                   cmdbyte[2] = 0x04;                     //有效数据为4个字节
                   cmdbyte[3] = bytes_NH4_N[1];
                   cmdbyte[4] = bytes_NH4_N[0];
                   cmdbyte[5] = bytes_NH4_N[3];
                   cmdbyte[6] = bytes_NH4_N[2];
                   Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                   byte[] crcreturn_NH4_N = CRC16(waiteforCorrect);
                   cmdbyte[length - 2] = crcreturn_NH4_N[0];
                   cmdbyte[length - 1] = crcreturn_NH4_N[1];
                   break;

           case 0x04: //悬浮物SC
                   float floatValue_SC = _24ValueSC;
                   byte[] bytes_SC= BitConverter.GetBytes(floatValue_SC);

                   cmdbyte[0] = (byte)(address % 256);      //柜机地址（1个字节）
                   cmdbyte[1] = 0x03;
                   cmdbyte[2] = 0x04;                     //有效数据为4个字节
                   cmdbyte[3] = bytes_SC[1];
                   cmdbyte[4] = bytes_SC[0];
                   cmdbyte[5] = bytes_SC[3];
                   cmdbyte[6] = bytes_SC[2];
                   Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                   byte[] crcreturn_SC = CRC16(waiteforCorrect);
                   cmdbyte[length - 2] = crcreturn_SC[0];
                   cmdbyte[length - 1] = crcreturn_SC[1];
                   break;

           case 0x06: //电导率
                   float floatValue_Conductivity = _24ValueConductivity;
                   byte[] bytes_Conductivity = BitConverter.GetBytes(floatValue_Conductivity);

                   cmdbyte[0] = (byte)(address % 256);      //柜机地址（1个字节）
                   cmdbyte[1] = 0x03;
                   cmdbyte[2] = 0x04;                     //有效数据为4个字节
                   cmdbyte[3] = bytes_Conductivity[1];
                   cmdbyte[4] = bytes_Conductivity[0];
                   cmdbyte[5] = bytes_Conductivity[3];
                   cmdbyte[6] = bytes_Conductivity[2];
                   Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                   byte[] crcreturn_Conductivity = CRC16(waiteforCorrect);
                   cmdbyte[length - 2] = crcreturn_Conductivity[0];
                   cmdbyte[length - 1] = crcreturn_Conductivity[1];
                   break;
           case 0x08: //氨氮-1
                   float floatValue_NH4_N_1 = _24ValueNH4_N_1;
                   byte[] bytes_NH4_N_1 = BitConverter.GetBytes(floatValue_NH4_N_1);

                   cmdbyte[0] = (byte)(address % 256);      //柜机地址（1个字节）
                   cmdbyte[1] = 0x03;
                   cmdbyte[2] = 0x04;                     //有效数据为4个字节
                   cmdbyte[3] = bytes_NH4_N_1[1];
                   cmdbyte[4] = bytes_NH4_N_1[0];
                   cmdbyte[5] = bytes_NH4_N_1[3];
                   cmdbyte[6] = bytes_NH4_N_1[2];
                   Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                   byte[] crcreturn_NH4_N_1 = CRC16(waiteforCorrect);
                   cmdbyte[length - 2] = crcreturn_NH4_N_1[0];
                   cmdbyte[length - 1] = crcreturn_NH4_N_1[1];
                   break;
           }
   

           try
           {
               if (serialPortPLC.IsOpen)  //如果串口开启
               {
                   serialPortPLC.Write(cmdbyte, 0, cmdbyte.Length);                  
               }
               else
               {
                   //toolStripStatusLabel2.Text = "PH与余氯串口未打开";
                   //label_PH_value.ForeColor = Color.Red;
                   // openPHandCl();
               }
           }
           catch (Exception ex)
           {
               WriteSensorLog("PLC数据查询发送出现错误:" + ex.ToString());
           }
           waiteforCorrect = null;
           cmdbyte = null;
           GC.Collect();
       }



        private void InitialTimer3ShowSpecifiedSpeed(int interval)
        {            
            //设置定时间隔(毫秒为单位)            
            timer3ShowSpecifiedSpeed.Interval = interval;
            //设置执行一次（false）还是一直执行(true)
            timer3ShowSpecifiedSpeed.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer3ShowSpecifiedSpeed.Enabled = true;            
            timer3ShowSpecifiedSpeed.Stop();
        }


        public static double GetHardDiskSpace()
        {
            double totalSize = 0;
            string str_HardDiskName =  "C:\\";
            str_HardDiskName = str_HardDiskName.ToUpper();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = (double)drive.TotalFreeSpace / (double)(1024 * 1024 * 1024);
                }
            }
            return totalSize;
        }

        private void cloudUpload_Click(object sender, EventArgs e)
        {
            //记录参数到配置文件
            IniWriteValue("CLOUDSETTING", " AutoSendData2Cloud", AutoSendData2Cloud.Checked.ToString());
            IniWriteValue("CLOUDSETTING", " CloudServerIP", cloudServerIP.Text);
            IniWriteValue("CLOUDSETTING", " CloudServerPort", cloudServerPort.Text);
            IniWriteValue("CLOUDSETTING", " SerialNumber", serialNumber.Text);

            bAutoSendData2Cloud = AutoSendData2Cloud.Checked;
            mCloudServerIPAddress = cloudServerIP.Text;
            mCloudServerPortNum = cloudServerPort.Text;
            mSerialNumber = serialNumber.Text;


            if (cloudUpload.Text == "开始上传")
            {
                if (string.IsNullOrEmpty(mSerialNumber))
                {
                    MessageBoxOut("序列号不能为空!");
                    return;
                }
                if (NetworkIsConnected())//如果网络连通
                {
                    connect();
                    if (IsConnectedFlag)//已经成功连接
                    {
                        byte[] serialbuffer = System.Text.Encoding.UTF8.GetBytes(mSerialNumber);//序列号字符串转化为字节数组
                        try
                        {
                            socketSend.Send(serialbuffer);
                            timer4SendData2Cloud.Start();   //开启上传云端
                            timer5Heartbeat.Start();        //开启心跳包
                        }
                        catch (Exception ex)
                        {
                            WriteSocketLog("发送序列号失败！" + ex.ToString());
                        }
                    }
                    else
                    {
                        socketSend.Close();
                        socketSend = null;
                        threadreconnect();
                    }

                }
                else//如果网络不通
                {
                    Thread th = new Thread(WaitForConnect);
                    th.Start();
                }
                cloudUpload.Text = "停止上传";
            }
            else
            {
                timer4SendData2Cloud.Stop();    //停止上传云
                timer5Heartbeat.Stop();         //停止心跳包
                cloudUpload.Text = "开始上传";
            }        
        }
        private void WaitForConnect()
        {
            while (true)
            {
                if (NetworkIsConnected())
                {
                    connect();
                    if (IsConnectedFlag)//已经成功连接
                    {
                        byte[] serialbuffer = System.Text.Encoding.UTF8.GetBytes(mSerialNumber);//序列号字符串转化为字节数组
                        try
                        {
                            socketSend.Send(serialbuffer);
                            timer4SendData2Cloud.Start();   //开启上传云端
                            timer5Heartbeat.Start();        //开启心跳包
                        }
                        catch (Exception ex)
                        {
                            WriteSocketLog("发送序列号失败！" + ex.ToString());
                        }
                    }
                    else
                    {
                        socketSend.Close();
                        socketSend = null;
                        threadreconnect();
                    }
                    break;
                }
                Thread.Sleep(1000);
            }

        }
        private void TimerUp4SendData2Cloud(object sender, System.Timers.ElapsedEventArgs e)
        {
            Upload2CloadFunction(
                 mCloudServerIPAddress,
                 mCloudServerPortNum,
                 mLocalAddressCode,

                 _24ValueCOD,     
                 _24ValueNH4_N, 
                 _24ValueSC,
                 _24ValueConductivity,
                 _24ValueNH4_N_1
                  );
          
        }


        private void Upload2CloadFunction(string serverIP,string serverPort,string station,float x1, float x2, float x3, float x4,float x5)
        {
            //浊度+ph+余氯+电导率+温度  待改
            string data = "#" + x1.ToString() + "," + x2.ToString() + "," + x3.ToString() + "," + x4.ToString() + "," + x5.ToString() + "#";//图书馆大柜机
//       string data = "#" + x1.ToString() + "," + x2.ToString() + "," + x3.ToString() + "," + x4.ToString() + "," + x5.ToString()+"," + x8.ToString()+"," + x9.ToString()+"," + x10.ToString() + "#";//鄱阳湖
           byte[] databuffer = System.Text.Encoding.UTF8.GetBytes(data);//序列号字符串转化为字节数组
           try
           {
               socketSend.Send(databuffer);
           }
           catch (Exception ex)
           {
               WriteSocketLog("socketSend.Send失败！" + ex.ToString());
           }

        }

        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
           // string base64 = "944R9YQVO4LBMV1H";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
           // req.Headers.Add("Authorization", "Basic"+base64);
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
                //if (i == 5)
                //    break;
                //i++;
            }
            
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                result = "返回信息错误："+ex.ToString();
            }
            return result;
        }
        private void PostData(string url, string responsStr, string deviceID)
        {
            string result = "";

            string postData = "{\"responsStr\":\"";
            postData += responsStr;
            postData += "\",";
            postData += "\"deviceID\":\"";
            postData += deviceID;
            postData += "\"}";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "POST";

            //req.Timeout = 800;//设置请求超时时间，单位为毫秒

            req.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(postData);

            req.ContentLength = data.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);

                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            Console.WriteLine(result);
        }

        private void AutoSendData2Cloud_CheckedChanged(object sender, EventArgs e)
        {
            IniWriteValue("CLOUDSETTING", " AutoSendData2Cloud", AutoSendData2Cloud.Checked.ToString());
            IniWriteValue("CLOUDSETTING", " CloudServerIP", cloudServerIP.Text);
            IniWriteValue("CLOUDSETTING", " CloudServerPort", cloudServerPort.Text);
            bAutoSendData2Cloud = AutoSendData2Cloud.Checked;
            mCloudServerIPAddress = cloudServerIP.Text;
            mCloudServerPortNum = cloudServerPort.Text;
        }
            
        private void communicationProtocal1thGeneration_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void communicationProtocal2thGeneration_CheckedChanged(object sender, EventArgs e)
        {

        }

      
        private void correctCMDSendIntOrFloat(byte id, byte address0, byte address1,byte functioncode, string textvalue,bool bIntorFloat)
        {
            if (bIntorFloat)
            {
                int length = 8;
                byte[] waiteforCorrect = new byte[length - 2];
                byte[] cmdbyte = new byte[length];
                cmdbyte[0] = id;
                cmdbyte[1] = functioncode;
                cmdbyte[2] = address0;
                cmdbyte[3] = address1;// 0x0C;  
                Int32 intValue = Convert.ToInt32(textvalue);
                cmdbyte[4] =(byte)(intValue/256);
                cmdbyte[5] = (byte)(intValue%256);

                Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                byte[] crcreturn = CRC16(waiteforCorrect);
                cmdbyte[6] = crcreturn[0];
                cmdbyte[7] = crcreturn[1];
                sendSerialPortBYTE(cmdbyte);
            }
            else
            {
                int length = 13;
                byte[] waiteforCorrect = new byte[length - 2];
                byte[] cmdbyte = new byte[length];
                cmdbyte[0] = id;
                cmdbyte[1] = functioncode;
                cmdbyte[2] = address0;
                cmdbyte[3] = address1;// 0x0C;
                cmdbyte[4] = 0x00;
                cmdbyte[5] = 0x02;      //浮点数需要修改2个寄存器
                cmdbyte[6] = 0x04;      //浮点数需要修改2个寄存器
             
                float floatValue = Convert.ToSingle(textvalue);
                byte[] bytes = BitConverter.GetBytes(floatValue);
                cmdbyte[7] = bytes[1];
                cmdbyte[8] = bytes[0];
                cmdbyte[9] = bytes[3];
                cmdbyte[10] = bytes[2];
                
                Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                byte[] crcreturn = CRC16(waiteforCorrect);
                cmdbyte[11] = crcreturn[0];
                cmdbyte[12] = crcreturn[1];
                sendSerialPortBYTE(cmdbyte);
            }
            
        }
        
        

        public void sendSerialPortBYTE(byte[] byteBuffer)
        {
            try
            {
                if (serialPortSensor.IsOpen)  //如果串口开启
                { 
                    serialPortSensor.Write(byteBuffer, 0, byteBuffer.Length);                    
                }
                else
                {
                    MessageBoxOut("传感器串口没被打开！请检查！");
                }
            }
            catch (Exception ex)
            {
                WriteSensorLog("传感器数据发送Byte数据出现错误:" + ex.ToString());

            }

        }
        public void sendSerialPortCMDBYTE(string cmd)
        {
            try
            {
                if (serialPortSensor.IsOpen)  //如果串口开启
                {                  
                    string strTemp = cmd;
                    string sendBuf = strTemp;
                    sendBuf = sendBuf.Trim();
                    sendBuf = sendBuf.Replace(',', ' ');//去掉英文逗号
                    sendBuf = sendBuf.Replace('，',' ');//去掉中文逗号
                    sendBuf = sendBuf.Replace("0x", "");//去掉0x
                    sendBuf.Replace("0X", "");//去掉0X
                    string[] strArray = sendBuf.Split(' ');
                    int byteBufferLength = strArray.Length+2;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (strArray[i] == "")
                        {
                            byteBufferLength--;
                        }
                    }
                    byte[] byteBuffer = new byte[byteBufferLength];
                    byte[] waiteforCorrect = new byte[byteBufferLength - 2];
                    int j = 0;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        byte[] byteOfStr = Encoding.Default.GetBytes(strArray[i]);//
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
                        waiteforCorrect[i] = byteBuffer[j];
                        j++;
                    }
                    byte[] crcreturn = CRC16(waiteforCorrect);
                    byteBuffer[byteBufferLength - 2] = crcreturn[0];
                    byteBuffer[byteBufferLength - 1] = crcreturn[1];
                    serialPortSensor.Write(byteBuffer, 0, byteBuffer.Length);
                    byteBuffer = null;
                    waiteforCorrect = null;
                    GC.Collect();
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                WriteSensorLog("传感器数据发送出现错误:" + ex.ToString());

            }

        }

        private void correctCMDSendResidualChlorine(byte id, byte address0,  byte address1,byte functioncode, float z0,float zk)
        {            
                int length = 21;
                byte[] waiteforCorrect = new byte[length - 2];
                byte[] cmdbyte = new byte[length];
                cmdbyte[0] = id;
                cmdbyte[1] = functioncode;
                cmdbyte[2] = address0;
                cmdbyte[3] = address1;// 0x0C;
                cmdbyte[4] = 0x00;
                cmdbyte[5] = 0x06;      //浮点数需要修改6个寄存器
                cmdbyte[6] = 0x0C;      //12个字节                
                byte[] bytes = BitConverter.GetBytes(z0);
                cmdbyte[7] = bytes[1];
                cmdbyte[8] = bytes[0];
                cmdbyte[9] = bytes[3];
                cmdbyte[10] = bytes[2];
                byte[] bytes2 = BitConverter.GetBytes(zk);
                cmdbyte[11] = bytes2[1];
                cmdbyte[12] = bytes2[0];
                cmdbyte[13] = bytes2[3];
                cmdbyte[14] = bytes2[2];
                byte[] bytes3 =new byte[4];
                //UInt32 data = 0x00000000;
               // ConvertIntToByteArray(data, ref bytes3);
                cmdbyte[15] = 0x00;//bytes3[1];
                cmdbyte[16] = 0x00;//bytes3[0];
                cmdbyte[17] = 0x00;//bytes3[3];
                cmdbyte[18] = 0x00;//bytes3[2];
                Array.Copy(cmdbyte, 0, waiteforCorrect, 0, length - 2);
                byte[] crcreturn = CRC16(waiteforCorrect);
                cmdbyte[19] = crcreturn[0];
                cmdbyte[20] = crcreturn[1];
                sendSerialPortBYTE(cmdbyte);
           
        }
        /// <summary>
        /// 把int32类型的数据转存到4个字节的byte数组中
        /// </summary>
        /// <param name="m">int32类型的数据</param>
        /// <param name="arry">4个字节大小的byte数组</param>
        /// <returns></returns>
        static bool ConvertIntToByteArray(UInt32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;
            arry[0] = (byte)(m & 0xFF);
            arry[1] = (byte)((m & 0xFF00) >> 8);
            arry[2] = (byte)((m & 0xFF0000) >> 16);
            arry[3] = (byte)((m >> 24) & 0xFF);
            return true;
        }




       





        //COD零点校准------------------------------------------------------
        private void correctionZero_COD_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                Thread th = new Thread(sendcmdCOD_Zero);
                th.Start();
                showHintMsg("正在进行零点校正，请稍后！");
            }
        }

        private void sendcmdCOD_Zero()
        {
            string cmd1 = "01 06 00 5A 00 3E";//发送62
            string cmd2 = "01 06 00 5A 00 19";//发送25
            string cmd3 = "01 06 00 5A 00 12";//发送18
            string cmd4 = "01 06 00 5A 00 3D";//发送61
            string cmd5 = "01 06 00 5A 00 55";//发送85
            string cmd6 = "01 06 00 5A 00 3D";//发送61
            string cmd7 = "01 06 00 5A 00 13";//发送19
            string cmd8 = "01 06 00 5A 00 2A";//发送42
            string cmd9 = "01 06 00 5A 00 34";//发送52

            sendSerialPortCMDBYTE(cmd1);//发送62
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd2);//发送25
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd3);//发送18
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd4);//发送61
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd5);//发送85
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd6);//发送61
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd7);//发送19
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd8);//发送42
            Thread.Sleep(500);
            sendSerialPortCMDBYTE(cmd9);//发送52
            if (msgShowFrm.InvokeRequired)
            {
                msgShowFrm.Invoke(new SetControlValue(showHintMsg), "设置完毕！");
            }
        }
        //COD因子校准------------------------------------------------------
        private void correctionK_COD_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xB8, K_COD_txbox.Text);
                showHintMsg("设置完毕！");   
            }
        }
        //COD偏差校准------------------------------------------------------
        private void correctionBias_COD_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xBA, Bias_COD_txbox.Text);
                showHintMsg("设置完毕！");  
            }
        }



        //NH4N因子校准------------------------------------------------------
        private void correctionK_NH4N_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0x9A, K_NH4N_txbox.Text);
                showHintMsg("设置完毕！");   
            }
        }
        //NH4N偏差校准------------------------------------------------------
        private void correctionBias_NH4N_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0x9C, Bias_NH4N_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }


        //NH4N-1因子校准------------------------------------------------------
        private void correctionK_NH4N_1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0x9A, K_NH4N_1_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }

        private void correctionBias_NH4N_1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0x9C, Bias_NH4N_1_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }


        ////电导率因子校准------------------------------------------------------
        //private void correctionK_Conduct_btn_Click(object sender, EventArgs e)
        //{
        //    correctCMDSend(0x04, 0x00, 0x0E, K_Conduct_txbox.Text);
        //    showHintMsg("设置完毕！");
        //}
        //电导率偏差校准------------------------------------------------------
        private void correctionBias_Conduct_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x04, 0x00, 0x0C, Bias_Conduct_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }
       


        //private void nextCorrect_Click(object sender, EventArgs e)
        //{
        //    content.SelectedIndex = 6;
        //    mTabCurrentIndex = 6;
        //}



        //悬浮物因子校准 无偏差校准
        private void correctionK_SC_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                Thread th = new Thread(correctSC);
                th.Start(); 
            }
        }

        private void correctSC()
        {
            string cmd = "03 06 00 1B 00 01";//先发送1 表明选择因子校准
            sendSerialPortCMDBYTE(cmd);
            Thread.Sleep(100);//间隔100ms
            correctCMDSend(0x03, 0x00, 0x06, K_SC_txbox.Text);
            showHintMsg("设置完毕！");  
        }


        private void correctionBias_pH_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x05, 0xDC, Bias_pH_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }

        private void correctionBias_pH_1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x05, 0xDC, Bias_pH_1_txbox.Text);
                showHintMsg("设置完毕！"); 
            }
        }

        private void display_pH_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                sendSerialPortCMD(2, "02 03 00 02 00 02 65 F8");//氨氮地址02  查询ph  寄存器地址02 
            }
        }

        private void display_pH_1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                sendSerialPortCMD(2, "05 03 00 02 00 02 64 4F");//氨氮地址05  查询ph-1  寄存器地址02 
            }
        }

        private void titleOrganization2_Click(object sender, EventArgs e)
        {

        }

        private void content_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void skinTabPage3_Click(object sender, EventArgs e)
        {

        }

        private void codStandard1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xC1, codStandard1_txb.Text);   //COD标液1
                showHintMsg("设置完毕！");
            }
        }


        private void codStandard2_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xC3, codStandard2_txb.Text);   //COD标液2
                showHintMsg("设置完毕！");
            }
        }



        private void NH4Npoints_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                string cmd = "02 06 08 A0 00 05";   
                if (NH4Npoints_txb.Text=="5")
                {
                    cmd = "02 06 08 A0 00 05";                
                }
                else if (NH4Npoints_txb.Text=="4")
                {
                    cmd = "02 06 08 A0 00 04";
                }
                else if (NH4Npoints_txb.Text=="3")
                {
                    cmd = "02 06 08 A0 00 03";
                }
                else
                {
                    cmd = "02 06 08 A0 00 05"; 
                }
                sendSerialPortCMDBYTE(cmd);
                showHintMsg("设置完毕！");
            }
        }

        private void NH4NStandard1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0xBA, NH4NStandard1_txb.Text);     //NH4N标液1
                showHintMsg("设置完毕！");
            }
        }

        private void NH4NStandard2_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0xC2, NH4NStandard2_txb.Text);     //NH4N标液2
                showHintMsg("设置完毕！");
            }
        }

        private void NH4NStandard3_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0xCA, NH4NStandard3_txb.Text);     //NH4N标液3
                showHintMsg("设置完毕！");
            }
        }

        private void NH4NStandard4_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0xD2, NH4NStandard4_txb.Text);     //NH4N标液4
                showHintMsg("设置完毕！");
            }
        }

        private void NH4NStandard5_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x02, 0x08, 0xDA, NH4NStandard5_txb.Text);     //NH4N标液5
                showHintMsg("设置完毕！");

            }
        }

        private void standardModeSelect_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                string cmd1 = "01 06 00 BC 00 00";//地址188写0   标液模式
                sendSerialPortCMDBYTE(cmd1);
                showHintMsg("当前为标液模式"); 
            }

        }

        private void sampleModeSelect_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                string cmd1 = "01 06 00 BC 00 01";//地址188写1   水样模式
                sendSerialPortCMDBYTE(cmd1);
                showHintMsg("当前为水样模式"); 
            }
        }

        private void nextCorrect_Click(object sender, EventArgs e)
        {
            content.SelectedIndex = 4;
        }

        private void previousCorrect_Click(object sender, EventArgs e)
        {
            content.SelectedIndex = 3;
        }

        private void btnUV254Read_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                if (btnUV254Read.Text=="连续读取")
                {
                    btnUV254Read.Text = "停止读取";
                    QueryUv254 = true;
                    Thread th = new Thread(readUV254);
                    th.Start();
                }
                else
                {
                    btnUV254Read.Text = "连续读取";
                    QueryUv254 = false;
                }
                
            }
        }

        private void readUV254()
        {
            while (QueryUv254)
            {
                string cmd1 = "01 03 00 06 00 02";//读取UV254 
                sendSerialPortCMDBYTE(cmd1);
                Thread.Sleep(5 * 1000);               
            }
        }

        private void btnUV254Write_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xBD, txbUV254Write.Text);  //写入VU254
                showHintMsg("写入成功！");
            }
        }

        private void btnPeriodWrite_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                string cmd1 = "01 06 00 10 02 D0";//720
                if (txbPeriodWrite.Text=="4096")
                {
                     cmd1 = "01 06 00 10 10 00";//4096
                
                }
                else if (txbPeriodWrite.Text=="720")
                {
                     cmd1 = "01 06 00 10 02 D0";//720
                }
                sendSerialPortCMDBYTE(cmd1);
                showHintMsg("写入成功！");
            }
        }

        private void btnUV254Sample2Write_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x01, 0x00, 0xBF, txbUV254Sample2Write.Text);  //写入样二VU254
                showHintMsg("写入成功！");
            }

        }

        private void xhNH4Npoints_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                string cmd = "05 06 08 A0 00 05";
                if (NH4Npoints_txb.Text == "5")
                {
                    cmd = "05 06 08 A0 00 05";
                }
                else if (NH4Npoints_txb.Text == "4")
                {
                    cmd = "05 06 08 A0 00 04";
                }
                else if (NH4Npoints_txb.Text == "3")
                {
                    cmd = "05 06 08 A0 00 03";
                }
                else
                {
                    cmd = "05 06 08 A0 00 05";
                }
                sendSerialPortCMDBYTE(cmd);
                showHintMsg("设置完毕！");
            }
        }

        private void xhNH4NStandard1_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0xBA, NH4NStandard1_txb.Text);     //NH4N标液1
                showHintMsg("设置完毕！");
            }
        }

        private void xhNH4NStandard2_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0xC2, NH4NStandard2_txb.Text);     //NH4N标液2
                showHintMsg("设置完毕！");
            }
        }

        private void xhNH4NStandard3_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0xCA, NH4NStandard3_txb.Text);     //NH4N标液3
                showHintMsg("设置完毕！");
            }
        }

        private void xhNH4NStandard4_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0xD2, NH4NStandard4_txb.Text);     //NH4N标液4
                showHintMsg("设置完毕！");
            }
        }

        private void xhNH4NStandard5_btn_Click(object sender, EventArgs e)
        {
            if (CaptureSensorContinuesBackUp.Text == "停止传感器连续采集")
            {
                showHintMsg("请先关闭采集！");
                return;
            }
            else
            {
                correctCMDSend(0x05, 0x08, 0xDA, NH4NStandard5_txb.Text);     //NH4N标液5
                showHintMsg("设置完毕！");

            }
        }
    }
}
