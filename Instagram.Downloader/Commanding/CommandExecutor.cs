using Feli.Instagram.Downloader.Commanding.Exceptions;
using Feli.Instagram.Downloader.Console;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class CommandExecutor
    {
        public readonly List<Command> Commands;
        private readonly ConsoleWriter writer;

        public CommandExecutor(ConsoleWriter writer, IServiceProvider serviceProvider)
        {
            this.writer = writer;
            var commandTypes = GetType().Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(Command)) && !x.IsAbstract);

            Commands = commandTypes.Select(x => (Command)ActivatorUtilities.CreateInstance(serviceProvider, x)).ToList();
        }

        public async Task ListenAsync()
        {
            while (true)
            {
                writer.Write(" > ", ConsoleColor.Cyan);

                var input = System.Console.ReadLine();

                if (input == null || input == string.Empty)
                {
                    continue;
                }

                var args = input.Split(' ').ToList();

                var name = args[0];

                args.RemoveAt(0);

                var command = Commands.FirstOrDefault(x => x.Name == name);

                if(command == null)
                {
                    writer.Write("[!] ", ConsoleColor.Red);
                    writer.Write($"Could not find any command called {name}", ConsoleColor.DarkRed);
                    writer.WriteLine("");
                    continue;
                }
                
                try
                {
                    await command.ExecuteAsync(args.ToArray());
                }
                catch (InvalidUsageException)
                {
                    writer.Write("[!] ", ConsoleColor.Red);
                    writer.Write("Invalid command usage for", ConsoleColor.DarkRed);
                    writer.Write($" {command.Name}", ConsoleColor.Cyan);
                    writer.Write(". Usage: ", ConsoleColor.DarkRed);
                    writer.Write($"{command.Syntax}", ConsoleColor.Cyan);
                    writer.WriteLine("");
                }
                catch (Exception ex)
                {
                    writer.Write("[!] Exception ", ConsoleColor.Red);
                    writer.Write(ex.ToString(), ConsoleColor.Yellow);
                    writer.WriteLine("");
                }
            }
        }
    }
}
