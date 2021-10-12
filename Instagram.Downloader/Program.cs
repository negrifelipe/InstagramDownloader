using Feli.Instagram.Downloader.Client;
using Feli.Instagram.Downloader.Commands;
using Feli.Instagram.Downloader.Console;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader
{
    public class Program
    {
        private static ServiceProvider serviceProvider;

        public static async Task Main(string[] args)
        {
            serviceProvider = new ServiceCollection()
                .AddSingleton<HttpClient>()
                .AddSingleton<ConsoleWriter>()
                .AddSingleton<InstagramClient>()
                .AddSingleton<CommandExecutor>()
                .BuildServiceProvider();

            var writer = serviceProvider.GetRequiredService<ConsoleWriter>();

            writer.WriteLine("Welcome to Instagram Downloader", ConsoleColor.Cyan);
            writer.WriteLine("To get help write the help command", ConsoleColor.Green);

            if (args.Any(x => x == "--login" || x == "-l"))
            {
                writer.WriteLine("Trying to log in with saved credentials..", ConsoleColor.Cyan);
                var result = await serviceProvider.GetRequiredService<InstagramClient>().LoginAsync();

                if (!result)
                {
                    writer.WriteLine("Credentials file was not found in cache", ConsoleColor.Red);
                }
            }

            await serviceProvider.GetRequiredService<CommandExecutor>().ListenAsync();
        }
    }
}
