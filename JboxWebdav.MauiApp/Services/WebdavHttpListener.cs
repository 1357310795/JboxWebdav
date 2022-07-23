using NWebDav.Server;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.HttpListener;
using NWebDav.Server.Logging;
using NWebDav.Server.Stores;
using System.Net;

namespace JboxWebdav.MauiApp.Services
{
    public class WebdavHttpListener
    {
        public static WebDavDispatcher webDavDispatcher;
        public static CancellationTokenSource cancellationTokenSource;
        public static HttpListener httpListener;

        private static ILogger s_log = LoggerFactory.CreateLogger(typeof(WebdavHttpListener));

        private static void DispatchHttpRequestsAsync()
        {
            var requestHandlerFactory = new RequestHandlerFactory();
            webDavDispatcher = new WebDavDispatcher(new JboxStore(), requestHandlerFactory);
            httpListener.BeginGetContext(new AsyncCallback(GetContextCallBack), httpListener);
        }

        private static async void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                if (httpListener.IsListening && !cancellationTokenSource.Token.IsCancellationRequested)
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
                else
                {

                }
            }
            catch (Exception ex)
            {
                s_log.Log(LogLevel.Fatal, ()=>"DispatchRequest失败", ex);
                //Console.WriteLine(ex.Message);
                //throw new ArgumentException(ex.Message);
            }
        }

        public static void Main(string httpPrefix)
        {
            LoggerFactory.Factory = new Log4NetAdapter();

            httpListener = new System.Net.HttpListener();
            httpListener.Prefixes.Add(httpPrefix);
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Start();

            cancellationTokenSource = new CancellationTokenSource();
            DispatchHttpRequestsAsync();

            Console.WriteLine("WebDAV server running. Press 'x' to quit.");
            //while (Console.ReadKey().KeyChar != 'x') ;
            //Console.ReadKey();
            //cancellationTokenSource.Cancel();
        }
    }
}
