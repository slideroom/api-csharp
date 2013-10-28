using System;
using System.Net;
using System.Text;
using System.Threading;


namespace SlideRoomTest
{
    class MockServer
    {
        private static int _port = 20240;

        public static void Run(MockResponse r, Action<int> onStarted)
        {
            var listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:" + _port + "/");

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
                                byte[] buf = Encoding.UTF8.GetBytes(r.Body);
                                ctx.Response.Headers.Add("Content-Type", r.ContentType);
                                ctx.Response.StatusCode = (int)r.Code;
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


            try
            {
                onStarted(_port);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                listener.Stop();
                listener.Close();
                _port += 1;
            }
        }
    }
}
