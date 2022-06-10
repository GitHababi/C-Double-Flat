using System;
using System.IO;
using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Utilities;
using C_Double_Flat.Core.Runtime;
using C_Double_Flat.StandardLibrary;
namespace C_Double_Flat.App
{
    internal class Program
    {
        public static readonly string Location = AppDomain.CurrentDomain.BaseDirectory;
        public static void Main(string[] args)
        {
            Interpreter.LoadLibrary(new Library());
            LoadLibraries();
            Console.Title = "C Double Flat";
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                // TODO: Localization! (Cause why not)
                Console.WriteLine("C Double Flat - REPL 2.4.5");
                Console.WriteLine("Created by Heerod Sahraei");
                Console.WriteLine("Copyleft Hababisoft Corporation. All rights unreserved.");
                REPL();
            }

            try
            {
                var statements = Parser.Parse(Lexer.Tokenize(File.ReadAllText(args[0])));

                var output = Interpreter.Interpret(statements, File.Exists(args[0]) ? Path.GetDirectoryName(args[0]) : Location);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (statements.Type == StatementType.Expression || output.Item2)
                    Console.WriteLine(output.Item1.ToString());
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
            }
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
                    var output = Interpreter.Interpret(statements, File.Exists(input) ? Path.GetDirectoryName(input) : Location);
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
            string librariesPath = Path.Combine(Location, @"lib\");
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
