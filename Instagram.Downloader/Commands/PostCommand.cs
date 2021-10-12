using Feli.Instagram.Downloader.Commanding.Exceptions;
using Feli.Instagram.Downloader.Posts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class PostCommand : Command
    {
        public override string Name => "post";

        public override string Syntax => "post [instagram profile post url]";

        public override bool RequiresLogin => true;

        public override string Description => "Downloads a post from a specified url";

        private readonly HttpClient httpClient;

        public PostCommand(HttpClient httpClient, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.httpClient = httpClient;
        }

        public override async Task OnExecuteAsync(string[] args)
        {
            if (args.Length < 1)
                throw new InvalidUsageException();

            var url = args[0];

            Writer.WriteLine("[Post Downloader] Getting post..", ConsoleColor.Green);
            var post = await ProfilePost.GetPostAsync(httpClient, url);
            Writer.WriteLine("[Post Downloader] Got post", ConsoleColor.Green);

            Writer.WriteLine("[Post Downloader] Downloading..", ConsoleColor.Green);
            await InstagramClient.DownloadProfilePostAsync(post);
            Writer.Write("[Post Downloader] Done", ConsoleColor.Green);
        }
    }
}
