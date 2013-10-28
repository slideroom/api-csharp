using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.Resources.RequestFormat.Csv);

                var expectedResult = new SlideRoom.Resources.RequestResult()
                {
                    Message = "test",
                    Submissions = 456,
                    Token = 123
                };

                Assert.AreEqual(expectedResult.Message, actualResult.Message);
                Assert.AreEqual(expectedResult.Submissions, actualResult.Submissions);
                Assert.AreEqual(expectedResult.Token, actualResult.Token);
            }
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

            using (var c = new TestClient(res))
            {
                try
                {
                    c.Client.Export.Request("test", SlideRoom.Resources.RequestFormat.Csv);
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual("Invalid Format", e.Message);
                    Assert.AreEqual(System.Net.HttpStatusCode.NotFound, e.StatusCode);
                }
                catch
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            }
        }
    }
}
