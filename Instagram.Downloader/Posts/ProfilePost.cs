using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Posts
{
    public class ProfilePost
    {
        public JObject Data { get; set; }

        public static async Task<ProfilePost> GetPostAsync(HttpClient client, string url)
        {
            var graphqlUrl = url += "?__a=1";

            var response = await client.GetAsync(graphqlUrl);

            var content = await response.Content.ReadAsStringAsync();

            return new ProfilePost()
            {
                Data = JObject.Parse(content)
            };
        }
    }
}
