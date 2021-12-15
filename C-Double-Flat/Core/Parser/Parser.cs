using C_Double_Flat.Core.Utilities;
using System.Collections.Generic;

namespace C_Double_Flat.Core.Parser
{
    public partial class Parser
    {
        private Parser(Token[] tokens, bool isNested)
        {
            this.Tokens = tokens;
            this.IsNested = isNested;
            this.Index = 0;
        }

        private readonly Token[] Tokens;
        private Token CurrentToken => Peek(0);
        private readonly bool IsNested;
        private int Index;
        public static Statement Parse(Token[] tokens, bool isNested = false)
        {
            return new Parser(tokens, isNested).Parse();
        }

        private Statement Parse()
        {

            List<Statement> output = new();

            while (CurrentToken.Type != TokenType.EndOfFile)
            {
                var current = CurrentToken;
                output.Add(ParseStatement());
                if (current.Equals(CurrentToken)) break;
            }
            return new StatementBlock(output);
        }

        private Token Peek(int amount = 1)
        {
            return (Index + amount < Tokens.Length) ? Tokens[Index + amount] : Tokens[^1];
        }

        private Token Next()
        {
            Token current = CurrentToken;
            Index++;
            return current;
        }

        private Token ExpectThenNext(TokenType type)
        {
            if (CurrentToken.Type == type) return Next();

            throw new ExpectedTokenException(CurrentToken.Position, CurrentToken.Type, type);

        }

    }
}
