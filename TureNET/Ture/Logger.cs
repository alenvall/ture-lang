using System;
using System.Collections.Generic;
using System.Text;

namespace Ture
{
    class Logger
    {
        public Logger() { }

        public void Debug(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.WriteLine($"WARN: {text}");
            Console.ResetColor();
        }

        public void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {text}");
            Console.ResetColor();
        }
    }
}
