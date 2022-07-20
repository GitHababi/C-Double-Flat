using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;

namespace C_Double_Flat.Core.Runtime
{
    public sealed partial class Interpreter
    {
        private IVariable InterpretExpression(ExpressionNode node)
        {
            switch (node.Type)
            {
                default:
                    throw new Exception($"Expression has invalid type '{node.Type}' at Line: {node.Position.Row} Column: {node.Position.Column}.");
                case NodeType.Not:
                    return InterpretNotNode((NotNode)node);
                case NodeType.Variable:
                    return ((VariableNode)node).Value;
                case NodeType.BinaryOperation:
                    return InterpretBinaryExpression((BinaryOperationNode)node);
                case NodeType.FunctionCall:
                    return InterpretFunctionCall((FunctionCallNode)node);
                case NodeType.AsName:
                    return GetVariable(InterpretExpression(((AsNameNode)node).Identifier).AsString());
                case NodeType.Literal:
                    if (((LiteralNode)node).Value.Type == Utilities.TokenType.Identifier)
                        return GetVariable(((LiteralNode)node).Value.Data);
                    return new ValueVariable((LiteralNode)node);
                case NodeType.CollectionLiteral:
                    var elements = new List<IVariable>();
                    foreach (ExpressionNode element in ((CollectionLiteralNode)node).Elements)
                        elements.Add(InterpretExpression(element));
                    return new CollectionVariable(elements);
                case NodeType.CollectionCall:
                    return InterpretCollectionCall((CollectionCallNode)node);
            }
        }
        private IVariable InterpretNotNode(NotNode node)
        {
            var value = InterpretExpression(node.Expression);
            return new ValueVariable(!value.AsBool());

        }
        private IVariable InterpretCollectionCall(CollectionCallNode node)
        {
            string name;
            IVariable variable;
            switch (node.Caller.Type)
            {
                case NodeType.AsName:
                    name = GetIdentifier(node.Caller);
                    variable = GetVariable(name);
                    break;
                case NodeType.Literal:
                    name = GetIdentifier(node.Caller);
                    variable = GetVariable(name);
                    break;
                default:
                    variable = InterpretExpression(node.Caller);
                    break;
            }
            if (variable.Type() != VariableType.Collection)
                return variable;
            var collec = (CollectionVariable)variable;

            var location = (int)InterpretExpression(node.Location).AsDouble() - 1;
            if (location >= collec.Variables.Count)
                return ValueVariable.Default;
            return collec.Variables[location];
        }
        private IVariable InterpretFunctionCall(FunctionCallNode node)
        {
            if (node.Caller.Type != NodeType.AsName && node.Caller.Type != NodeType.Literal)
                return ValueVariable.Default;
         
            List<IVariable> parameters = new();
            var name = GetIdentifier(node.Caller);
            node.Parameters.ForEach(p => parameters.Add(InterpretExpression(p)));

            var function = GetFunction(name);
            try
            {
                Environment.CurrentDirectory = this.Dir; // ensure all execution happens where the interpreter is
                return function.Run(parameters);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                /* oopsie */
                return ValueVariable.Default;
            }

        }

        private IVariable InterpretBinaryExpression(BinaryOperationNode node)
        {
            var left = InterpretExpression(node.Left);

            var right = InterpretExpression(node.Right);

            return node.Operation.Type switch
            {
                TokenType.Plus => Add(left, right),
                TokenType.Minus => Subtract(left, right),
                TokenType.Divide => Divide(left, right),
                TokenType.Multiply => Multiply(left, right),
                TokenType.Equal => Equals(left, right),
                TokenType.NotEqual => new ValueVariable(!Equals(left, right).AsBool()),
                TokenType.LessThan => LessThan(left, right),
                TokenType.GreaterThanOrEqual => new ValueVariable(!LessThan(left, right).AsBool()),
                TokenType.GreaterThan => GreaterThan(left, right),
                TokenType.LessThanOrEqual => new ValueVariable(!GreaterThan(left, right).AsBool()),
                TokenType.And => And(left, right),
                TokenType.Or => Or(left, right),
                _ => throw new Exception($"Operation has invalid type '{node.Operation}' at Line: {node.Position.Row} Column: {node.Position.Column}"),
            };
        }
        public static IVariable And(IVariable left, IVariable right)
        {
            // Check if either operand is a collection
            if (left.Type() != right.Type() || left.Type() == VariableType.Collection)
                return new ValueVariable(false);

            return new ValueVariable(left.AsBool() && right.AsBool());
        }
        public static IVariable Or(IVariable left, IVariable right)
        {
            // Check if either operand is a collection
            if (left.Type() != right.Type() || left.Type() == VariableType.Collection)
                return new ValueVariable(false);

            return new ValueVariable(left.AsBool() || right.AsBool());
        }
        public static IVariable GreaterThan(IVariable left, IVariable right)
        {
            return new ValueVariable(left.AsDouble() > right.AsDouble());
        }
        public static IVariable LessThan(IVariable left, IVariable right)
        {
            return new ValueVariable(left.AsDouble() < right.AsDouble());
        }

        public static IVariable Equals(IVariable left, IVariable right)
        {
            if (left.Type() != right.Type())
                return new ValueVariable(false);
            if (left.Type() != VariableType.Collection)
                return new ValueVariable(((ValueVariable)left).Value.Equals(((ValueVariable)right).Value));
            if (((CollectionVariable)left).Variables.Count != ((CollectionVariable)right).Variables.Count)
                return new ValueVariable(false);

            for (int i = 0; i < ((CollectionVariable)left).Variables.Count; i++)
            {
                if (!((ValueVariable)Equals(((CollectionVariable)left).Variables[i], ((CollectionVariable)right).Variables[i])).AsBool())
                    return new ValueVariable(false);
            }
            return new ValueVariable(true);

        }
        public static IVariable Divide(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection || right.Type() == VariableType.Collection)
                return ValueVariable.Default;
            if (right.AsDouble() == 0)
                return left;
            return new ValueVariable(left.AsDouble() / right.AsDouble());
        }
        private static IVariable Multiply(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection || right.Type() == VariableType.Collection)
                return ValueVariable.Default;
            return new ValueVariable(left.AsDouble() * right.AsDouble());
        }
        public static IVariable Subtract(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection || right.Type() == VariableType.Collection)
                return ValueVariable.Default;
            return new ValueVariable(left.AsDouble() - right.AsDouble());
        }
        public static IVariable Add(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection || right.Type() == VariableType.Collection)
                return ValueVariable.Default;
            return Add((ValueVariable)left, (ValueVariable)right);
        }
        private static IVariable Add(ValueVariable left, ValueVariable right)
        {
            var x = ValueVariable.ResolveAddingTypes(left, right);

            var value = x.Item1.Value.Type switch
            {
                Utilities.ValueType.Number => (x.Item1.AsDouble() + x.Item2.AsDouble()).ToString(),
                Utilities.ValueType.String => x.Item1.AsString() + x.Item2.AsString(),
                _ => throw new Exception($"ValueVariable has invalid type '{left.Value.Type}' while adding."),
            };
            return new ValueVariable(new Value(x.Item1.Value.Type, value));
        }
    }
}
