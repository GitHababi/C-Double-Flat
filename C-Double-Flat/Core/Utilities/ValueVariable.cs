using System;
namespace C_Double_Flat.Core.Utilities
{
    public struct ValueVariable : IVariable
    {
        public readonly Value Value;

        public VariableType Type() => VariableType.Value;

        public static readonly ValueVariable Default = new(0);

        public ValueVariable(ValueVariable variable)
        {
            Value = variable.Value;
        }

        public ValueVariable(bool value)
        {
            this.Value = new Value(ValueType.Boolean, value.ToString().ToLower());
        }

        public ValueVariable(double value)
        {
            this.Value = new Value(ValueType.Number, value.ToString());
        }

        public ValueVariable(string value)
        {
            this.Value = new Value(ValueType.String, value);
        }

        public ValueVariable(Value value)
        {
            this.Value = value;
        }

        public ValueVariable(LiteralNode Literal)
        {
            this.Value = new Value() { Data = Literal.Value.Data, Type = Literal.Value.Type.ToValueType() };
        }

        public bool AsBool()
        {
            return (Value.Data == "true") || (AsDouble() == 1);
        }

        public double AsDouble()
        {
            if (Value.Type == ValueType.Boolean) return (Value.Data == "true") ? 1 : 0;
            try
            {
                return Convert.ToDouble(Value.Data);
            }
            catch
            {
                return 0;
            }
        }
        public string AsString()
        {
            return Value.Data ?? "";
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

        public ValueVariable CastTo(ValueType value)
        {
            if (value == Value.Type) return this;
            return value switch
            {
                ValueType.Number => new(AsDouble()),
                ValueType.Boolean => new(AsBool()),
                ValueType.String => new(Value.Data),
                _ => throw new ArgumentException($"Casting value was not expected to be {this.Value}"),
            };
        }

        public override string ToString()
        {
            return $"{this.Value}";
        }
    }
}
