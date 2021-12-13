using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Utilities;
using C_Double_Flat.Core.Runtime;
using System.Reflection;
namespace C_Double_Flat
{
    public class Program
    {
        public static string Location => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static void Main(string[] args)
        {
            LoadLibraries();
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    var statements = Parser.Parse(Lexer.Tokenize(File.ReadAllText(args[0])));

                    var output = Interpreter.Interpret(statements, File.Exists(args[0]) ? Path.GetDirectoryName(args[0]) : Location);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(output.Item2 ? output.Item1.ToString() : "");
                    Console.ResetColor();
                    Console.ReadKey(false);
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("C Double Flat - REPL v2 Indev");
                Console.WriteLine("Created by Heerod Sahraei");
                Console.WriteLine("Copyright (C) Hababisoft Corporation. All rights reserved.");
            }

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
                    var statements = Parser.Parse(tokens);

                    var output = Interpreter.Interpret(statements, File.Exists(input) ? Path.GetDirectoryName(input) : Location);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(output.Item2 ? output.Item1.ToString() : "");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }

            }
        }
        private static void LoadLibraries()
        {
            string librariesPath = Path.Combine(Location, @"lib\");
            if (!Directory.Exists(librariesPath)) Directory.CreateDirectory(librariesPath);

            var files = Directory.GetFiles(librariesPath);

            foreach (var file in files)
            {
                var errors = AddFunctionsFromPath(file);
                if (errors != "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"An error occured while loading library '{file}':");
                    Console.WriteLine(errors);
                    Console.ResetColor();
                }
            }
        }
        private static string AddFunctionsFromPath(string libraryPath)
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
