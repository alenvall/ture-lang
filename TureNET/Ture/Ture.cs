using System;
using System.Collections.Generic;
using System.IO;

namespace Ture
{
    class Ture
    {
        private const string VER = "0.0.1";
        private static Logger log;
        private static bool ErrorOccured = false;

        static void Main(string[] args)
        {
            log = new Logger();

            if (args.Length > 1)
            {
                log.Info("Woof! Usage: ture [script.tr]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void PrintIntro()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.Info("       __");
            log.Info("  (___()'`;");
            log.Info("  /,    /`   _");
            log.Info("  \\\\\"--\\\\   (_)");
            log.Info($"\nTure [ver. {VER}]");
            Console.ResetColor();
        }

        private static void RunFile(string fileName)
        {
            var source = "";

            try
            {
                fileName = fileName.Trim();

                if (!fileName.EndsWith(".tr"))
                {
                    fileName += ".tr";
                }
                source = File.ReadAllText(fileName);
            }
            catch (Exception)
            {
                log.Error("Problem reading file!");
            }

            if (source.Length > 0)
            {
                PrintIntro();
                log.Info($"Woof! Scanning \"{fileName}\"...");
                Run(source);
            }

            if (ErrorOccured)
            {
                log.Error($"Encountered one or more errors while parsing \"{fileName}\"!");
                Environment.Exit(65);
            }
        }

        private static void RunPrompt()
        {
            PrintIntro();

            while (true)
            {
                ErrorOccured = false;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n> ");
                Console.ResetColor();
                Run(Console.ReadLine());
            }
        }

        private static void Run(string expression)
        {
            Scanner scanner = new Scanner(expression);
            List<Token> tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                log.Debug(token.ToString());
            }
        }

        public static void Error(int lineNumber, string message)
        {
            log.Error($"[line {lineNumber}] - {message}");
            ErrorOccured = true;
        }
    }
}
