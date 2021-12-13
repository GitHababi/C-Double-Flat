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
        Insertion,
        SemiColon,

        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equal,
        NotEqual,

        Or,
        And,

        If,
        Else,
        Loop,
        Repeat,
        Return,
        Run,
        AsName,
        Global,
        Local,

        LeftParenthesis,
        RightParenthesis,
        LeftCurlyBracket,
        RightCurlyBracket,
        LeftSquareBracket,
        RightSquareBracket
    }
}
