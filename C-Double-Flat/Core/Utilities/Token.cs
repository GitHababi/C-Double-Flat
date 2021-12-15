
namespace C_Double_Flat.Core.Utilities
{
    public struct Token
    {
        public Position Position;
        public string Data;
        public TokenType Type;

        public Token(Position position, TokenType tokenType, string data)
        {
            this.Position = position;
            this.Type = tokenType;
            this.Data = data;
        }

        public Token(Position positon, TokenType tokenType)
        {
            this.Position = positon;
            this.Type = tokenType;
            this.Data = "";
        }

        public override string ToString()
        {
            return $"{{{Position.Row}}}{{{Position.Column}}} {Type} : [{Data}]";
        }

    }

    public static class TokenFacts
    {
        public static int GetPrecedence(this Token tok)
        {
            return tok.Type switch
            {
                TokenType.Multiply or TokenType.Divide => 4,

                TokenType.Plus or TokenType.Minus => 3,

                TokenType.Equal or
                TokenType.NotEqual or
                TokenType.LessThan or
                TokenType.LessThanOrEqual or
                TokenType.GreaterThan or
                TokenType.GreaterThanOrEqual => 2,

                TokenType.And or TokenType.Or => 1,
                _ => 0,
            };
        }
    }
}
