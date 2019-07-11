using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailProcessor
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        static void Main(string[] args)
        {
            //开始之前隐藏Console窗体，免得截图的时候显示在图片中
            //Console.Title = "HYStrategy";
            //IntPtr intptr = FindWindow(null, "HYStrategy");
            //if (intptr != IntPtr.Zero)
            //{
            //    ShowWindow(intptr, 0);//隐藏这个窗口
            //}

            //执行截图并邮箱发送
            try
            {
                string map = ClipImage();
                DoSendMail(map);
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "客户端:Write()写入文本异常:" + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private static void Write(string str)
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
                Write(DateTime.Now.ToString() + "客户端:Write()写入文本异常:" + ex.Message);
            }

        }

        /// <summary>
        /// 在此处完成对全屏的截图
        /// </summary>
        private static string ClipImage()
        {
            try
            {
                //1-----------------------------------------
                //屏幕宽
                int iWidth = Screen.PrimaryScreen.Bounds.Width;
                //屏幕高
                int iHeight = Screen.PrimaryScreen.Bounds.Height;
                //按照屏幕宽高创建位图
                Bitmap img = new Bitmap(iWidth, iHeight);
                //从一个继承自Image类的对象中创建Graphics对象
                Graphics gc = Graphics.FromImage(img);
                //抓屏并拷贝到myimage里
                gc.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
                //this.BackgroundImage = img;
                //Clipboard.Clear();
                //Clipboard.SetDataObject(img);
                //保存位图
                string name = Guid.NewGuid().ToString();
                img.Save(@"C:\" + name + ".jpg");

                return @"C:\" + name + ".jpg";
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "客户端:ClipImage()截图异常:" + ex.Message);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 处理QQMail
        /// </summary>
        private static void DoSendMail(string imageMap)
        {
            try
            {
                //实例化一个发送邮件类。
                MailMessage mailMessage = new MailMessage();
                //发件人邮箱地址，方法重载不同，可以根据需求自行选择。
                mailMessage.From = new MailAddress("578931169@qq.com", "策略服务器");
                //收件人邮箱地址。
                mailMessage.To.Add(new MailAddress("578931169@qq.com"));

                //邮件标题。
                mailMessage.Subject = DateTime.Now.ToString() + ":策略服务器有新的消息";
                //邮件内容。
                //mailMessage.Body = strbaser64;//转化成HTML把图片然后发送
                //发送截图
                mailMessage.IsBodyHtml = false;
                mailMessage.Attachments.Add(new Attachment(imageMap));


                //实例化一个SmtpClient类。
                SmtpClient client = new SmtpClient();
                //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
                client.Host = "smtp.qq.com";
                //使用安全加密连接。
                client.EnableSsl = true;

                //不和请求一块发送。
                client.UseDefaultCredentials = false;
                //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                client.Credentials = new NetworkCredential("578931169@qq.com", "lhfdcuftsjjwbbhd");
                //发送
                client.Send(mailMessage);
                //Context.Response.Write("发送成功");
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + "客户端:DoSendMail()发送异常:" + ex.Message);
                throw new Exception(ex.Message);
            }
        }


    }
}
