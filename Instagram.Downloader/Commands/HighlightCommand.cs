using Feli.Instagram.Downloader.Commanding.Exceptions;
using Feli.Instagram.Downloader.Posts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class HighlightCommand : Command
    {
        public override string Name => "highlight";

        public override string Syntax => "highlight [instagram highlight url]";

        public override bool RequiresLogin => true;
        public override string Description => "Downloads a highlight from the specified url";

        private readonly HttpClient httpClient;

        public HighlightCommand(HttpClient httpClient, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.httpClient = httpClient;
        }

        public override async Task OnExecuteAsync(string[] args)
        {
            if (args.Length < 1)
                throw new InvalidUsageException();

            var url = args[0];

            Writer.WriteLine("[Highlight Downloader] Getting post..", ConsoleColor.Green);
            var post = await HighlightPost.GetPostAsync(httpClient, url);
            Writer.WriteLine("[Highlight Downloader] Got post", ConsoleColor.Green);

            Writer.WriteLine("[Highlight Downloader] Downloading..", ConsoleColor.Green);
            await InstagramClient.DownloadHighlightPostAsync(post);
            Writer.Write("[Highlight Downloader] Done", ConsoleColor.Green);
        }
    }
}
