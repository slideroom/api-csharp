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
                queryString.NotContains("since");

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
        public void GoodRequestTsv()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Tsv);

                // test the request
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "tsv");
                queryString.NotContains("ss");
                queryString.NotContains("since");

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
        public void GoodRequestXlsx()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Xlsx);

                // test the request
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "xlsx");
                queryString.NotContains("ss");
                queryString.NotContains("since");

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
        public void GoodRequestWithSince()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            var since = DateTime.Now;
            var sinceVal = (long)((TimeSpan)(since.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv, null, since);

                // test the request too..
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "csv");
                queryString.NotContains("ss");
                queryString.ContainsAndEquals("since", sinceVal.ToString());

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
        public void GoodRequestWithSinceAndSavedSearch()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            var since = DateTime.Now;
            var sinceVal = (long)((TimeSpan)(since.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv, "saved search", since);

                // test the request too..
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "csv");
                queryString.ContainsAndEquals("ss", "saved search");
                queryString.ContainsAndEquals("since", sinceVal.ToString());

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
                queryString.NotContains("since");

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
        public void GoodRequestWithPool()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = System.Net.HttpStatusCode.OK,
                Body = @"{ ""token"": 123, ""submissions"": 456, ""message"": ""test""}"
            };

            using (var c = new TestClient(res))
            {
                var actualResult = c.Client.Export.Request("test", SlideRoom.API.Resources.RequestFormat.Csv, "saved search", null, SlideRoom.API.Resources.Pool.Archived);

                // test the request too..
                var queryString = c.Request.QueryString;
                GeneralClient.TestRequiredParameters(queryString);

                queryString.ContainsAndEquals("export", "test");
                queryString.ContainsAndEquals("format", "csv");
                queryString.ContainsAndEquals("ss", "saved search");
                queryString.NotContains("since");
                queryString.ContainsAndEquals("pool", "archived");
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
