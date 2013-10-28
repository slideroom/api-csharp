using System;

namespace SlideRoomTest
{
    public class TestClient
    {
        public static void GetTestClient(MockResponse res, Action<SlideRoom.SlideRoomClient> runner)
        {
            MockServer.Run(res, (port) =>
            {
                SlideRoom.SlideRoomClient client = new SlideRoom.SlideRoomClient("123", "456", "test", "test@test.com", "http://localhost:" + port + "/");
                runner(client);
            });
        }
    }
}
