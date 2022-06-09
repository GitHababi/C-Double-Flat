using C_Double_Flat.Core.Utilities;
using C_Double_Flat.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace StandardLibrary
{
    public class Library : ILoadable
    {
        public List<CustomFunction> GetFunctions()
        {
            return new()
            {
                new("disp_out", p =>
                {
                    p.ForEach(i => Console.Write(i.AsString()));
                    return ValueVariable.Default;
                }),

                new("disp_in", p =>
                {
                    return new ValueVariable(Console.ReadKey().ToString());
                }),

                new("disp_echo", p =>
                {
                    p.ForEach(i => Console.Write(i.AsString()));
                    Console.WriteLine();
                    return ValueVariable.Default;
                }),

                new("disp_prompt", p =>
                {
                    return new ValueVariable(Console.ReadLine());
                }),

                new("disp_clear", p =>
                {
                    Console.Clear();
                    return ValueVariable.Default;
                }),

                new("disp_title", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    Console.Title = p[0].AsString();
                    return ValueVariable.Default;
                }),

                new("disp_color", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    Console.ForegroundColor = p[0].AsString().ToLower().Trim() switch
                    {

                        "black" => ConsoleColor.Black,
                        "darkblue" => ConsoleColor.DarkBlue,
                        "darkgreen" => ConsoleColor.DarkGreen,
                        "darkcyan" => ConsoleColor.DarkCyan,
                        "darkred" => ConsoleColor.DarkRed,
                        "darkmagenta" => ConsoleColor.DarkMagenta,
                        "darkyellow" => ConsoleColor.DarkYellow,
                        "darkgray" => ConsoleColor.DarkGray,
                        "darkgrey" => ConsoleColor.DarkGray,
                        "gray" => ConsoleColor.Gray,
                        "grey" => ConsoleColor.Gray,
                        "green" => ConsoleColor.Green,
                        "blue" => ConsoleColor.Blue,
                        "yellow" => ConsoleColor.Yellow,
                        "red" => ConsoleColor.Red,
                        "magenta" => ConsoleColor.Magenta,
                        "white" => ConsoleColor.White,
                        _ => Console.ForegroundColor
                    };
                    return ValueVariable.Default;
                }),


                #region Math

                new("math_abs", p =>
               {
                   if (p.Count < 1)
                       return ValueVariable.Default;
                   return new ValueVariable(Math.Abs(p[0].AsDouble()));
               }),

                new("math_round", p =>
               {
                   if (p.Count < 1)
                       return ValueVariable.Default;
                   return new ValueVariable(Math.Round(p[0].AsDouble()));
               }),

                new("math_ceil", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Ceiling(p[0].AsDouble()));
                }),

                new("math_floor", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Floor(p[0].AsDouble()));
                }),

                new("math_pow", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Pow(p[0].AsDouble(), p[1].AsDouble()));
                }),

                new("math_mod", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    return new ValueVariable(p[0].AsDouble() % p[1].AsDouble());
                }),

                new("math_sqrt", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    var result = Math.Sqrt(p[0].AsDouble());
                    return new ValueVariable((double.IsNaN(result)) ? 0 : result);
                }),

                new("math_rand", p =>
                {
                    var random = new Random();
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    if (p.Count < 2)
                        return new ValueVariable(random.Next((int)p[0].AsDouble() - 1) + 1);
                    return new ValueVariable(random.Next((int)p[0].AsDouble(), (int)p[1].AsDouble() + 1));
                }),

                new("math_sin", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Sin(p[0].AsDouble()));
                }),

                new("math_cos", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Cos(p[0].AsDouble()));
                }),

                new("math_tan", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Tan(p[0].AsDouble()));
                }),

                new("math_log", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    if (p.Count < 2)
                        return new ValueVariable(Math.Log(p[0].AsDouble()));
                    return new ValueVariable(Math.Log(p[0].AsDouble(), p[1].AsDouble()));

                }),

                #endregion

                #region Collection

                new("col_create", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    IVariable toFill = ValueVariable.Default;
                    if (p.Count > 1)
                        toFill = p[1];
                    IVariable[] output = Enumerable.Repeat(toFill, (int)(p[0].AsDouble())).ToArray();
                    return new CollectionVariable(output);
                }),

                new("col_add", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.Variables.Add(p[1]);
                    return collection;
                }),

                new("col_remove", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];

                    collection.Variables.Remove(p[1]);

                    return collection;
                }),

                new("col_remove_at", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var index = (int)p[1].AsDouble() - 1;
                    var collection = (CollectionVariable)p[0];
                    collection.Variables.RemoveAt((int)(p[1].AsDouble() - 1));
                    return collection;
                }),

                new("col_loc_of", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    if (p.Count < 2)
                        return p[0];


                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var list = ((CollectionVariable)p[0]).Variables.ToList();

                    return new ValueVariable(list.IndexOf(p[1]) + 1);
                }),

                #endregion

                #region String

                new("str_substr", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    if (p.Count < 3)
                        return new ValueVariable(p[0].AsString()[(int)(p[1].AsDouble() - 1)..]);
                    return new ValueVariable(p[0].AsString()[(int)(p[1].AsDouble() - 1)..(int)(p[2].AsDouble() - 1)]);
                }),

                new("str_size", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(p[0].AsString().Length);
                }),

                new("str_has", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    return new ValueVariable(p[0].AsString().Contains(p[1].AsString()));
                }),

                new("str_split", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    List<IVariable> list = new();
                    p[0].AsString().Split(p[1].AsString()).ToList().ForEach(x => list.Add(new ValueVariable(x)));

                    return new CollectionVariable(list);

                }),

                new("str_to_col", p =>
                {

                    if (p.Count < 1)
                        return ValueVariable.Default;

                    List<IVariable> list = new();
                    p[0].AsString().ToCharArray().ToList().ForEach(x => list.Add(new ValueVariable(x.ToString())));

                    return new CollectionVariable(list);

                }),

                new("str_to_upper", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsString().ToUpper());
                }),

                new("str_to_lower", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsString().ToLower());
                }),

                new("str_trim", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsString().Trim());

                }),

                #endregion

                #region Misc

                new("to_num", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsDouble());
                }),

                new("to_str", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsString());
                }),

                new("to_bool", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(p[0].AsBool());
                }),

                new("exit", p =>
                {
                    Environment.Exit(0);
                    return ValueVariable.Default;
                }),
                #endregion

                #region IO

                new("cbb_loc", p => new ValueVariable(Environment.CurrentDirectory)),

                new("path_comb", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(Path.Combine(p.Select(p => p.AsString()).ToArray()));
                }),

                new("folder_exists", p =>
                {
                    if (p.Count < 1)
                        return new ValueVariable(false);

                    return new ValueVariable(Directory.Exists(p[0].AsString()));
                }),

                new("folder_files", p =>
                {
                    if (p.Count < 1)
                        return new ValueVariable(false);

                    string[] files = Directory.GetFiles(p[0].AsString());
                    var filesAsVars = files.Select(p => new ValueVariable(p)).Cast<IVariable>();
                    return new CollectionVariable(filesAsVars.ToArray());
                }),

                new("folder_delete", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    Directory.Delete(p[0].AsString(), true);

                    return ValueVariable.Default;
                }),

                new("folder_create", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    Directory.CreateDirectory(p[0].AsString());

                    return ValueVariable.Default;
                }),

                new("file_append", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    File.AppendAllText(p[0].AsString(), p[1].AsString());

                    return ValueVariable.Default;
                }),

                new("file_write", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    File.WriteAllText(p[0].AsString(), p[1].AsString());

                    return ValueVariable.Default;
                }),

                new("file_delete", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    File.Delete(p[0].AsString());

                    return ValueVariable.Default;
                }),

                new("file_copy", p =>
               {
                   if (p.Count < 2)
                       return ValueVariable.Default;

                   File.Copy(p[0].AsString(), p[1].AsString());

                   return ValueVariable.Default;
               }),

                new("file_exists", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(File.Exists(p[0].AsString()));
                }),

                new("file_read", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    return new ValueVariable(File.ReadAllText(p[0].AsString()));
                }),

                new("file_read_lines", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    string[] lineStrings = File.ReadAllLines(p[0].AsString());
                    try
                    {
                        File.ReadAllText(p[0].AsString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    IVariable[] lines = new IVariable[lineStrings.Length];
                    for (int i = 0; i < lineStrings.Length; i++)
                        lines[i] = new ValueVariable(lineStrings[i]);

                    return new CollectionVariable(lines);
                }),

                new("file_folder", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;

                    var location = Path.GetDirectoryName(p[0].AsString());

                    return new ValueVariable(location);
                })
                
                #endregion
            };
        }
    }
}
