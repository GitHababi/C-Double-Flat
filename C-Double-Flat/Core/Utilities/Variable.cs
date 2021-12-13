using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Runtime;
namespace C_Double_Flat.Core.Utilities
{

    public enum VariableType
    {
        Value,
        Collection
    }

    public abstract class Variable
    {
        public VariableType Type;

        public static explicit operator bool(Variable var)
        {
            if (var.Type == VariableType.Value)
                return (bool)(ValueVariable)var;
            return (bool)(CollectionVariable)var;
        }
        public static implicit operator double(Variable var)
        {
            if (var.Type == VariableType.Value)
                return (double)(ValueVariable)var;
            return (double)(CollectionVariable)var;
        }
        public static implicit operator string(Variable var)
        {
            if (var.Type == VariableType.Value)
                return (string)(ValueVariable)var;
            return (string)(CollectionVariable)var;
        }
        public override string ToString()
        {
            return this.Type == VariableType.Value ? ((ValueVariable)this).ToString() : ((CollectionVariable)this).ToString();
        }
    }


    public sealed class ValueVariable : Variable
    {
        public Value Value;

        public static readonly ValueVariable Default = new(0);

        public ValueVariable(bool value)
        {
            this.Type = VariableType.Value;
            this.Value = new Value(ValueType.Boolean, value.ToString().ToLower());
        }

        public ValueVariable(double value)
        {
            this.Type = VariableType.Value;
            this.Value = new Value(ValueType.Number, value.ToString());
        }

        public ValueVariable(string value)
        {
            this.Type = VariableType.Value;
            this.Value = new Value(ValueType.String, value);
        }

        public ValueVariable(Value value)
        {
            this.Type = VariableType.Value;
            this.Value = value;
        }

        public ValueVariable(LiteralNode Literal)
        {
            this.Type = VariableType.Value;
            this.Value = new Value() { Data = Literal.Value.Data, Type = Literal.Value.Type.ToValueType() };
        }

        public static explicit operator bool(ValueVariable var)
        {
            return (var.Value.Data == "true") || ((double)var == 1);
        }

        public static explicit operator double(ValueVariable var)
        {
            if (var.Value.Type == ValueType.Boolean) return (var.Value.Data == "true") ? 1 : 0;
            try
            {
                return Convert.ToDouble(var.Value.Data);
            }
            catch
            {
                return 0;
            }
        }
        public static explicit operator string(ValueVariable var)
        {
            return var.Value.Data ?? "";
        }


        private static readonly ValueType[,] resolvingAddingValues =
        {
            { ValueType.Number, ValueType.String, ValueType.Number },
            { ValueType.String, ValueType.String, ValueType.String },
            { ValueType.Number, ValueType.String, ValueType.Number }
        };
        public static (ValueVariable, ValueVariable) ResolveAddingTypes(ValueVariable var1, ValueVariable var2)
        {
            var castTo = resolvingAddingValues[(int)var1.Value.Type, (int)var2.Value.Type];
            return (var1.CastTo(castTo), var2.CastTo(castTo));
        }

        private static readonly ValueType[,] resolvingComparingValues =
        {
            { ValueType.Number, ValueType.String, ValueType.Number },
            { ValueType.String, ValueType.String, ValueType.String },
            { ValueType.Number, ValueType.String, ValueType.Boolean}
        };
        public static (ValueVariable, ValueVariable) ResolveComparingTypes(ValueVariable var1, ValueVariable var2)
        {
            var castTo = resolvingComparingValues[(int)var1.Value.Type, (int)var2.Value.Type];
            return (var1.CastTo(castTo), var2.CastTo(castTo));
        }

        public ValueVariable CastTo(ValueType value)
        {
            if (value == this.Value.Type) return this;
            switch (value)
            {
                case ValueType.Number:
                    return this.Value.Type switch
                    {
                        ValueType.String => new(new Value(value, ((double)this).ToString())),
                        ValueType.Boolean => new(new Value(value, ((double)this).ToString())),
                        _ => this,
                    };
                case ValueType.Boolean:
                    return this.Value.Type switch
                    {
                        ValueType.Number => new(new Value(value, ((bool)this).ToString())),
                        ValueType.String => new(new Value(value, ((bool)this).ToString())),
                        _ => this,
                    };
                case ValueType.String:
                    return new(new Value(value, this.Value.Data));
                default:
                    throw new ArgumentException($"Casting value was not expected to be {this.Value}");
            }
        }

        public override string ToString()
        {
            return $"{this.Value}";
        }
    }

    public sealed class CollectionVariable : Variable
    {
        public List<Variable> Variables;

        public CollectionVariable(Variable[] variables)
        {
            this.Type = VariableType.Collection;
            this.Variables = variables.ToList();
        }

        public CollectionVariable(Variable variable)
        {
            this.Type = VariableType.Collection;
            this.Variables = new();
            this.Variables.Add(variable);
        }

        /// <summary>
        /// Add an element to the end of the collection
        /// </summary>
        /// <param name="variable"></param>
        public void AddMember(Variable variable)
        {
            Variables.Add(variable);
        }

        /// <summary>
        /// Removes element at location in collection
        /// </summary>
        /// <param name="location"></param>
        public void RemoveMemberAt(int location)
        {
            if (location > Variables.Count || location - 1 < 0) return;
            Variables.RemoveAt(location - 1);
        }

        public void RemoveMember(Variable variable)
        {
            for (int i = 0; i < Variables.Count; i++)
                if ((bool)Interpreter.Equals(variable, Variables[i]))
                {
                    Variables.RemoveAt(i);
                    break;
                }

        }

        /// <summary>
        /// Access an element from a collection safely given the location (starting at 1).
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Variable AccessMember(int location)
        {
            if (location > Variables.Count || location - 1 < 0) return ValueVariable.Default;
            return Variables[location - 1];
        }

        /// <summary>
        /// Set element in a collection safely
        /// If position is out of bounds of array, it will add default values until it is
        /// </summary>
        /// <param name="location"></param>
        /// <param name="variable"></param>
        public void SetMember(int location, Variable variable)
        {
            while (location > Variables.Count)
                Variables.Add(ValueVariable.Default);
            Variables[location - 1] = variable;
        }

        public CollectionVariable(List<Variable> variables)
        {
            this.Type = VariableType.Collection;
            this.Variables = variables;
        }

        public static explicit operator double(CollectionVariable var)
        {
            return var.Variables.Count;
        }

        public static explicit operator bool(CollectionVariable var)
        {
            return (var.Variables.Count > 0);
        }

        public static explicit operator string(CollectionVariable var)
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
