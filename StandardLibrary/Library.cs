using System;
using C_Double_Flat.Core.Utilities;
using System.Collections.Generic;

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
                    p.ForEach(i => Console.Write((string)i));
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


                #region Math

                new("math_abs", p =>
                        {
                            if (p.Count < 1)
                                return ValueVariable.Default;
                            return new ValueVariable(Math.Abs(p[0]));
                        }),

                new("math_round", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Round(p[0]));
                }),

                new("math_pow", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    return new ValueVariable(Math.Pow(p[0], p[1]));
                }),

                new("math_mod", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;
                    return new ValueVariable(p[0] % p[1]);
                }),

                new("math_sqrt", p =>
                {
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    var result = Math.Sqrt(p[0]);
                    return new ValueVariable(result == double.NaN ? ValueVariable.Default : result);
                }),

                new("math_rand", p =>
                {
                    var random = new Random();
                    if (p.Count < 1)
                        return ValueVariable.Default;
                    if (p.Count < 2)
                    {
                        return new ValueVariable(random.Next((int)(double)p[0] - 1) + 1);
                    }
                    return new ValueVariable(random.Next((int)(double)p[0], (int)(double)p[1] + 1));
                }),

                #endregion

                #region Collection

                new("col_add", p => 
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.AddMember(p[1]);
                    return collection;
                }),

                new("col_remove", p =>
                {
                    if (p.Count < 2)
                        return ValueVariable.Default;

                    if (p[0].Type != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.RemoveMember(p[1]);
                    return collection;
                }),

                new("col_remove_at", p =>
                {
                    if (p.Count < 2) 
                        return ValueVariable.Default;

                    if (p[0].Type != VariableType.Collection)
                        return p[0];

                    var collection = (CollectionVariable)p[0];
                    collection.RemoveMemberAt((int)(double)p[1]);
                    return collection;
                }),

                #endregion
            };
        }
    }
}
