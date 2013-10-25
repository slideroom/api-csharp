using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            SlideRoomTest.TestClient.GetTestClient(res, (c) =>
            {
                var actualResult = c.Export.Download(123);

                var expectedResult = new SlideRoom.Resources.DownloadResult()
                {
                    Pending = true,
                    ExportStream = null
                };

                Assert.AreEqual(actualResult.ExportStream, expectedResult.ExportStream);
                Assert.AreEqual(actualResult.Pending, expectedResult.Pending);
            });
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

            SlideRoomTest.TestClient.GetTestClient(res, (c) =>
            {
                var actualResult = c.Export.Download(123);

                Assert.IsFalse(actualResult.Pending);
                Assert.IsNotNull(actualResult.ExportStream);

                var actualReport = String.Empty;
                using (StreamReader reader = new StreamReader(actualResult.ExportStream, Encoding.UTF8))
                {
                    actualReport = reader.ReadToEnd();
                }

                Assert.AreEqual(actualReport, "test,report");
            });
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

            SlideRoomTest.TestClient.GetTestClient(res, (c) =>
            {
                try
                {
                    c.Export.Download(123);
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual(e.Message, "Export no longer available.");
                    Assert.AreEqual(e.StatusCode, System.Net.HttpStatusCode.Gone);
                }
                catch (Exception e)
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            });
        }
    }
}
