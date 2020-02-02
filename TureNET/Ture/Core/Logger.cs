using System;

namespace Ture.Core
{
    class Logger
    {
        public Logger() { }

        public void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void Info(string text)
        {
            Console.WriteLine(text);
        }

        public void Warn(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warn: {text}");
            Console.ResetColor();
        }

        public void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {text}");
            Console.ResetColor();
        }
    }
}
