using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Variable
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
    }
}
