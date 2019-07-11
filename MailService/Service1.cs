using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cjwdev.WindowsApi;
using System.Runtime.InteropServices;
using System.Configuration;

namespace MailService
{
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Timer timer = null;
        private int m_timeSpanNotify = 0;
        public Service1()
        {
            InitializeComponent();
            string timeSpanNotify = ConfigurationManager.AppSettings["TimeNotify"];

            int timeSpanSec = 0;

            int.TryParse(timeSpanNotify, out timeSpanSec);

            m_timeSpanNotify = timeSpanSec;
        }

        protected override void OnStart(string[] args)
        {

            timer = new System.Threading.Timer(Timer, null, 0, m_timeSpanNotify);
        }

        private void Timer(object o)
        {
            try
            {
                AppStart(@"C:\MailProcessor\MailProcessor.exe");

                Write(DateTime.Now.ToString() + "服务：Timer()定时器成功处理一次");
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "服务：Timer()定时器处理异常:" + ex.Message);
            }
        }



        private void Write(string str)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("C:\\MailService.txt", true))
                {
                    sw.WriteLine(str);
                }
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "Write()写入文本异常:" + ex.Message);
            }

        }

        protected override void OnStop()
        {
            try
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "OnStop()服务结束异常:" + ex.Message);
            }
        }


        public void AppStart(string appPath)
        {
            try
            {

                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    "",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
