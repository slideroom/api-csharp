using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
