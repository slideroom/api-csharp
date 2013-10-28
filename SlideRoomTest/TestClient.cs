using System;

namespace SlideRoomTest
{
    public class TestClient : IDisposable
    {
        private MockServer server { get; set; }

        public SlideRoom.SlideRoomClient Client { get; set; }

        public TestClient(MockResponse res)
        {
            server = new MockServer(res);
            Client = new SlideRoom.SlideRoomClient("123", "456", "test", "test@test.com", "http://localhost:" + server.Port + "/");
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
