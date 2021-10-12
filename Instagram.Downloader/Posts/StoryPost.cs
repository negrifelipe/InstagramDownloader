using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Posts
{
    public class StoryPost
    {
        public JObject Data { get; set; }
        public string Id { get; set; }
        public string StoryId { get; set; }

        public static async Task<StoryPost> GetPostAsync(HttpClient client, string url)
        {
            var regex = new Regex("^[0-9]+$");

            var storyId = url.Split('/').FirstOrDefault(x => regex.IsMatch(x));

            var graphqlUrl = url += url.Contains("?") ? "&__a=1" : "?__a=1";

            var response = await client.GetAsync(graphqlUrl);

            var content = await response.Content.ReadAsStringAsync();
            
            var storyData = JObject.Parse(content);

            var id = storyData.SelectToken("$.user.id").ToString();

            var reelData = await client.GetAsync($"https://i.instagram.com/api/v1/feed/reels_media/?reel_ids={id}");

            var unparsedReel = await reelData.Content.ReadAsStringAsync();

            var reel = JObject.Parse(unparsedReel);

            return new StoryPost()
            {
                Id = id,
                StoryId = storyId,
                Data = reel
            };
        }
    }
}
