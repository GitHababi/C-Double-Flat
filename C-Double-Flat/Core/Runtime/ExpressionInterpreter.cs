using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Utilities;

namespace C_Double_Flat.Core.Runtime
{
    public sealed partial class Interpreter
    {
        private Variable InterpretExpression(ExpressionNode node)
        {
            switch (node.Type)
            {
                default:
                    throw new Exception($"Expression has invalid type '{node.Type}' at Line: {node.Position.Row} Column: {node.Position.Column}.");
                case NodeType.Variable:
                    return ((VariableNode)node).Value;
                case NodeType.BinaryOperation:
                    return InterpretBinaryExpression((BinaryOperationNode)node);
                case NodeType.FunctionCall:
                    return InterpretFunctionCall((FunctionCallNode)node);
                case NodeType.Literal:
                    if (((LiteralNode)node).Value.Type == Utilities.TokenType.Identifier)
                        return GetVariable(((LiteralNode)node).Value.Data);
                    return new ValueVariable((LiteralNode)node);
                case NodeType.CollectionLiteral:
                    var elements = new List<Variable>();
                    foreach (ExpressionNode element in ((CollectionLiteralNode)node).Elements)
                        elements.Add(InterpretExpression(element));
                    return new CollectionVariable(elements);
                case NodeType.CollectionCall:
                    return InterpretCollectionCall((CollectionCallNode)node);
            }
        }
        private Variable InterpretCollectionCall(CollectionCallNode node)
        {
            string name;
            Variable variable;
            switch (node.Caller.Type)
            {
                case NodeType.AsName:
                    name = InterpretAsNameAssignment(node.Caller);
                    variable = GetVariable(name);
                    break;
                case NodeType.Literal:
                    name = InterpretLiteralAssignment(node.Caller);
                    variable = GetVariable(name);
                    break;
                default:
                    variable = InterpretExpression(node.Caller);
                    break;
            }
            if (variable.Type != VariableType.Collection) 
                return variable;
            var collec = (CollectionVariable)variable;

            return collec.AccessMember((int)(double)InterpretExpression(node.Location));
        }
        private Variable InterpretFunctionCall(FunctionCallNode node)
        {
            string name = "";
            List<Variable> parameters = new();
            switch (node.Caller.Type)
            {
                case NodeType.AsName:
                    name = InterpretAsNameAssignment(node.Caller);
                    break;
                case NodeType.Literal:
                    name = InterpretLiteralAssignment(node.Caller);
                    break;
                default:
                    return ValueVariable.Default;
            }

            node.Parameters.ForEach(p => parameters.Add(InterpretExpression(p)));

            var function = GetFunction(name);

            return function.Run(parameters);

        }

        private Variable InterpretBinaryExpression(BinaryOperationNode node)
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
                    return new ValueVariable(!(bool)Equals(left, right));
                case TokenType.LessThan:
                    return LessThan(left, right);
                case TokenType.GreaterThanOrEqual:
                    return new ValueVariable(!(bool)LessThan(left, right));
                case TokenType.GreaterThan:
                    return GreaterThan(left, right);
                case TokenType.LessThanOrEqual:
                    return new ValueVariable(!(bool)GreaterThan(left, right));
                case TokenType.And:
                    return And(left, right);
                case TokenType.Or:
                    return Or(left, right);
                default:
                    throw new Exception($"Operation has invalid type '{node.Operation}' at Line: {node.Position.Row} Column: {node.Position.Column}");
            }
        }
        public static Variable And(Variable left, Variable right)
        {
            // Check if either operand is a collection
            if (left.Type != right.Type || left.Type == VariableType.Collection)
                return new ValueVariable(false);

            return new ValueVariable((bool)(ValueVariable)(left) && (bool)(ValueVariable)(right));
        }
        public static Variable Or(Variable left, Variable right)
        {
            // Check if either operand is a collection
            if (left.Type != right.Type || left.Type == VariableType.Collection)
                return new ValueVariable(false);

            return new ValueVariable((bool)(ValueVariable)(left) || (bool)(ValueVariable)(right));
        }
        public static Variable GreaterThan(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return new ValueVariable((double)(CollectionVariable)left > (double)(CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return new ValueVariable((double)(CollectionVariable)left > (double)(ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return new ValueVariable((double)(ValueVariable)left > (double)(CollectionVariable)right);
            return new ValueVariable((double)(ValueVariable)left > (double)(ValueVariable)right);
        }
        public static Variable LessThan(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return new ValueVariable((double)(CollectionVariable)left < (double)(CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return new ValueVariable((double)(CollectionVariable)left < (double)(ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return new ValueVariable((double)(ValueVariable)left < (double)(CollectionVariable)right);
            return new ValueVariable((double)(ValueVariable)left < (double)(ValueVariable)right);
        }

        public static Variable Equals(Variable left, Variable right)
        {
            if (left.Type != right.Type)
                return new ValueVariable(false);
            if (left.Type == VariableType.Collection)
            {
                if (((CollectionVariable)left).Variables.Count != ((CollectionVariable)right).Variables.Count)
                    return new ValueVariable(false);

                for (int i = 0; i < (left as CollectionVariable).Variables.Count; i++)
                {
                    if (!(bool)(ValueVariable)Equals(((CollectionVariable)left).Variables[i], ((CollectionVariable)right).Variables[i]))
                        return new ValueVariable(false);
                }
                return new ValueVariable(true);
            }
            return new ValueVariable(((ValueVariable)left).Value.Equals(((ValueVariable)right).Value));
        }

        #region Operations
        #region Divide
        public static Variable Divide(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return Divide((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return Divide((CollectionVariable)left, (ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return Divide((ValueVariable)left, (CollectionVariable)right);
            return Divide((ValueVariable)left, (ValueVariable)right);
        }
        private static Variable Divide(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<Variable>();
            for (int i = 0; i < Math.Min(left.Variables.Count, right.Variables.Count); i++)
                output.Add(Divide(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static Variable Divide(ValueVariable left, CollectionVariable right)
        {
            for (int i = 0; i < right.Variables.Count; i++)
                right.Variables[i] = Divide(left, right.Variables[i]);
            return right;
        }
        private static Variable Divide(CollectionVariable left, ValueVariable right)
        {
            for (int i = 0; i < left.Variables.Count; i++)
                left.Variables[i] = Divide(left.Variables[i], right);
            return left;
        }
        private static Variable Divide(ValueVariable left, ValueVariable right)
        {
            if ((double)right == 0) return left;
            return new ValueVariable(new Value(Utilities.ValueType.Number,
                ((double)left / (double)right).ToString()
                ));
        }
        #endregion
        #region Multiply
        private static Variable Multiply(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return Multiply((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return Multiply((CollectionVariable)left, (ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return Multiply((CollectionVariable)right, (ValueVariable)left);
            return Multiply((ValueVariable)left, (ValueVariable)right);
        }
        private static Variable Multiply(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<Variable>();
            for (int i = 0; i < Math.Min(left.Variables.Count, right.Variables.Count); i++)
                output.Add(Multiply(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static Variable Multiply(CollectionVariable collection, ValueVariable factor)
        {
            for (int i = 0; i < collection.Variables.Count; i++)
                collection.Variables[i] = Multiply(collection.Variables[i], factor);
            return collection;
        }
        private static Variable Multiply(ValueVariable left, ValueVariable right)
        {
            left.Value.Data = ((double)left * (double)right).ToString();
            return left;
        }
        #endregion
        #region Subtraction
        public static Variable Subtract(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return Subtract((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return Subtract((CollectionVariable)left, (ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return Subtract((ValueVariable)left, (CollectionVariable)right);
            return Subtract((ValueVariable)left, (ValueVariable)right);
        }
        private static Variable Subtract(CollectionVariable left, CollectionVariable right)
        {
            var output = new List<Variable>();
            for (int i = 0; i < Math.Min(left.Variables.Count, right.Variables.Count); i++)
                output.Add(Subtract(left.Variables[i], right.Variables[i]));
            return new CollectionVariable(output);
        }
        private static Variable Subtract(ValueVariable left, CollectionVariable right)
        {
            for (int i = 0; i < right.Variables.Count; i++)
                right.Variables[i] = Subtract(left, right.Variables[i]);
            return right;
        }
        private static Variable Subtract(CollectionVariable left, ValueVariable right)
        {
            for (int i = 0; i < left.Variables.Count; i++)
                left.Variables[i] = Subtract(left.Variables[i], right);
            return left;
        }
        private static Variable Subtract(ValueVariable left, ValueVariable right)
        {
            return new ValueVariable(new Value(Utilities.ValueType.Number,
                ((double)left - (double)right).ToString()
                ));
        }
        #endregion
        #region Addition
        public static Variable Add(Variable left, Variable right)
        {
            if (left.Type == VariableType.Collection && right.Type == VariableType.Collection)
                return Add((CollectionVariable)left, (CollectionVariable)right);
            if (left.Type == VariableType.Collection)
                return Add((CollectionVariable)left, (ValueVariable)right);
            if (right.Type == VariableType.Collection)
                return Add((ValueVariable)left, (CollectionVariable)right);
            return Add((ValueVariable)left, (ValueVariable)right);
        }

        public static Variable Add(CollectionVariable collection, ValueVariable addend)
        {
            for (int i = 0; i < collection.Variables.Count; i++)
                collection.Variables[i] = Add(collection.Variables[i], addend);
            return collection;
        }
        private static Variable Add(CollectionVariable left, CollectionVariable right)
        {
            left.Variables.AddRange(right.Variables);
            return left;
        }
        private static Variable Add(ValueVariable left, ValueVariable right)
        {
            var x = ValueVariable.ResolveAddingTypes(left, right);
            left = x.Item1;
            right = x.Item2;

            switch (left.Value.Type)
            {
                case Utilities.ValueType.Number:
                    left.Value.Data = ((double)left + (double)right).ToString();
                    break;
                case Utilities.ValueType.String:
                    left.Value.Data = (string)left + (string)right;
                    break;
                default:
                    throw new Exception($"ValueVariable has invalid type '{left.Value.Type}' while adding.");
            }

            return left;
        }
        #endregion
        #endregion
    }
}
