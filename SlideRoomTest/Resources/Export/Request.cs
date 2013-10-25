using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlideRoomTest.Resources.Export
{
    [TestClass]
    public class ExportRequest
    {
        [TestMethod]
        public void GoodRequest()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            SlideRoomTest.TestClient.GetTestClient(res, (c) =>
            {
                var actualResult = c.Export.Request("test", SlideRoom.Resources.RequestFormat.Csv);

                var expectedResult = new SlideRoom.Resources.RequestResult()
                {
                    Message = "test",
                    Submissions = 456,
                    Token = 123
                };

                Assert.AreEqual(actualResult.Message, expectedResult.Message);
                Assert.AreEqual(actualResult.Submissions, expectedResult.Submissions);
                Assert.AreEqual(actualResult.Token, expectedResult.Token);
            });
        }

        [TestMethod]
        public void BadRequest()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.NotFound,
                Body = @"{ ""message"": ""Invalid Format"" }"
            };

            TestClient.GetTestClient(res, (c) =>
            {
                try
                {
                    c.Export.Request("test", SlideRoom.Resources.RequestFormat.Csv);
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual(e.Message, "Invalid Format");
                    Assert.AreEqual(e.StatusCode, System.Net.HttpStatusCode.NotFound);
                }
                catch (Exception e)
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            });
        }
    }
}
