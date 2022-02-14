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
                    name = GetAsNameIdentifier(node.Caller);
                    variable = GetVariable(name);
                    break;
                case NodeType.Literal:
                    name = GetLiteralIdentifier(node.Caller);
                    variable = GetVariable(name);
                    break;
                default:
                    variable = InterpretExpression(node.Caller);
                    break;
            }
            if (variable.Type() != VariableType.Collection)
                return variable;
            var collec = (CollectionVariable)variable;

            return collec.AccessMember((int)InterpretExpression(node.Location).AsDouble());
        }
        private IVariable InterpretFunctionCall(FunctionCallNode node)
        {
            string name = "";
            List<IVariable> parameters = new();
            switch (node.Caller.Type)
            {
                case NodeType.AsName:
                    name = GetAsNameIdentifier(node.Caller);
                    break;
                case NodeType.Literal:
                    name = GetLiteralIdentifier(node.Caller);
                    break;
                default:
                    return ValueVariable.Default;
            }

            node.Parameters.ForEach(p => parameters.Add(InterpretExpression(p)));

            var function = GetFunction(name);
            try
            {
                return function.Run(parameters);
            }
            catch
            { 
                /* oopsie */
                return ValueVariable.Default;
            }

        }

        private IVariable InterpretBinaryExpression(BinaryOperationNode node)
        {
            var left = InterpretExpression(node.Left);

            var right = InterpretExpression(node.Right);

            switch (node.Operation.Type)
            {
                case TokenType.Plus:
                    return Add(left, right);
                case TokenType.Minus:
                    return Subtract(left, right);
                case TokenType.Divide:
                    return Divide(left, right);
                case TokenType.Multiply:
                    return Multiply(left, right);
                case TokenType.Equal:
                    return Equals(left, right);
                case TokenType.NotEqual:
                    return new ValueVariable(!Equals(left, right).AsBool());
                case TokenType.LessThan:
                    return LessThan(left, right);
                case TokenType.GreaterThanOrEqual:
                    return new ValueVariable(!LessThan(left, right).AsBool());
                case TokenType.GreaterThan:
                    return GreaterThan(left, right);
                case TokenType.LessThanOrEqual:
                    return new ValueVariable(!GreaterThan(left, right).AsBool());
                case TokenType.And:
                    return And(left, right);
                case TokenType.Or:
                    return Or(left, right);
                default:
                    throw new Exception($"Operation has invalid type '{node.Operation}' at Line: {node.Position.Row} Column: {node.Position.Column}");
            }
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
            if (left.Type() == VariableType.Collection)
            {
                if (((CollectionVariable)left).Variables.Length != ((CollectionVariable)right).Variables.Length)
                    return new ValueVariable(false);

                for (int i = 0; i < ((CollectionVariable)left).Variables.Length; i++)
                {
                    if (!((ValueVariable)Equals(((CollectionVariable)left).Variables[i], ((CollectionVariable)right).Variables[i])).AsBool())
                        return new ValueVariable(false);
                }
                return new ValueVariable(true);
            }
            return new ValueVariable(((ValueVariable)left).Value.Equals(((ValueVariable)right).Value));
        }

        #region Operations
        #region Divide
        public static IVariable Divide(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection && right.Type() == VariableType.Collection)
                return Divide((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type() == VariableType.Collection)
                return Divide((CollectionVariable)left, (ValueVariable)right);
            if (right.Type() == VariableType.Collection)
                return Divide((ValueVariable)left, (CollectionVariable)right);
            return Divide((ValueVariable)left, (ValueVariable)right);
        }
        private static IVariable Divide(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>();
            for (int i = 0; i < Math.Min(left.Variables.Length, right.Variables.Length); i++)
                output.Add(Divide(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static IVariable Divide(ValueVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>();
            foreach (var item in right.Variables)
                output.Add(Divide(left, item));
            return new CollectionVariable(output);
        }
        private static IVariable Divide(CollectionVariable left, ValueVariable right)
        {
            var output = new List<IVariable>();
            foreach (var item in left.Variables)
                output.Add(Divide(item, right));
            return new CollectionVariable(output);
        }
        private static IVariable Divide(ValueVariable left, ValueVariable right)
        {
            if (right.AsDouble() == 0) return left;
            return new ValueVariable(left.AsDouble() / right.AsDouble());
        }
        #endregion
        #region Multiply
        private static IVariable Multiply(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection && right.Type() == VariableType.Collection)
                return Multiply((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type() == VariableType.Collection)
                return Multiply((CollectionVariable)left, (ValueVariable)right);
            if (right.Type() == VariableType.Collection)
                return Multiply((CollectionVariable)right, (ValueVariable)left);
            return Multiply((ValueVariable)left, (ValueVariable)right);
        }
        private static IVariable Multiply(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>();
            for (int i = 0; i < Math.Min(left.Variables.Length, right.Variables.Length); i++)
                output.Add(Multiply(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static IVariable Multiply(CollectionVariable collection, ValueVariable factor)
        {
            var output = new List<IVariable>();
            foreach (var item in collection.Variables)
                output.Add(Multiply(item, factor));
            return new CollectionVariable(output);
        }
        private static IVariable Multiply(ValueVariable left, ValueVariable right)
        {
            return new ValueVariable(left.AsDouble() * right.AsDouble());
        }
        #endregion
        #region Subtraction
        public static IVariable Subtract(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection && right.Type() == VariableType.Collection)
                return Subtract((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type() == VariableType.Collection)
                return Subtract((CollectionVariable)left, (ValueVariable)right);
            if (right.Type() == VariableType.Collection)
                return Subtract((ValueVariable)left, (CollectionVariable)right);
            return Subtract((ValueVariable)left, (ValueVariable)right);
        }
        private static IVariable Subtract(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>();
            for (int i = 0; i < Math.Min(left.Variables.Length, right.Variables.Length); i++)
                output.Add(Subtract(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static IVariable Subtract(ValueVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>();
            foreach (var item in right.Variables)
                output.Add(Subtract(left, item));
            return new CollectionVariable(output);
        }
        private static IVariable Subtract(CollectionVariable left, ValueVariable right)
        {
            var output = new List<IVariable>();
            foreach (var item in left.Variables)
                output.Add(Subtract(item, right));
            return new CollectionVariable(output);
        }
        private static IVariable Subtract(ValueVariable left, ValueVariable right)
        {
            return new ValueVariable(left.AsDouble() - right.AsDouble());
        }
        #endregion
        #region Addition
        public static IVariable Add(IVariable left, IVariable right)
        {
            if (left.Type() == VariableType.Collection && right.Type() == VariableType.Collection)
                return Add((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type() == VariableType.Collection)
                return Add((CollectionVariable)left, (ValueVariable)right);
            if (right.Type() == VariableType.Collection)
                return Add((CollectionVariable)right, (ValueVariable)left);
            return Add((ValueVariable)left, (ValueVariable)right);
        }

        public static IVariable Add(CollectionVariable collection, ValueVariable addend)
        {
            var output = new List<IVariable>();
            foreach (var item in collection.Variables)
                output.Add(Add(item, addend));
            return new CollectionVariable(output);
        }
        private static IVariable Add(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<IVariable>(left.Variables);
            output.AddRange(right.Variables);
            return new CollectionVariable(output);
        }
        private static IVariable Add(ValueVariable left, ValueVariable right)
        {
            var x = ValueVariable.ResolveAddingTypes(left, right);
            var op1 = x.Item1;
            var op2 = x.Item2;

            switch (op1.Value.Type)
            {
                case Utilities.ValueType.Number:
                    op1.Value.Data = (op1.AsDouble() + op2.AsDouble()).ToString();
                    break;
                case Utilities.ValueType.String:
                    op1.Value.Data = op1.AsString() + op2.AsString();
                    break;
                default:
                    throw new Exception($"ValueVariable has invalid type '{left.Value.Type}' while adding.");
            }

            return op1;
        }
        #endregion
        #endregion
    }
}
