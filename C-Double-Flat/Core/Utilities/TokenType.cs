namespace C_Double_Flat.Core.Utilities
{
    public enum TokenType
    {
        EndOfFile,
        Number,
        Identifier,
        Boolean,
        String,

        Plus,
        Minus,
        Multiply,
        Divide,

        Comma,
        Assignment,
        SemiColon,

        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equal,
        NotEqual,

        Or,
        And,
        Not,

        If,
        Else,
        Loop,
        Repeat,
        Return,
        Run,
        AsName,
        Global,
        Local,
        Dispose,

        LeftParenthesis,
        RightParenthesis,
        LeftCurlyBracket,
        RightCurlyBracket,
        LeftSquareBracket,
        RightSquareBracket
    }
}
