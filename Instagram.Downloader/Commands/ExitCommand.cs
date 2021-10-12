using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class ExitCommand : Command
    {
        public override string Name => "exit";

        public override string Syntax => "exit";

        public override string Description => "Exits the application";

        public override bool RequiresLogin => false;

        public ExitCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Task OnExecuteAsync(string[] args)
        {
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }
}
