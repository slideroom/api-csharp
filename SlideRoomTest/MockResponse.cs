using System.Net;

namespace SlideRoomTest
{
    public class MockResponse
    {
        public string ContentType { get; set; }
        public string Body { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
