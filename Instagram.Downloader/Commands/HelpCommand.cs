using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => "help";

        public override string Syntax => "help";

        public override string Description => "A command that display info from other commands";

        public override bool RequiresLogin => false;

        private readonly IServiceProvider serviceProvider;

        public HelpCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override Task OnExecuteAsync(string[] args)
        {
            var commandExecutor = (CommandExecutor)serviceProvider.GetService(typeof(CommandExecutor));
            Writer.WriteLine("Commands: ", ConsoleColor.Cyan);
            foreach(var command in commandExecutor.Commands)
            {
                Writer.Write("Name: ", ConsoleColor.Yellow);
                Writer.Write(command.Name, ConsoleColor.Cyan);
                Writer.Write(" Description: ", ConsoleColor.Yellow);
                Writer.Write(command.Description + Environment.NewLine, ConsoleColor.Cyan);
                Writer.Write(" Syntax: ", ConsoleColor.Yellow);
                Writer.Write(command.Syntax + Environment.NewLine, ConsoleColor.Cyan);
            }

            return Task.CompletedTask;
        }
    }
}
