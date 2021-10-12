using Feli.Instagram.Downloader.Auth.Har;
using Feli.Instagram.Downloader.Commanding.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Feli.Instagram.Downloader.Commands
{
    public class LoginCommand : Command
    {
        public override string Name => "login";
        public override string Syntax => "login [har file path: Optional] With no arguments i will try to log in using saved credentials";
        public override bool RequiresLogin => false;
        public override string Description => "Logs in to instagram for interacting with the api";

        public LoginCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task OnExecuteAsync(string[] args)
        {
            //if(args.Length < 2)
            //{
            //    var username = args[0];
            //    var password = args[1];


            //}
            if(args.Length < 1)
            {
                Writer.WriteLine("Trying to log in with saved credentials..", ConsoleColor.Cyan);

                var result = await InstagramClient.LoginAsync();

                if (!result)
                {
                    Writer.Write("Credentials file was not found in cache", ConsoleColor.Red);
                    return;
                }
            }
            else
            {
                var har = args[0];

                if (!File.Exists(har))
                {
                    Writer.Write("[!] ", ConsoleColor.Red);
                    Writer.Write("The har file does not exist", ConsoleColor.DarkRed);
                    return;
                }

                var result = await InstagramClient.LoginAsync(HarAuthProvider.GetAuthCredential(har));

                if (!result)
                    Writer.Write("Could not log in");
            }
        }
    }
}
