using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace WaterMonitoring
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕获的异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += Application_ThreadException;
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
                var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                                        ex.GetType().Name, ex.Message, ex.StackTrace);
                WriteLog(str, ex);//日志写入

            }
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str;
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            var error = e.Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                                  error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            WriteLog(str, error);
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            var str = error != null ? string.Format(strDateInfo + "Application UnhandledException:{0};\r\n堆栈信息:{1}", error.Message, error.StackTrace) :
                string.Format("Application UnhandledError:{0}", e);
            WriteLog(str, error);
        }
        static void WriteLog(string str, Exception error)
        {
            //string str1 = @"C:\ErrLog";
            if (!Directory.Exists("ErrLog"))
                if (!Directory.Exists("ErrLog"))
            {
                Directory.CreateDirectory("ErrLog");
            }
            using (var sw = new StreamWriter(@"ErrLog\ErrLog.txt", true))
            {
                sw.WriteLine(str);
                //sw.WriteLine("异常信息：" + error.Message);
                //sw.WriteLine("调用堆栈：\n" + error.StackTrace.Trim());
                sw.WriteLine("异常对象：" + error.Source);
                sw.WriteLine("触发方法：" + error.TargetSite);
                sw.WriteLine("---------------------------------------------------------");
                sw.Close();
            }
        }

    }
}
