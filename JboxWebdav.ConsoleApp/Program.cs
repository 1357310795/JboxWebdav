using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Xml;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.HttpListener;
using NWebDav.Server.Logging;
using NWebDav.Server.Stores;

using NWebDav.Sample.HttpListener.LogAdapters;
using System.Diagnostics;
using Jbox.Service;
using NWebDav.Server.Helpers;

namespace NWebDav.Sample.HttpListener
{
    internal class Program
    {
        public static WebDavDispatcher webDavDispatcher;

        private static async void DispatchHttpRequestsAsync(System.Net.HttpListener httpListener, CancellationToken cancellationToken)
        {
            // Create a request handler factory that uses basic authentication
            var requestHandlerFactory = new RequestHandlerFactory();

            // Create WebDAV dispatcher
            var homeFolder = @"D:\BaiduNetdiskDownload";
            var webDavDispatcher = new WebDavDispatcher(new JboxStore(), requestHandlerFactory);

            HttpListenerContext httpListenerContext;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    httpListenerContext = await httpListener.GetContextAsync().ConfigureAwait(false);
                    if (httpListenerContext == null)
                        break;
                    // Determine the proper HTTP context
                    IHttpContext httpContext;
                    if (httpListenerContext.Request.IsAuthenticated)
                        httpContext = new HttpBasicContext(httpListenerContext, Jac.checkJac);
                    else
                        httpContext = new HttpContext(httpListenerContext);

                    // Dispatch the request
                    await webDavDispatcher.DispatchRequestAsync(httpContext).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static async void DispatchHttpRequestsAsync1(System.Net.HttpListener httpListener, CancellationToken cancellationToken)
        {
            // Create a request handler factory that uses basic authentication
            var requestHandlerFactory = new RequestHandlerFactory();

            // Create WebDAV dispatcher
            var homeFolder = @"D:\BaiduNetdiskDownload";
            //webDavDispatcher = new WebDavDispatcher(new DiskStore(homeFolder), requestHandlerFactory);
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
                        //httpContext = new HttpContext(httpListenerContext);
                        httpContext = new HttpBasicContext(httpListenerContext, Jac.checkJac);
                    }
                    catch(Exception ex)
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
                throw new ArgumentException(ex.Message);
            }
        }

        private static void Main(string[] args)
        {
            // Use the Log4NET adapter for logging
            LoggerFactory.Factory = new ConsoleAdapter();

            Jac.ReadInfo();

            // Obtain the HTTP binding settings
            var webdavProtocol =  "http";
            //var webdavIp = "192.168.1.105";
            //var webdavPort = "80";
            var webdavIp = "127.0.0.1";
            var webdavPort = "11111";

            using (var httpListener = new System.Net.HttpListener())
            {
                // Add the prefix
                httpListener.Prefixes.Add($"{webdavProtocol}://{webdavIp}:{webdavPort}/");

                // Use basic authentication if requested
                var webdavUseAuthentication = true;
                if (webdavUseAuthentication)
                {
                    // Check if HTTPS is enabled
                    if (webdavProtocol != "https")
                        Console.WriteLine("Most WebDAV clients cannot use authentication on a non-HTTPS connection");

                    // Set the authentication scheme and realm
                    httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
                    httpListener.Realm = "WebDAV server";
                }
                else
                {
                    // Allow anonymous access
                    httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                }

                // Start the HTTP listener
                httpListener.Start();

                // Start dispatching requests
                var cancellationTokenSource = new CancellationTokenSource();
                DispatchHttpRequestsAsync1(httpListener, cancellationTokenSource.Token);

                // Wait until somebody presses return
                Console.WriteLine("WebDAV server running. Press 'x' to quit.");
                while (Console.ReadKey().KeyChar != 'x') ;
                Console.ReadKey();
                cancellationTokenSource.Cancel();
            }
        }

        
    }
}
