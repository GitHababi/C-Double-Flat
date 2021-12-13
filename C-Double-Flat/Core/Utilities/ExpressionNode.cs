using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Double_Flat.Core.Utilities
{
    public abstract class ExpressionNode
    {
        public Position Position;
        public NodeType Type;
    }

    public sealed class VariableNode : ExpressionNode
    {
        public Variable Value;


        public VariableNode(Variable value)
        {
            this.Value = value;
            Type = NodeType.Variable;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public sealed class BinaryOperationNode : ExpressionNode
    {
        public ExpressionNode Left;
        public Token Operation;
        public ExpressionNode Right;
        

        public BinaryOperationNode(ExpressionNode left, Token operation, ExpressionNode right)
        {
            this.Position = operation.Position;
            this.Left = left;
            this.Operation = operation;
            this.Right = right;
            Type = NodeType.BinaryOperation;
        }
        public override string ToString()
        {
            return $"[{Left},{Operation.Type},{Right}]";
        }
    }

    public sealed class LiteralNode : ExpressionNode
    {
        public Token Value;


        public LiteralNode(Token value)
        {
            this.Value = value;
            Type = NodeType.Literal;
        }

        public override string ToString()
        {
            return $"({Value.Type} : {Value.Data})";
        }
    }
    

    public sealed class CollectionLiteralNode : ExpressionNode
    {
        public List<ExpressionNode> Elements = new();


        public CollectionLiteralNode(Position position, List<ExpressionNode> elements)
        {
            this.Position = position;
            this.Elements = elements;
            Type = NodeType.CollectionLiteral;
        }

        public override string ToString()
        {
            string elementsAsString = "";
            Elements.ForEach(a => {
                elementsAsString += a + ",";
            });

            return $"(Collection : {elementsAsString})";
        }
    }

    public sealed class AsNameNode : ExpressionNode
    {
        public ExpressionNode Identifier;


        public AsNameNode(Position position,ExpressionNode identifier)
        {
            this.Position = position;
            this.Identifier = identifier;
            Type = NodeType.AsName;
        }

        public override string ToString()
        {
            return $"(AsName : {Identifier})";
        }
    }

    public sealed class FunctionCallNode : ExpressionNode
    {
        public ExpressionNode Caller;
        public List<ExpressionNode> Parameters = new();


        public FunctionCallNode(Position position, ExpressionNode caller, List<ExpressionNode> parameters)
        {
            this.Position = position;
            this.Caller = caller;
            this.Parameters = parameters;
            Type = NodeType.FunctionCall;
        }
        public override string ToString()
        {
            string parametersAsString = "";
            Parameters.ForEach(a =>
            {
                parametersAsString += a.ToString() + ",";
            });
            return $"(Caller : {Caller} | Parameters : {parametersAsString})";
        }
    }
    public sealed class CollectionCallNode : ExpressionNode
    {
        public ExpressionNode Caller;
        public ExpressionNode Location;


        public CollectionCallNode(Position position, ExpressionNode caller, ExpressionNode location)
        {
            this.Position = position;
            this.Caller = caller;
            this.Location = location;
            Type = NodeType.CollectionCall;
        }

        public override string ToString()
        {
            return $"(Collection : {Caller} at : {Location})";
        }
    }
}
