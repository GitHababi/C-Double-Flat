using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Runtime;
using C_Double_Flat.Core.Utilities;
using C_Double_Flat.StandardLibrary;
using System;
using System.IO;

namespace C_Double_Flat.App
{
    internal class Program
    {
        public static readonly string InstallLocation = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string StartLocation = Environment.CurrentDirectory;
        public static void Main(string[] args)
        {
            Interpreter.LoadLibrary(new Library());
            LoadLibraries();
            Console.Title = "C Double Flat";
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                // TODO: Localization! (Cause why not)
                Console.WriteLine("C Double Flat - 3.0.1");
                Console.WriteLine("Created by Heerod Sahraei");
                Console.WriteLine("Copyleft Hababisoft Corporation. All rights unreserved.");
                REPL();
            }

            try
            {
                var fullPath = Path.GetFullPath(args[0]);
                var statements = Parser.Parse(Lexer.Tokenize(File.ReadAllText(fullPath)));
                var output = Interpreter.Interpret(statements, File.Exists(fullPath) ? Path.GetDirectoryName(fullPath) : StartLocation);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (statements.Type == StatementType.Expression || output.Item2)
                    Console.WriteLine(output.Item1.ToString());
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
            }
            Console.ResetColor();
        }

        private static void REPL()
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(">> ");
                    Console.ResetColor();
                    string input = Console.ReadLine();
                    Token[] tokens;
                    if (File.Exists(input))
                        tokens = Lexer.Tokenize(File.ReadAllText(input));
                    else
                        tokens = Lexer.Tokenize(input);
                    var statements = Parser.Parse(tokens);
                    var output = Interpreter.Interpret(statements, StartLocation);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    if (statements.Type == StatementType.Expression || output.Item2)
                        Console.WriteLine(output.Item1.ToString());
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }

        private static void LoadLibraries()
        {
            Console.Title = "Loading...";
            string librariesPath = Path.Combine(InstallLocation, @"lib\");
            if (!Directory.Exists(librariesPath)) Directory.CreateDirectory(librariesPath);

            var files = Directory.GetFiles(librariesPath);

            foreach (var file in files)
            {
                if (file.EndsWith(".dll"))
                {
                    try
                    {
                        Interpreter.LoadLibraryFromPath(file);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"An error occured while loading library '{Path.GetFileName(file)}':");
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                    }

                }
            }
        }
    }
}
