using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace TextGenerator.TextSources
{
    public class Twitter : ITextSource
    {
        private const string oAuthUrl = "https://api.twitter.com/oauth2/token";
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _handle;
        public Twitter(string key, string secret, string handle)
        {
            _apiKey = key;
            _apiSecret = secret;
            _handle = handle;
        }

        public IEnumerable<string> GetTextFromSource()
        {
            var auth = GetAuthenticateResponse();

            // Do the timeline
            var timelineFormat = $"https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={_handle}&include_rts=0&exclude_replies=1&count=200";
            var timelineUrl = timelineFormat;
            List<string> result = new List<string>();
            bool go = true;
            while (go)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(timelineUrl);
                request.Headers.Add("Authorization", $"{auth.token_type} {auth.access_token}");
                request.Method = "Get";
                var timeLineJson = string.Empty;
                using (WebResponse response = request.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                        timeLineJson = reader.ReadToEnd();
                var lst = JsonConvert.DeserializeObject<IEnumerable<Twit>>(timeLineJson);
                result.AddRange(lst.Select(x => x.text));
                go = go && lst.Count() > 1;
                if (go)
                {
                    var id = lst.Min(x => x.id);
                    timelineUrl = timelineFormat + "&max_id=" + id.ToString();
                }
            }
            return result;
        }

        private TwitAuthenticateResponse GetAuthenticateResponse()
        {
            var authHeaderFormat = "Basic {0}";

            var authHeader = string.Format(authHeaderFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(_apiKey) + ":" +
                Uri.EscapeDataString((_apiSecret)))
            ));

            var postBody = "grant_type=client_credentials";

            HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            authRequest.Headers.Add("Authorization", authHeader);
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (Stream stream = authRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            authRequest.Headers.Add("Accept-Encoding", "gzip");

            WebResponse authResponse = authRequest.GetResponse();
            // deserialize into an object
            TwitAuthenticateResponse twitAuthResponse;
            using (authResponse)
            {
                using (var reader = new StreamReader(authResponse.GetResponseStream()))
                {
                    var objectText = reader.ReadToEnd();
                    twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                }
            }
            return twitAuthResponse;
        }

        public class Twit
        {
            public long id { get; set; }
            public string text { get; set; }
        }

        public class TwitAuthenticateResponse
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
        }
    }
}
