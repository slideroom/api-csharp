using System;
using System.Net;

namespace SlideRoomTest
{
    public class TestClient : IDisposable
    {
        private MockServer server { get; set; }

        public SlideRoom.SlideRoomClient Client { get; set; }
        public HttpListenerRequest Request { get; set; }

        public const string ApiHashKey = "123";
        public const string AccessKey = "456";
        public const string Organization = "test";
        public const string EmailAddress = "test@test.com";

        public TestClient(MockResponse res)
        {
            server = new MockServer(res, ctx => Request = ctx);
            Client = new SlideRoom.SlideRoomClient(ApiHashKey, AccessKey, Organization, EmailAddress, "http://localhost:" + server.Port + "/");
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
