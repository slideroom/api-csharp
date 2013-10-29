using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlideRoomTest.Extensions;
using System.Collections.Specialized;
using System.Net;

namespace SlideRoomTest
{
    [TestClass]
    public class GeneralClient
    {
        [TestMethod]
        public void SignCorrectly()
        {
            var params1 = new NameValueCollection();
            params1["email"] = "test@test.com";
            Assert.AreEqual("nDRWry9G/lr8S9UQKlC1Ih6csUs=", SlideRoom.SlideRoomClient.SignParameters(params1, TestClient.ApiHashKey, TestClient.AccessKey));

            var params2 = new NameValueCollection();
            params2["email"] = "tEst@tEsT.cOm";
            Assert.AreEqual("nDRWry9G/lr8S9UQKlC1Ih6csUs=", SlideRoom.SlideRoomClient.SignParameters(params2, TestClient.ApiHashKey, TestClient.AccessKey));

            // test ordering
            var params3 = new NameValueCollection();
            params3["email"] = "test@test.com";
            params3["export"] = "TestExport";
            Assert.AreEqual("02R1j+skg0jwXGyoErb5zdq0r38=", SlideRoom.SlideRoomClient.SignParameters(params3, TestClient.ApiHashKey, TestClient.AccessKey));

            var params4 = new NameValueCollection();
            params4["export"] = "TestExport";
            params4["email"] = "test@test.com";
            Assert.AreEqual("02R1j+skg0jwXGyoErb5zdq0r38=", SlideRoom.SlideRoomClient.SignParameters(params4, TestClient.ApiHashKey, TestClient.AccessKey));
        }


        [TestMethod]
        public void HandleMalformedJSON()
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = HttpStatusCode.InternalServerError,
                Body = @"{ ""mess"
            };


            using (var c = new TestClient(res))
            {
                try
                {
                    c.Client.GetRawResponse("bad", new NameValueCollection());
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual("Unterminated string. Expected delimiter: \". Path '', line 1, position 7.", e.Message);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                }
                catch
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            }
        }

        [TestMethod]
        public void BadResponse410()
        {
            MockBadResponse("410", HttpStatusCode.Gone);
        }

        [TestMethod]
        public void BadResponse404()
        {
            MockBadResponse("404", HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void BadResponse403()
        {
            MockBadResponse("403", HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void BadResponse400()
        {
            MockBadResponse("400", HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void BadResponse500()
        {
            MockBadResponse("500", HttpStatusCode.InternalServerError);
        }

        private static void MockBadResponse(string message, HttpStatusCode code)
        {
            var res = new MockResponse()
            {
                ContentType = "application/json",
                Code = code,
                Body = @"{ ""message"": """ + message + @""" } "
            };

            using (var c = new TestClient(res))
            {
                try
                {
                    c.Client.GetRawResponse("bad", new NameValueCollection());
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual(message, e.Message);
                    Assert.AreEqual(code, e.StatusCode);
                }
                catch
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            }
        }

        public static void TestRequiredParameters(NameValueCollection query)
        {
            query.ContainsAndEquals("email", TestClient.EmailAddress);
            query.Contains("expires");
            query.Contains("signature");
        }
    }
}
