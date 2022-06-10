using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace C_Double_Flat.Core.Runtime
{
    public sealed partial class Interpreter
    {
        private static readonly Dictionary<string, IVariable> GlobalVariables = new();
        private static readonly object _lock = new();

        private static readonly Dictionary<string, IFunction> Functions = new();
        private static readonly object _lock1 = new();
        public static (IVariable, bool) Interpret(Statement statement, string dir)
        {
            return new Interpreter(dir).Interpret(statement);
        }

        private Interpreter(string dir)
        {
            this.Dir = dir;
        }

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

        public static void LoadLibraryFromPath(string libraryPath)
        {

            var lib = Assembly.LoadFile(libraryPath);
            var types = lib.GetTypes();
            foreach (var type in types)
            {
                if (typeof(ILoadable).IsAssignableFrom(type))
                    Interpreter.SetFunction(((ILoadable)Activator.CreateInstance(type)).GetFunctions());
            }
        }

        public static void LoadLibrary(ILoadable library)
        {
            Interpreter.SetFunction(library.GetFunctions());
        }


        private (IVariable, bool) Interpret(Statement statement)
        {
            Environment.CurrentDirectory = this.Dir; // ensure all execution happens where the interpreter is
           try
           {
                switch (statement.Type)
                {
                    case StatementType.Dispose:
                        InterpretDispose((DisposeStatement)statement);
                        break;
                    case StatementType.Expression:
                        return (InterpretExpression(((ExpressionStatement)statement).Expression),false);
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

            }
            catch { /* Oopsie */ }
            Environment.CurrentDirectory = this.Dir;
            return (ValueVariable.Default, false);
        }
        private void InterpretDispose(DisposeStatement statement)
        {
            if (statement.Disposer.Type != NodeType.AsName &&
                statement.Disposer.Type != NodeType.Literal &&
                statement.Disposer.Type != NodeType.Variable)
                return;
            string name = GetIdentifier(statement.Disposer);
            lock (_lock)
            {
                if (!GlobalVariables.Remove(name))
                    LocalVariables.Remove(name);
            }
        }

        private void InterpretFunction(FunctionStatement statement)
        {
            List<Token> args = new();
            if (statement.Identifier.Type != NodeType.AsName &&
                statement.Identifier.Type != NodeType.Literal)
                return;
            string name = GetIdentifier(statement.Identifier);

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
                if (path.EndsWith(".dll"))
                {
                    LoadLibraryFromPath(path);
                    return (ValueVariable.Default, false);
                }
                var data = File.ReadAllText(path);

                var statements = Parser.Parser.Parse(Lexer.Tokenize(data));
                var location = Path.GetDirectoryName(Path.Combine(Environment.CurrentDirectory, path));
                return Interpret(statements, location);
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
        private void InterpretAssignment(AssignmentStatement statement)
        {
            if (statement.Identifier.Type == NodeType.CollectionCall)
            {
                _ = InterpretCollectionLocationAssignment(statement.Identifier as CollectionCallNode, statement.Assignment, statement.Global, true);
                // discard the return value as it's only used internally
                return;
            }
            
            if (statement.Identifier.Type != NodeType.AsName &&
                statement.Identifier.Type != NodeType.Literal)
                return;
            string assignment = GetIdentifier(statement.Identifier);
            
            SetVariable(statement.Global, assignment, InterpretExpression(statement.Assignment));
        }

        /// <summary>
        /// A helper method for assigning variables to collections, regardless of the number of dimensions.
        /// </summary>
        private IVariable InterpretCollectionLocationAssignment(CollectionCallNode assigner, ExpressionNode assignment, bool globality, bool topLevel = false)
        {
            var location = Convert.ToInt32(InterpretExpression(assigner.Location).AsDouble()) - 1;
            CollectionVariable variable;
            if (assigner.Caller.Type != NodeType.CollectionCall)
            {
                var baseId = GetIdentifier(assigner.Caller);
                var baseVariable = GetVariable(baseId);
                if (baseVariable.Type() != VariableType.Collection)
                {
                    baseVariable = new CollectionVariable();
                    SetVariable(globality, baseId, baseVariable);
                }
                variable = ((CollectionVariable)baseVariable).ExtendTo(location + 1);
            }
            else
                variable = ((CollectionVariable)InterpretCollectionLocationAssignment(assigner.Caller as CollectionCallNode, assignment, globality))
                           .ExtendTo(location+1);
            if (location < 1)
                variable.ExtendTo(1); 
            if (variable.Variables[location].Type() != VariableType.Collection && !topLevel)
                variable.Variables[location] = new CollectionVariable();
            if (topLevel)
                variable.Variables[location] = InterpretExpression(assignment);
            else
                ((CollectionVariable)variable.Variables[location]).ExtendTo(location+1);
            return variable.Variables[location];
        }
        private string GetIdentifier(ExpressionNode node)
        {
            if (node.Type == NodeType.Literal)
                return ((LiteralNode)node).Value.Data;
            if (node.Type == NodeType.AsName)
                return InterpretExpression(((AsNameNode)node).Identifier).AsString();
            if (node.Type == NodeType.FunctionCall)
            {
                FunctionCallNode colnode = (FunctionCallNode)node;
                return colnode.Caller.Type switch
                {
                    NodeType.Literal => GetIdentifier(colnode.Caller),
                    NodeType.AsName => GetIdentifier(colnode.Caller),
                    _ => ""
                };
            }
            if (node.Type == NodeType.CollectionCall)
            {
                CollectionCallNode colnode = (CollectionCallNode)node;
                return GetIdentifier(colnode.Caller);
            }
            return "";
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
