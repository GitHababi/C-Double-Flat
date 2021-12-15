namespace C_Double_Flat.Core.Utilities
{
    /* Faster to implicitly convert than to check the type each type.*/
    public enum NodeType
    {
        BinaryOperation,
        Literal,
        CollectionLiteral,
        FunctionCall,
        CollectionCall,
        AsName,
        Variable,
        Not
    }

    public enum StatementType
    {
        Assignment,
        Function,
        Block,
        Return,
        Expression,
        If,
        Run,
        Loop,
        Repeat,
        Dispose
    }
}
