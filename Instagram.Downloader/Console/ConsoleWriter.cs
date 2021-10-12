using System;

namespace Feli.Instagram.Downloader.Console
{
    public class ConsoleWriter
    {
        public void Write(string value, ConsoleColor color = ConsoleColor.White)
        {
            System.Console.ForegroundColor = color;
            System.Console.Write(value);
            System.Console.ResetColor();
        }

        public void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(value);
            System.Console.ResetColor();
        }
    }
}
