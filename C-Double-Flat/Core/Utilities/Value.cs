namespace C_Double_Flat.Core.Utilities
{
    public struct Value
    {
        public ValueType Type;
        public string Data;

        public Value(LiteralNode node)
        {
            this.Type = node.Value.Type.ToValueType();
            this.Data = node.Value.Data;
        }

        public Value(ValueType type, string data)
        {
            this.Type = type;
            this.Data = data;
        }

        public override string ToString()
        {
            if (Type == ValueType.String)
                return $"\"{Data}\"";
            return $"{Data}";
        }
    }

    public enum ValueType
    {
        Number,
        String,
        Boolean,
    }

    public static class ValueFacts
    {
        public static ValueType ToValueType(this TokenType type)
        {
            return type switch
            {
                TokenType.Number => ValueType.Number,
                TokenType.Boolean => ValueType.Boolean,
                TokenType.String => ValueType.String,
                _ => ValueType.Number,
            };
        }
    }
}
