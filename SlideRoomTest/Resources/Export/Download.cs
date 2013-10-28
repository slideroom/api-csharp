using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace SlideRoomTest.Resources.Export
{
    [TestClass]
    public class ExportDownload
    {
        [TestMethod]
        public void GoodPendingDownload()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.Accepted,
                Body = ""
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Download(123);

                var expectedResult = new SlideRoom.Resources.DownloadResult()
                {
                    Pending = true,
                    ExportStream = null
                };

                Assert.AreEqual(actualResult.ExportStream, expectedResult.ExportStream);
                Assert.AreEqual(actualResult.Pending, expectedResult.Pending);
            }
        }


        [TestMethod]
        public void GoodDownload()
        {
            var res = new MockResponse()
            {
                ContentType = "text/plain",
                Code = System.Net.HttpStatusCode.OK,
                Body = "test,report"
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Download(123);

                Assert.IsFalse(actualResult.Pending);
                Assert.IsNotNull(actualResult.ExportStream);

                var actualReport = String.Empty;
                using (StreamReader reader = new StreamReader(actualResult.ExportStream, Encoding.UTF8))
                {
                    actualReport = reader.ReadToEnd();
                }

                Assert.AreEqual("test,report", actualReport);
            }
        }


        [TestMethod]
        public void BadDownload()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.Gone,
                Body = @"{ ""message"": ""Export no longer available."" }"
            };

            using (var c = new TestClient(res))
            {
                try
                {
                    c.Client.Export.Download(123);
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual("Export no longer available.", e.Message);
                    Assert.AreEqual(System.Net.HttpStatusCode.Gone, e.StatusCode);
                }
                catch
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            }
        }
    }
}
