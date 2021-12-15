using System.Collections.Generic;

namespace C_Double_Flat.Core.Utilities
{
    public interface IFunction
    {
        public string Name { get; }
        public IVariable Run(List<IVariable> variables);
    }
}
