using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Double_Flat.Core.Utilities
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(Position pos, char token)
        : base($"Invalid token: '{token}' at Line: {pos.Row} Column: {pos.Column}") { }
    }

    public class TerminatingStringException : Exception {
        public TerminatingStringException(Position pos) 
        : base($"Invalid string starting at Line: {pos.Row} Column: {pos.Column}") { }
    }

    public class NumberFormattingException : Exception
    {
        public NumberFormattingException(Position pos)
        : base($"Invalid number formatting at Line: {pos.Row} Column: {pos.Column}") { }
    }

    public class ExpectedTokenException: Exception
    {
        public ExpectedTokenException(Position pos, TokenType type)
        : base($"Unexpected token {type} at Line: {pos.Row} Column: {pos.Column}") { }

        public ExpectedTokenException(Position pos, TokenType type, TokenType expected)
        : base($"Expected token {expected} but got {type} at Line: {pos.Row} Column: {pos.Column}") { }
    }
}
