using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using Newtonsoft.Json;

namespace TextGenerator.TextSources
{
    public class RedditTitles : ITextSource
    {
        private readonly string _subReddit;
        public RedditTitles(string subReddit)
        {
            _subReddit = subReddit;
        }

        public IEnumerable<string> GetTextFromSource()
        {
            using (var client = new HttpClient())
            {
                var result = client.GetStringAsync($"https://www.reddit.com/r/{_subReddit}/top.json?limit=100").Result;
                var response = JsonConvert.DeserializeObject<RedditObject<SubRedditResult>>(result);
                return response.Data.Children.Select(x => x.Data.Title);
            }
        }

        private class RedditThread
        {
            public string Title { get; set; }
        }

        private class SubRedditResult
        {
            public IEnumerable<RedditObject<RedditThread>> Children { get; set; }
        }

        private class RedditObject<t>
        {
            public t Data { get; set; }
        }
    }
}
