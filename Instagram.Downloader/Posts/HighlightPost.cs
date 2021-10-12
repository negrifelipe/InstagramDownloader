using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Posts
{
    public class HighlightPost
    {
        public JObject Data { get; set; }

        public string Id { get; set; }

        public static async Task<HighlightPost> GetPostAsync(HttpClient client, string url)
        {
            var regex = new Regex("^[0-9]+$");

            var id = url.Split('/').FirstOrDefault(x => regex.IsMatch(x));

            var response = await client.GetAsync($"https://i.instagram.com/api/v1/feed/reels_media/?reel_ids=highlight:{id}");

            var content = await response.Content.ReadAsStringAsync();

            return new HighlightPost()
            {
                Id = id,
                Data = JObject.Parse(content)
            };
        }
    }
}
