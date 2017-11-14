using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace TextGenerator.TextSources
{
    class RedditAuth
    {
        private const string AUTH_URL = "https://www.reddit.com/api/v1/access_token";
        private const string APP_ID = "8bfffca7-f652-41e3-b33d-6bbd664a3ffb";
        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;
        private string _token;

        public RedditAuth(string clientId, string clientSecret)
        {
            CLIENT_ID = clientId;
            CLIENT_SECRET = clientSecret;
        }
        private void GetOAuthToken()
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "https://oauth.reddit.com/grants/installed_client"),
                    new KeyValuePair<string, string>("device_id", APP_ID)
                });
                string encoded = Convert.ToBase64String(Encoding.Default.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET}"));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
                var result = client.PostAsync(AUTH_URL, content).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = JsonConvert.DeserializeObject<RedditAuthResponse>(result.Content.ReadAsStringAsync().Result);
                    _token = response.Access_Token;
                }

            }
        }

        private class RedditAuthResponse
        {
            public string Access_Token { get; set; }
            public string Token_Type { get; set; }
            public int Expires_In { get; set; }
            public string Scope { get; set; }
        }
    }

}
