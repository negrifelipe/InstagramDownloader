using Feli.Instagram.Downloader.Console;
using Feli.Instagram.Downloader.Posts;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Client
{
    public class InstagramClient
    {
        private readonly HttpClient httpClient;
        private readonly ConsoleWriter consoleWriter;
        public bool IsLoggedIn { get; set; } = false;

        public InstagramClient(
            ConsoleWriter consoleWriter,
            HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.consoleWriter = consoleWriter;

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 9; SM-A102U Build/PPR1.180610.011; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/74.0.3729.136 Mobile Safari/537.36 Instagram 155.0.0.37.107 Android (28/9; 320dpi; 720x1468; samsung; SM-A102U; a10e; exynos7885; en_US; 239490550)");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        public async Task<bool> LoginAsync()
        {
            if (!Directory.Exists("Cache") || !File.Exists(Path.Combine("Cache", "credentials")))
                return false;

            var credentials = await File.ReadAllTextAsync(Path.Combine("Cache", "credentials"));

            return await LoginAsync(credentials);
        }

        public async Task<bool> LoginAsync(string username, string password)
        {

        }

        public async Task<bool> LoginAsync(string cookies)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", cookies);

            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");

            var response = await httpClient.GetAsync("https://instagram.com/");

            if (!response.IsSuccessStatusCode)
                return false;

            var document = new HtmlDocument();
            document.LoadHtml(await response.Content.ReadAsStringAsync());

            var script = document.DocumentNode.SelectNodes("/html/body/script")[10];

            var sharedData = "window._sharedData = ";

            var unparsedScript = script.InnerText.Substring(sharedData.Length).Replace(";", "");

            var @object = JObject.Parse(unparsedScript);

            await File.WriteAllTextAsync(Path.Combine("Cache", "credentials"), cookies);

            IsLoggedIn = true;

            consoleWriter.WriteLine($"Successfully logged in as {@object["config"]["viewer"]["username"]}", ConsoleColor.Green);

            return IsLoggedIn;
        }

        public async Task DownloadProfilePostAsync(ProfilePost post)
        {
            var isMultiple = post.Data.SelectToken("$.graphql.shortcode_media.edge_sidecar_to_children", false) != null;

            if(isMultiple)
            {
                foreach(var data in post.Data["graphql"]["shortcode_media"]["edge_sidecar_to_children"]["edges"] as JArray)
                {
                    await DownloadPostAsync(data["node"]);
                }
            }
            else
            {
                await DownloadPostAsync(post.Data["graphql"]["shortcode_media"]);
            }
        }

        internal async Task DownloadPostAsync(JToken post)
        {
            var isVideo = (bool)post["is_video"];

            var id = post["id"].ToString();
            var url = string.Empty;
  
            if (isVideo) 
            {
                url = post["video_url"].ToString();
            }
            else
            {
                url = (post["display_resources"] as JArray).OrderByDescending(x => (int)x["config_width"] * (int)x["config_height"]).FirstOrDefault()["src"].ToString();
            }

            var extension = url.Substring(url.IndexOf("_n.") + 2, url.IndexOf('?'));

            extension = extension.Remove(extension.IndexOf('?'));

            var response = await httpClient.GetAsync(url);

            var buffer = await response.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(id + extension, buffer);
        }

        public async Task DownloadHighlightPostAsync(HighlightPost post)
        {
            var media = post.Data.SelectToken($"$.reels.highlight:{post.Id}");

            var folder = media.SelectToken("$.title").ToString();

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var items = media["items"] as JArray;

            foreach (var reel in items)
            {
                await DownloadReel(reel);
            }
        }

        public Task DownloadStoryPostAsync(StoryPost post)
        {
            var items = post.Data["reels"][post.Id]["items"] as JArray;

            var reel = items.FirstOrDefault(r => r["pk"].ToString() == post.StoryId);

            return DownloadReel(reel);
        }

        private async Task DownloadReel(JToken reel)
        {
            var isVideo = (int)reel["media_type"] == 2;

            var url = string.Empty;

            if (isVideo)
            {
                url = (reel["video_versions"] as JArray).OrderByDescending(v => (int)v["width"] * (int)v["height"]).FirstOrDefault()["url"].ToString();
            }
            else
            {
                url = (reel["image_versions2"]["candidates"] as JArray).OrderByDescending(v => (int)v["width"] * (int)v["height"]).FirstOrDefault()["url"].ToString();
            }

            var extension = url.Substring(url.IndexOf("_n.") + 2, url.IndexOf('?'));

            extension = extension.Remove(extension.IndexOf('?'));

            var response = await httpClient.GetAsync(url);

            var buffer = await response.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(reel["pk"].ToString() + extension, buffer);
        }
    }
}
