using System;
using C_Double_Flat.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace StandardLibrary
{
    public class Library : ILoadable
    {
        public List<CustomFunction> GetFunctions()
        {
            return new()
            {
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

                new("disp_color", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    ConsoleColor color = Console.ForegroundColor;

                    switch (p[0].AsString().ToLower().Trim())
                    {
                        case "black":
                            color = ConsoleColor.Black;
                            break;
                        case "darkblue":
                            color = ConsoleColor.DarkBlue;
                            break;
                        case "darkgreen":
                            color = ConsoleColor.DarkGreen;
                            break;
                        case "darkcyan":
                            color = ConsoleColor.DarkCyan;
                            break;
                        case "darkred":
                            color = ConsoleColor.DarkRed;
                            break;
                        case "darkmagenta":
                            color = ConsoleColor.DarkMagenta;
                            break;
                        case "darkyellow":
                            color = ConsoleColor.DarkYellow;
                            break;
                        case "green":
                            color = ConsoleColor.Green;
                            break;
                        case "blue":
                            color = ConsoleColor.Blue;
                            break;
                        case "yellow":
                            color = ConsoleColor.Yellow;
                            break;
                        case "red":
                            color = ConsoleColor.Red;
                            break;
                        case "magenta":
                            color = ConsoleColor.Magenta;
                            break;
                        case "white":
                            color = ConsoleColor.White;
                            break;
                    }

                    Console.ForegroundColor = color;
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
                    {
                        return new ValueVariable(random.Next((int)p[0].AsDouble() - 1) + 1);
                    }
                    return new ValueVariable(random.Next((int)p[0].AsDouble(), (int)p[1].AsDouble() + 1));
                }),

                #endregion

                #region Collection

                new("col_add", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.AddMember(p[1]);
                    return collection;
                }),

                new("col_remove", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.RemoveMember(p[1]);
                    return collection;
                }),

                new("col_remove_at", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type() != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.RemoveMemberAt((int)p[1].AsDouble());
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
                #endregion
            };
        }
    }
}
