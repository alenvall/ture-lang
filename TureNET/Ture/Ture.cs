using System;
using System.Collections.Generic;
using System.IO;
using Ture.Core;
using Ture.Models;

namespace Ture
{
    class Ture
    {
        private const string VER = "0.0.1";
        private static readonly Logger log = new Logger();
        private static readonly Interpreter interpreter = new Interpreter();

        private static bool ErrorOccured = false;
        private static bool RuntimeErrorOccured = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                log.Info("Woof! Usage: ture [script.tr]");
                System.Environment.Exit(64);
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
                log.Info($"Woof! Running \"{fileName}\"...\n");
                Run(source);
            }

            if (ErrorOccured)
            {
                log.Error($"Encountered one or more errors while parsing \"{fileName}\"!");
                System.Environment.Exit(65);
            }

            if (RuntimeErrorOccured)
            {
                log.Error($"Runtime error occured!");
                System.Environment.Exit(70);
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

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);

            var statements = parser.Parse();

            if (ErrorOccured)
            {
                return;
            }

            log.Info(interpreter.Interpret(statements));
        }

        public static void Report(int lineNumber, string where, string message)
        {
            log.Error($"[line {lineNumber}] - {message} {where}");
            ErrorOccured = true;
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.LineNumber, "at end", message);
            }
            else
            {
                Report(token.LineNumber, "at \"" + token.Lexeme + "\"", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            log.Error($"[line {error.Token.LineNumber}] {error.Message}");
            RuntimeErrorOccured = true;
        }

        public static void Print(string text)
        {
            log.Info(text);
        }
    }
}
