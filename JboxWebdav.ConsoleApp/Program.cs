using System.Net;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.HttpListener;
using NWebDav.Server.Logging;
using NWebDav.Server.Stores;
using NWebDav.Sample.HttpListener.LogAdapters;
using System.Diagnostics;
using Jbox.Service;
using NWebDav.Server.Helpers;
using JboxWebdav.ConsoleApp.Services;
using System.Text;

namespace NWebDav.Sample.HttpListener
{
    internal class Program
    {
        public static WebDavDispatcher webDavDispatcher;

        private static async void DispatchHttpRequestsAsync(System.Net.HttpListener httpListener, CancellationToken cancellationToken)
        {
            var requestHandlerFactory = new RequestHandlerFactory();

            webDavDispatcher = new WebDavDispatcher(new JboxStore(), requestHandlerFactory);

            httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
        }

        private static async void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                System.Net.HttpListener httpListener = ar.AsyncState as System.Net.HttpListener;
                if (httpListener.IsListening)
                {
                    HttpListenerContext httpListenerContext = httpListener.EndGetContext(ar);
                    httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);

                    if (httpListenerContext == null)
                        return;

                    IHttpContext httpContext = null;
                    try
                    {
                        httpContext = new HttpContext(httpListenerContext);
                        //httpContext = new HttpBasicContext(httpListenerContext, Jac.checkJac);
                    }
                    catch (Exception ex)
                    {
                        httpContext = new HttpContext(httpListenerContext);
                        httpContext.Response.SetStatus(DavStatusCode.Unauthorized);
                        await httpContext.CloseAsync().ConfigureAwait(false);
                        return;
                    }

                    // Dispatch the request
                    await webDavDispatcher.DispatchRequestAsync(httpContext).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw new ArgumentException(ex.Message);
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to JboxWebdav!");
            var res = TryLogin();
            if (!res)
            {
                Console.WriteLine("Cookie登录失败，请使用账号密码登录");
                while(true)
                {
                    Console.Write("Jaccount账号：");
                    var account = Console.ReadLine();
                    Console.Write("Jaccount密码：");
                    var password = ReadPassword();
                    var res1 = Jac.Login(account, password);
                    if (res1.state == Jac.LoginState.success)
                    {
                        Console.WriteLine("登录成功！");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(res1.message);
                    }
                }
            }
            LoggerFactory.Factory = new ConsoleAdapter();

            var webdavProtocol = "http";
            var webdavIp = "127.0.0.1";
            var webdavPort = "65472";

            using (var httpListener = new System.Net.HttpListener())
            {
                httpListener.Prefixes.Add($"{webdavProtocol}://{webdavIp}:{webdavPort}/");

                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Start();

                var cancellationTokenSource = new CancellationTokenSource();
                DispatchHttpRequestsAsync(httpListener, cancellationTokenSource.Token);

                Console.WriteLine("WebDAV 服务器运行中。按下 x 可以退出，按下 c 进入设置。");
                while (Console.ReadKey().KeyChar != 'x') ;
                Console.ReadKey();
                cancellationTokenSource.Cancel();
            }
        }

        private static bool TryLogin()
        {
            Jac.InitStorage(new WinStorage());
            Jac.ReadInfo();
            if (Jac.dic.Count > 0)
            {
                var ac = Jac.dic.Keys.First();
                if (Jac.TryLastCookie(ac))
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReadPassword()
        {
            //string res = "";
            //while (true)
            //{
            //    ConsoleKeyInfo ck = Console.ReadKey(true);

            //    if (ck.Key != ConsoleKey.Enter)
            //    {
            //        if (ck.Key != ConsoleKey.Backspace)
            //        {
            //            res += ck.KeyChar.ToString();
            //            Console.Write("*");
            //        }
            //        else
            //        {
            //            Console.Write("\b \b");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine();
            //        break;
            //    }
            //}
            //return res;
            char cPassword;     //登陆时要用的密码 
            StringBuilder cPass = new StringBuilder();//关键方法 
            cPassword = Console.ReadKey(true).KeyChar;//输入字符可以让他不显示出来
            while (cPassword != '\r')//回车
            {
                if (cPassword == '\b')//退格
                {
                    if (cPass.Length == 0)
                    {
                        cPassword = (char)Console.ReadKey(true).KeyChar;//输入字符可以让他不显示出来
                    }
                    else
                    {
                        cPass.Remove(cPass.Length - 1, 1);
                        int cur_x = Console.CursorLeft;
                        int cur_y = Console.CursorTop;
                        Console.SetCursorPosition(cur_x - 1, cur_y);//光标定位    根据光标位置自己改动   x,y坐标
                        Console.Write(" ");
                        Console.SetCursorPosition(cur_x - 1, cur_y);//光标定位    根据光标位置自己改动
                        cPassword = (char)Console.ReadKey(true).KeyChar;
                    }
                }
                else
                {
                    cPass.Append(cPassword);
                    Console.Write('*');
                    cPassword = (char)Console.ReadKey(true).KeyChar;
                }
            }
            Console.WriteLine();
            return cPass.ToString();
        }
    }
}
