using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Runtime;
namespace C_Double_Flat.Core.Utilities
{
    public class UserFunction : IFunction
    {
        public string Name { get => "User Function"; }

        private readonly Statement statement;
        private readonly List<Token> args;
        private readonly string sourceDir;
        public IVariable Run(List<IVariable> parameters)
        {
            List<Statement> argsStatements = new();
            for (int i = 0; i < args.Count; i++)
            {
                VariableNode assignment;
                if (i < parameters.Count)
                    assignment = new VariableNode(parameters[i]);
                else
                    assignment = new VariableNode(ValueVariable.Default);
                
                var identifier = new LiteralNode(args[i]);
                argsStatements.Add
                   (new AssignmentStatement(Position.Zero, identifier, assignment, false));
            }
            argsStatements.Add(this.statement);
            var toExecute = new StatementBlock(argsStatements);
            return Interpreter.Interpret(toExecute, sourceDir).Item1;
        }

        public UserFunction(List<Token> args, Statement statement, string sourceDir)
        {
            this.statement = statement;
            this.args = args;
            this.sourceDir = sourceDir;
        }
    }
}
