using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlideRoomTest
{
    public class MockResponse
    {
        public string ContentType { get; set; }
        public string Body { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
