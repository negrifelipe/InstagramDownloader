using Feli.Instagram.Downloader.Client;
using Feli.Instagram.Downloader.Console;
using System;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Syntax { get; }
        public abstract string Description { get; }
        public abstract bool RequiresLogin { get; }

        public InstagramClient InstagramClient { get; set; }
        public ConsoleWriter Writer { get; set; }

        protected Command(IServiceProvider serviceProvider) 
        {
            InstagramClient = (InstagramClient)serviceProvider.GetService(typeof(InstagramClient));
            Writer = (ConsoleWriter)serviceProvider.GetService(typeof(ConsoleWriter));
        }

        public async Task ExecuteAsync(string[] args) 
        {
            if(!InstagramClient.IsLoggedIn && RequiresLogin)
            {
                Writer.WriteLine("[Log In] Please log in to execute this command", ConsoleColor.Green);
                return;
            }

            await OnExecuteAsync(args);
            Writer.WriteLine("");
        }

        public abstract Task OnExecuteAsync(string[] args);
    }
}
