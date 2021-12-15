using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
namespace C_Double_Flat.Core.Runtime
{
    public sealed partial class Interpreter
    {
        private static readonly Dictionary<string, IVariable> GlobalVariables = new();
        private static readonly object _lock = new();

        private static readonly Dictionary<string, IFunction> Functions = new();
        private static readonly object _lock1 = new();

        /// <summary>
        /// Safely gets a function from the Interpreter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IFunction GetFunction(string key)
        {
            lock (_lock1)
            {
                return Functions.TryGetValue(key, out var func) ? func : CustomFunction.Default;
            }
        }

        /// <summary>
        /// Safely adds function to the Interpreter. If function already exists, it will replace it.
        /// This is for specific use of adding multiple custom C# funcs.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="function"></param>
        public static void SetFunction(List<CustomFunction> functions)
        {
            functions.ForEach(f => SetFunction(f));
        }

        /// <summary>
        /// Safely adds function to the Interpreter. If function already exists, it will replace it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="function"></param>
        public static void SetFunction(CustomFunction function)
        {
            SetFunction(function.Name, function);
        }

        /// <summary>
        /// Safely adds function to the Interpreter. If function already exists, it will replace it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="function"></param>
        public static void SetFunction(string key, IFunction function)
        {
            lock (_lock1)
            {
                Functions.Remove(key);
                Functions.Add(key, function);
            }
        }

        /// <summary>
        /// Safely get a global variable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IVariable TryGetGlobalVariable(string key)
        {
            lock (_lock) { return GlobalVariables.TryGetValue(key, out var variable) ? variable : ValueVariable.Default; }
        }

        /// <summary>
        /// Safely set a global variable. If it already exists locally, remove it from the local list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetGlobalVariable(string key, IVariable value)
        {
            LocalVariables.Remove(key);
            lock (_lock)
            {
                GlobalVariables.Remove(key);
                GlobalVariables.Add(key, value);
            }
        }
        private readonly Dictionary<string, IVariable> LocalVariables = new();
        private readonly string Dir;
        private IVariable TryGetLocalVariable(string key)
        {
            return LocalVariables.TryGetValue(key, out var variable) ? variable : ValueVariable.Default;
        }

        /// <summary>
        /// Safely set a local variable. If it already exists globally, it will set it globally.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetLocalVariable(string key, IVariable value)
        {
            LocalVariables.Remove(key);
            if (GlobalVariables.ContainsKey(key))
                SetGlobalVariable(key, value);
            else
                LocalVariables.Add(key, value);
        }
        private IVariable GetVariable(string key)
        {
            if (!LocalVariables.ContainsKey(key))
                return (TryGetGlobalVariable(key));
            return (TryGetLocalVariable(key));
        }

        private void SetVariable(bool global, string key, IVariable value)
        {
            if (global)
                SetGlobalVariable(key, value);
            else
                SetLocalVariable(key, value);
        }
        public static (IVariable, bool) Interpret(Statement statement, string dir)
        {
            return new Interpreter(dir).Interpret(statement);
        }

        private Interpreter(string dir)
        {
            this.Dir = dir;
        }

        private (IVariable, bool) Interpret(Statement statement)
        {
            switch (statement.Type)
            {
                case StatementType.Dispose:
                    InterpretDispose((DisposeStatement)statement);
                    break;
                case StatementType.Expression:
                    InterpretExpression(((ExpressionStatement)statement).Expression);
                    break;
                case StatementType.Block:
                    return InterpretBlock((StatementBlock)statement);
                case StatementType.Return:
                    return InterpretReturn((ReturnStatement)statement);
                case StatementType.Assignment:
                    InterpretAssignment((AssignmentStatement)statement);
                    break;
                case StatementType.Repeat:
                    return InterpretRepeat((RepeatStatement)statement);
                case StatementType.Loop:
                    return InterpretLoop((LoopStatement)statement);
                case StatementType.If:
                    return InterpretIf((IfStatement)statement);
                case StatementType.Run:
                    return InterpretRun((RunStatement)statement);
                case StatementType.Function:
                    InterpretFunction((FunctionStatement)statement);
                    break;
                default:
                    break;
            }
            return (ValueVariable.Default, false);
        }
        private void InterpretDispose(DisposeStatement statement)
        {
            string name;
            switch (statement.Disposer.Type)
            {
                case NodeType.AsName:
                    name = GetAsNameIdentifier(statement.Disposer);
                    break;
                case NodeType.Literal:
                    name = GetLiteralIdentifier(statement.Disposer);
                    break;
                case NodeType.FunctionCall:
                    name = GetFunctionCallIdentifier(statement.Disposer);
                    lock (_lock1) Functions.Remove(name);
                    return;
                default:
                    return;
            }
            lock (_lock)
            {
                if (!GlobalVariables.Remove(name))
                    LocalVariables.Remove(name);
            }

        }

        private void InterpretFunction(FunctionStatement statement)
        {
            List<Token> args = new();
            string name;
            switch (statement.Identifier.Type)
            {
                case NodeType.AsName:
                    name = GetAsNameIdentifier(statement.Identifier);
                    break;
                case NodeType.Literal:
                    name = GetLiteralIdentifier(statement.Identifier);
                    break;
                default:
                    return;
            }

            foreach (ExpressionNode node in statement.ParameterNames)
            {
                if (node.Type != NodeType.Literal) continue;
                var param = node as LiteralNode;
                args.Add(param.Value);
            }

            var function = new UserFunction(args, statement.Statement, Dir);
            SetFunction(name, function);
        }
        private (IVariable, bool) InterpretRun(RunStatement runStatement)
        {
            var path = InterpretExpression(runStatement.RelativeLocation).AsString();
            try
            {
                path = File.Exists(path) ? path : Path.Combine(Dir, path);
                var data = File.ReadAllText(path);
                return Interpret(
                    Parser.Parser.Parse(Lexer.Tokenize(data)),
                    Path.GetDirectoryName(path)
                    );
            }
            catch { /* oopsie */ }
            return (ValueVariable.Default, false);

        }
        private (IVariable, bool) InterpretIf(IfStatement ifStatement)
        {
            if (InterpretExpression(ifStatement.Condition).AsBool())
                return Interpret(ifStatement.DoIf);
            return Interpret(ifStatement.DoElse);
        }
        private (IVariable, bool) InterpretRepeat(RepeatStatement repeatStatement)
        {
            var nums = Convert.ToInt32(InterpretExpression(repeatStatement.Amount).AsDouble());
            for (int i = 0; i < nums; i++)
            {
                var variable = Interpret(repeatStatement.Statement);
                if (variable.Item2)
                    return variable;
            }
            return (ValueVariable.Default, false);
        }
        private (IVariable, bool) InterpretLoop(LoopStatement loopStatement)
        {
            while (InterpretExpression(loopStatement.Condition).AsBool())
            {
                var variable = Interpret(loopStatement.Statement);
                if (variable.Item2)
                    return variable;
            }
            return (ValueVariable.Default, false);
        }
        private void InterpretAssignment(AssignmentStatement assignmentStatement)
        {
            string assignment;
            // Get the name of the variable to set.
            switch (assignmentStatement.Identifier.Type)
            {
                case NodeType.Literal:
                    assignment = GetLiteralIdentifier(assignmentStatement.Identifier);
                    break;
                case NodeType.AsName:
                    assignment = GetAsNameIdentifier(assignmentStatement.Identifier);
                    break;
                case NodeType.CollectionCall:
                    // Collection Call setting (i.e. asd <- [1] : 1; ) is special and requires different method
                    InterpretCollectionLocationAssignment(assignmentStatement);
                    return;
                default:
                    return;
            }
            SetVariable(assignmentStatement.Global, assignment, InterpretExpression(assignmentStatement.Assignment));
        }
        private void InterpretCollectionLocationAssignment(AssignmentStatement assignmentStatement)
        {
            CollectionCallNode collectionCallNode = (CollectionCallNode)assignmentStatement.Identifier;

            int location = Convert.ToInt32(InterpretExpression(collectionCallNode.Location).AsDouble());
            string assignment;
            switch (collectionCallNode.Caller.Type)
            {
                case NodeType.Literal:
                    assignment = GetLiteralIdentifier(collectionCallNode.Caller);
                    break;
                case NodeType.AsName:
                    assignment = GetAsNameIdentifier(collectionCallNode.Caller);
                    break;
                default:
                    return;
            }

            var current = GetVariable(assignment);
            if (current.Type() != VariableType.Collection)
                SetVariable(assignmentStatement.Global, assignment, new CollectionVariable(InterpretExpression(assignmentStatement.Assignment)));
            else
            {
                var collection = (CollectionVariable)current;
                collection.SetMember(location, InterpretExpression(assignmentStatement.Assignment));
            }

        }
        private string GetFunctionCallIdentifier(ExpressionNode node)
        {
            FunctionCallNode colnode = (FunctionCallNode)node;
            return colnode.Caller.Type switch
            {
                NodeType.Literal => GetLiteralIdentifier(colnode.Caller),
                NodeType.AsName => GetAsNameIdentifier(colnode.Caller),
                _ => ""
            };
        }
        private string GetAsNameIdentifier(ExpressionNode node)
        {
            AsNameNode asnnode = (AsNameNode)node;
            return InterpretExpression(asnnode.Identifier).AsString();
        }
        private static string GetLiteralIdentifier(ExpressionNode node)
        {
            LiteralNode litnode = (LiteralNode)node;
            return litnode.Value.Data;
        }
        private (IVariable, bool) InterpretReturn(ReturnStatement returnStatement)
        {
            return (InterpretExpression(returnStatement.Expression), true);
        }
        private (IVariable, bool) InterpretBlock(StatementBlock block)
        {
            foreach (Statement statement in block.Statements)
            {
                var x = Interpret(statement);
                if (x.Item2) return (x.Item1, true);
            }
            return (ValueVariable.Default, false);
        }
    }
}
