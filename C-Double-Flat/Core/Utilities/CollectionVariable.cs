using System.Collections.Generic;
using System.Linq;
using C_Double_Flat.Core.Runtime;
namespace C_Double_Flat.Core.Utilities
{
    public struct CollectionVariable : IVariable
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

        /// <summary>
        /// Add an element to the end of the collection
        /// </summary>
        /// <param name="variable"></param>
        public void AddMember(IVariable variable)
        {
            Variables = Variables.Concat(new IVariable[] { variable }).ToArray();
        }

        /// <summary>
        /// Removes element at location in collection
        /// </summary>
        /// <param name="location"></param>
        public void RemoveMemberAt(int location)
        {
            if (location > Variables.Length || location - 1 < 0) return;
            var list = new List<IVariable>(Variables);
            list.RemoveAt(location - 1);
            Variables = list.ToArray();
        }

        public void RemoveMember(IVariable variable)
        {
            for (int i = 0; i < Variables.Length; i++)
                if (Interpreter.Equals(variable, Variables[i]).AsBool())
                {
                    RemoveMemberAt(i);
                    break;
                }

        }

        /// <summary>
        /// Access an element from a collection safely given the location (starting at 1).
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public IVariable AccessMember(int location)
        {
            if (location > Variables.Length || location - 1 < 0) return ValueVariable.Default;
            return Variables[location - 1];
        }

        /// <summary>
        /// Set element in a collection safely
        /// If position is out of bounds of array, it will add default values until it is
        /// </summary>
        /// <param name="location"></param>
        /// <param name="variable"></param>
        public void SetMember(int location, IVariable variable)
        {
            while (location > Variables.Length)
                Variables = Variables.Concat(new IVariable[] { ValueVariable.Default }).ToArray();
            Variables[location - 1] = variable;
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
    }
}
