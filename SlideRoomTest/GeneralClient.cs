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
            Assert.AreEqual(SlideRoom.SlideRoomClient.SignParameters(params1, apiKey, accessKey), "nDRWry9G/lr8S9UQKlC1Ih6csUs=");


            NameValueCollection params2 = new NameValueCollection();
            params2.Add("email", "tEst@tEsT.cOm");
            Assert.AreEqual(SlideRoom.SlideRoomClient.SignParameters(params2, apiKey, accessKey), "nDRWry9G/lr8S9UQKlC1Ih6csUs=");
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
                    Assert.AreEqual(e.Message, "Unterminated string. Expected delimiter: \". Path '', line 1, position 7.");
                    Assert.AreEqual(e.StatusCode, HttpStatusCode.InternalServerError);
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
                    Assert.AreEqual(e.Message, message);
                    Assert.AreEqual(e.StatusCode, code);
                }
                catch (Exception e)
                {
                    Assert.Fail("should throw a SlideRoomAPIException");
                }
            });
        }
    }
}
