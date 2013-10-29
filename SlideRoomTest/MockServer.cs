using System;
using System.Net;
using System.Text;
using System.Threading;


namespace SlideRoomTest
{
    class MockServer : IDisposable
    {
        private static int _port = 20240;

        private MockResponse mockResponse { get; set; }
        private HttpListener listener { get; set; }
        private Action<HttpListenerRequest> OnRequest { get; set; }
        public int Port { get; set; }

        public MockServer(MockResponse res, Action<HttpListenerRequest> onRequest = null)
        {
            mockResponse = res;
            Port = _port;
            OnRequest = onRequest;
            _port += 1;

            Run();
        }

        private void Run()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:" + Port + "/");

            // TODO: need a way to catch an already listening exception, and just try the next port
            listener.Start();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                if (OnRequest != null)
                                {
                                    OnRequest(ctx.Request);
                                }

                                byte[] buf = Encoding.UTF8.GetBytes(mockResponse.Body);
                                ctx.Response.Headers.Add("Content-Type", mockResponse.ContentType);
                                ctx.Response.StatusCode = (int)mockResponse.Code;
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { }
                            finally
                            {
                                ctx.Response.OutputStream.Close();
                            }

                        }, listener.GetContext());
                    }
                }
                catch { }
            });
        }

        public void Dispose()
        {
            listener.Stop();
            listener.Close();
        }
    }
}
