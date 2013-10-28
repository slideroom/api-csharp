using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlideRoomTest
{
    [TestClass]
    public class GeneralClient
    {
        [TestMethod]
        public void SignCorrectly()
        {
            var apiKey = "123";
            var accessKey = "456";

            NameValueCollection params1 = new NameValueCollection();
            params1.Add("email", "test@test.com");
            Assert.AreEqual("nDRWry9G/lr8S9UQKlC1Ih6csUs=", SlideRoom.SlideRoomClient.SignParameters(params1, apiKey, accessKey));


            NameValueCollection params2 = new NameValueCollection();
            params2.Add("email", "tEst@tEsT.cOm");
            Assert.AreEqual("nDRWry9G/lr8S9UQKlC1Ih6csUs=", SlideRoom.SlideRoomClient.SignParameters(params2, apiKey, accessKey));

            // test ordering
            NameValueCollection params3 = new NameValueCollection();
            params3.Add("email", "test@test.com");
            params3.Add("export", "TestExport");
            Assert.AreEqual("02R1j+skg0jwXGyoErb5zdq0r38=", SlideRoom.SlideRoomClient.SignParameters(params3, apiKey, accessKey));

            NameValueCollection params4 = new NameValueCollection();
            params4.Add("export", "TestExport");
            params4.Add("email", "test@test.com");
            Assert.AreEqual("02R1j+skg0jwXGyoErb5zdq0r38=", SlideRoom.SlideRoomClient.SignParameters(params4, apiKey, accessKey));
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

            TestClient.GetTestClient(res, (c) =>
            {
                try
                {
                    c.GetRawResponse("bad", new NameValueCollection());
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual("Unterminated string. Expected delimiter: \". Path '', line 1, position 7.", e.Message);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                }
                catch (Exception e)
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            });
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

            TestClient.GetTestClient(res, (c) =>
            {
                try
                {
                    c.GetRawResponse("bad", new NameValueCollection());
                    Assert.Fail("should throw an exception");
                }
                catch (SlideRoom.SlideRoomAPIException e)
                {
                    Assert.AreEqual(message, e.Message);
                    Assert.AreEqual(code, e.StatusCode);
                }
                catch (Exception e)
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            });
        }
    }
}
