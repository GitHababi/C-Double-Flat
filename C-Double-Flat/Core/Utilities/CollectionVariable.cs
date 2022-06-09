using C_Double_Flat.Core.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace C_Double_Flat.Core.Utilities
{
    public class CollectionVariable : IVariable, IEnumerable<IVariable>
    {
        public List<IVariable> Variables;
        public VariableType Type() => VariableType.Collection;
        
        public CollectionVariable()
        {
            this.Variables = new();
        }
        
        public CollectionVariable(IVariable[] variables)
        {
            this.Variables = variables.ToList();
        }

        public CollectionVariable(IVariable variable)
        {
            this.Variables = new List<IVariable>() { variable };
        }


        public CollectionVariable(List<IVariable> variables)
        {
            this.Variables = variables;
        }

        public CollectionVariable ExtendTo(int length)
        {
            while (Variables.Count < length)
            {
                Variables.Add(ValueVariable.Default);
            }
            return this;
        }

        public double AsDouble()
        {
            return this.Variables.Count;
        }

        public bool AsBool()
        {
            return (this.Variables.Count > 0);
        }

        public string AsString()
        {
            return "";
        }

        public override string ToString()
        {
            if (Variables.Count == 0)
            {
                return "[]";
            }
            else
            {
                return $"[{string.Join(", ", Variables.Select(x => x.ToString()))}]";
            }
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return new VariableEnumerator(Variables.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
