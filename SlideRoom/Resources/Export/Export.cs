using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace SlideRoom.API.Resources
{
    public class Export
    {
        private SlideRoomClient client { get; set; }

        public Export(SlideRoomClient c)
        {
            client = c;
        }

        public RequestResult Request(string exportName, RequestFormat format, string savedSearch = null)
        {
            var nvc = new NameValueCollection();
            nvc["export"] = exportName;
            nvc["format"] = format.ToString().ToLower();

            if (!String.IsNullOrEmpty(savedSearch))
            {
                nvc["ss"] = savedSearch;
            }

            return client.GetExpectedJSONResult<RequestResult>("export/request", nvc);
        }

        public DownloadResult Download(int token)
        {
            var nvc = new NameValueCollection();
            nvc["token"] = token.ToString();

            var rawResponse = client.GetRawResponse("export/download", nvc);

            if (rawResponse.StatusCode == HttpStatusCode.OK)
            {
                return new DownloadResult()
                {
                    Pending = false,
                    ExportStream = rawResponse.GetResponseStream()
                };
            }
            else
            {
                return new DownloadResult()
                {
                    Pending = true,
                    ExportStream = null
                };
            }
        }
    }


    public class RequestResult
    {
        [JsonProperty("token")]
        public int Token { get; set; }

        [JsonProperty("submissions")]
        public int Submissions { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }


    public class DownloadResult
    {
        public bool Pending { get; set; }
        public Stream ExportStream { get; set; }
    }
}
