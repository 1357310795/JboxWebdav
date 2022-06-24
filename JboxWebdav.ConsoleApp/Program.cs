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

namespace NWebDav.Sample.HttpListener
{
    internal class Program
    {
        private static async void DispatchHttpRequestsAsync(System.Net.HttpListener httpListener, CancellationToken cancellationToken)
        {
            // Create a request handler factory that uses basic authentication
            var requestHandlerFactory = new RequestHandlerFactory();

            // Create WebDAV dispatcher
            var homeFolder = @"D:\Download";
            var webDavDispatcher = new WebDavDispatcher(new DiskStore(homeFolder), requestHandlerFactory);

            // Determine the WebDAV username/password for authorization
            // (only when basic authentication is enabled)
            var webdavUsername =  "test";
            var webdavPassword =  "test";

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
                        httpContext = new HttpBasicContext(httpListenerContext, checkIdentity: i => i.Name == webdavUsername && i.Password == webdavPassword);
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

        private static void Main(string[] args)
        {
            // Use the Log4NET adapter for logging
            LoggerFactory.Factory = new ConsoleAdapter();

            // Obtain the HTTP binding settings
            var webdavProtocol =  "http";
            var webdavIp =  "127.0.0.1";
            var webdavPort =  "11111";

            using (var httpListener = new System.Net.HttpListener())
            {
                // Add the prefix
                httpListener.Prefixes.Add($"{webdavProtocol}://{webdavIp}:{webdavPort}/");

                // Use basic authentication if requested
                var webdavUseAuthentication = false;
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
                DispatchHttpRequestsAsync(httpListener, cancellationTokenSource.Token);

                // Wait until somebody presses return
                Console.WriteLine("WebDAV server running. Press 'x' to quit.");
                while (Console.ReadKey().KeyChar != 'x') ;
                Console.ReadKey();
                cancellationTokenSource.Cancel();
            }
        }
    }
}
