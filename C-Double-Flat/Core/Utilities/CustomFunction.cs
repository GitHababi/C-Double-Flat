﻿using System;
using System.Collections.Generic;

namespace C_Double_Flat.Core.Utilities
{
    public class CustomFunction : IFunction
    {
        public string Name { get; private set; }

        private readonly Func<List<IVariable>, IVariable> Function;
        public IVariable Run(List<IVariable> variables)
        {
            return Function?.Invoke(variables);
        }

        public CustomFunction(string name, Func<List<IVariable>, IVariable> Function)
        {
            this.Function = Function;
            this.Name = name;
        }

        public static CustomFunction Default = new("Default", v => ValueVariable.Default);
    }
}
