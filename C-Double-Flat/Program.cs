using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Runtime;
using C_Double_Flat.Core.Utilities;
using System;
using System.IO;
using System.Reflection;
namespace C_Double_Flat
{
    public class Program
    {
        public static readonly string Location = AppDomain.CurrentDomain.BaseDirectory;
        public static void Main(string[] args)
        {
            LoadLibraries();
            Console.Title = "C Double Flat";
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    try
                    {
                        var statements = Parser.Parse(Lexer.Tokenize(File.ReadAllText(args[0])));

                        var output = Interpreter.Interpret(statements, File.Exists(args[0]) ? Path.GetDirectoryName(args[0]) : Location);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine(output.Item2 ? output.Item1.ToString() : "");
                        Environment.Exit(0);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine(e.Message);
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                Console.WriteLine("C Double Flat - REPL 2.2.0");
                Console.WriteLine("Created by Heerod Sahraei");
                Console.WriteLine("Copyleft Hababisoft Corporation. All rights unreserved.");
            }

            REPL();
        }

        private static void REPL()
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(">");
                    Console.ResetColor();
                    string input = Console.ReadLine();
                    Token[] tokens;
                    if (File.Exists(input))
                        tokens = Lexer.Tokenize(File.ReadAllText(input));
                    else
                        tokens = Lexer.Tokenize(input);
                    // tokens.ToList().ForEach(x => Console.WriteLine(x));
                    var statements = Parser.Parse(tokens);
                    // Console.WriteLine(statements);
                    var output = Interpreter.Interpret(statements, File.Exists(input) ? Path.GetDirectoryName(input) : Location);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(output.Item2 ? output.Item1.ToString() : "");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(e.Message);
                    // Console.Error.WriteLine(e.StackTrace);
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
                    var errors = AddFunctionsFromPath(file);
                    if (errors != "")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"An error occured while loading library '{Path.GetFileName(file)}':");
                        Console.WriteLine(errors);
                        Console.ResetColor();
                    }

                }
            }
        }
        public static string AddFunctionsFromPath(string libraryPath)
        {

            try
            {
                var lib = Assembly.LoadFile(libraryPath);
                var types = lib.GetTypes();
                foreach (var type in types)
                {
                    if (typeof(ILoadable).IsAssignableFrom(type))
                        Interpreter.SetFunction(((ILoadable)Activator.CreateInstance(type)).GetFunctions());
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "";
        }
    }
}
