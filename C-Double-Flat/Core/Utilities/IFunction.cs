using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Double_Flat.Core.Utilities
{
    public interface IFunction
    {
        public string Name { get; }
        public IVariable Run(List<IVariable> variables);
    }
}
