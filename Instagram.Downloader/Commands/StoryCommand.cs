using Feli.Instagram.Downloader.Commanding.Exceptions;
using Feli.Instagram.Downloader.Posts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class StoryCommand : Command
    {
        public override string Name => "story";

        public override string Syntax => "story [instagram story url]";

        public override bool RequiresLogin => true;

        public override string Description => "Downloads a story from a specified url";

        private readonly HttpClient httpClient;

        public StoryCommand(HttpClient httpClient, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.httpClient = httpClient;
        }

        public override async Task OnExecuteAsync(string[] args)
        {
            if (args.Length < 1)
                throw new InvalidUsageException();

            var url = args[0];

            Writer.WriteLine("[Story Downloader] Getting post..", ConsoleColor.Green);
            var post = await StoryPost.GetPostAsync(httpClient, url);
            Writer.WriteLine("[Story Downloader] Got post", ConsoleColor.Green);

            Writer.WriteLine("[Story Downloader] Downloading..", ConsoleColor.Green);
            await InstagramClient.DownloadStoryPostAsync(post);
            Writer.Write("[Story Downloader] Done", ConsoleColor.Green);
        }
    }
}
