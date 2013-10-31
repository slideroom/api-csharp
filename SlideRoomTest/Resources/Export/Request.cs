using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using SlideRoomTest.Extensions;

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
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv);

                // test the request
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "csv");
                queryString.NotContains("ss");

                var expectedResult = new SlideRoom.API.Resources.RequestResult()
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
        public void GoodRequestWithSavedSearch()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv, "saved search");

                // test the request too..
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "csv");
                queryString.ContainsAndEquals("ss", "saved search");

                var expectedResult = new SlideRoom.API.Resources.RequestResult()
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
                    c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv);
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.API.SlideRoomAPIException e)
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
