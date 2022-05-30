using C_Double_Flat.Core.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace C_Double_Flat.Core.Utilities
{
    public struct CollectionVariable : IVariable, IEnumerable<IVariable>
    {
        public IVariable[] Variables;
        public VariableType Type() => VariableType.Collection;
        public CollectionVariable(IVariable[] variables)
        {
            this.Variables = variables;
        }

        public CollectionVariable(IVariable variable)
        {
            this.Variables = new IVariable[] { variable };
        }


        public CollectionVariable(List<IVariable> variables)
        {
            this.Variables = variables.ToArray();
        }

        public double AsDouble()
        {
            return this.Variables.Length;
        }

        public bool AsBool()
        {
            return (this.Variables.Length > 0);
        }

        public string AsString()
        {
            return "";
        }

        public override string ToString()
        {
            string variables = "";
            foreach (var variable in Variables)
                variables += variable.ToString();
            return $"[{variables}]";
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return new VariableEnumerator(Variables);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
