using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace SlideRoom
{
    public class SlideRoomClient
    {
        private const string DEFAULT_BASE_URL = "https://api.slideroom.com";

        private string BaseUrl { get; set; }

        private string APIKey { get; set; }
        private string AccessKey { get; set; }
        private string OrganizationCode { get; set; }
        private string EmailAddress { get; set; }
        private TimeSpan RequestLifespan { get; set; }

        public Resources.Export Export { get; private set; }

        public SlideRoomClient(string apiKey, string accessKey, string orgainationCode, string emailAddress, string baseUrl)
        {
            BaseUrl = baseUrl;
            if (BaseUrl.EndsWith("/"))
            {
                BaseUrl = BaseUrl.TrimEnd('/');
            }

            APIKey = apiKey;
            AccessKey = accessKey;
            OrganizationCode = orgainationCode;
            EmailAddress = emailAddress;

            RequestLifespan = TimeSpan.FromMinutes(1);

            SetUpResources();
        }

        public SlideRoomClient(string apiKey, string accessKey, string orgainationCode, string emailAddress)
            : this(apiKey, accessKey, orgainationCode, emailAddress, DEFAULT_BASE_URL)
        {
        }

        // If an empty parameter list, try to set up via app settings
        public SlideRoomClient()
            :this(
                ConfigurationManager.AppSettings["SlideRoomApiKey"] ?? "",
                ConfigurationManager.AppSettings["SlideRoomAccessKey"] ?? "",
                ConfigurationManager.AppSettings["SlideRoomEmailAddress"] ?? "",
                ConfigurationManager.AppSettings["SlideRoomOrganizationCode"] ?? "",
                ConfigurationManager.AppSettings["SlideRoomBaseURL"] ?? DEFAULT_BASE_URL)
        {
        }


        private void SetUpResources()
        {
            Export = new Resources.Export(this);
        }


        public static string SignParameters(NameValueCollection nvc, string apiHashKey, string accessKey)
        {
            var collectionToSign = new SortedDictionary<string, string>();
            foreach (var key in nvc.AllKeys)
            {
                collectionToSign.Add(key, nvc[key]);
            }
            collectionToSign.Add("access-key", accessKey);

            var stringToSign = String.Join("\n", collectionToSign.ToList().ConvertAll((kvp) => (kvp.Key + "=" + kvp.Value)).ToArray());
            return Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(apiHashKey)).ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToLower())));
        }


        private string BuildURL(string path, NameValueCollection paramsToSend)
        {
            var now = (long)((TimeSpan)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            var expires = now + RequestLifespan.TotalSeconds;

            NameValueCollection paramBuilder = HttpUtility.ParseQueryString(String.Empty);

            paramBuilder.Add("email", EmailAddress);
            paramBuilder.Add("expires", expires.ToString());

            foreach (var key in paramsToSend.AllKeys)
            {
                paramBuilder.Add(key, paramsToSend[key]);
            }

            var signature = SignParameters(paramBuilder, APIKey, AccessKey);

            paramBuilder.Add("signature", signature);

            return BaseUrl + "/" + OrganizationCode + "/" + path + "?" + paramBuilder.ToString();
        }


        public HttpWebResponse GetRawResponse(string path, NameValueCollection nvc)
        {
            var url = BuildURL(path, nvc);
            var request = HttpWebRequest.Create(url);
            request.Method = "GET";

            try
            {
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                HandleBadResponse(e.Response as HttpWebResponse);
            }

            return null;
        }


        private string ReadEntireResponse(HttpWebResponse res)
        {
            var responseText = String.Empty;
            using (var responseStream = res.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream))
                {
                    responseText = reader.ReadToEnd();
                }
            }

            return responseText;
        }


        public void HandleBadResponse(HttpWebResponse res)
        {
            try
            {
                var responseText = ReadEntireResponse(res);
                var exceptionResponse = JsonConvert.DeserializeObject<SlideRoomAPIExceptionResponse>(responseText);
                throw new SlideRoomAPIException(exceptionResponse.Message, res.StatusCode);
            }
            catch (Exception e)
            {
                throw new SlideRoomAPIException(e.Message, res.StatusCode);
            }

        }

        public T GetExpectedJSONResult<T>(string path, NameValueCollection nvc)
        {
            var responseText = ReadEntireResponse(GetRawResponse(path, nvc));

            // parse response as json, if possible
            if (!String.IsNullOrWhiteSpace(responseText))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(responseText);
                }
                catch (Exception e)
                {
                    throw new SlideRoomAPIException(e.Message);
                }
            }

            throw new SlideRoomAPIException("Empty JSON Response");
        }
    }
}
