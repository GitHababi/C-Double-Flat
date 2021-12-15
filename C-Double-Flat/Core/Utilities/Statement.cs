using System.Collections.Generic;

namespace C_Double_Flat.Core.Utilities
{
    /// <summary>
    /// Parent class for all statements.
    /// </summary>
    public abstract class Statement
    {
        public Position Position;
        public StatementType Type;
    }

    public sealed class DisposeStatement : Statement
    {
        public ExpressionNode Disposer;

        public DisposeStatement(Position position, ExpressionNode disposer)
        {
            this.Position = position;
            this.Disposer = disposer;
            this.Type = StatementType.Dispose;
        }

        public override string ToString()
        {
            return $"- DISPOSE OF: {Disposer}";
        }
    }

    public sealed class IfStatement : Statement
    {
        public ExpressionNode Condition;
        public Statement DoIf;
        public Statement DoElse;

        public IfStatement(Position position, Statement doIf, Statement doElse, ExpressionNode condition)
        {
            this.Position = position;
            this.DoElse = doElse;
            this.DoIf = doIf;
            this.Condition = condition;
            this.Type = StatementType.If;

        }


        public override string ToString()
        {
            return $"- IF {Condition} THEN : {DoIf} \n OTHERWISE {DoElse}";
        }
    }

    public sealed class ReturnStatement : Statement
    {
        public ExpressionNode Expression;

        public ReturnStatement(Position position, ExpressionNode expression)
        {
            this.Position = position;
            this.Expression = expression;
            this.Type = StatementType.Return;
        }

        public override string ToString()
        {
            return $"- RETURN {Expression}";
        }
    }
    public sealed class RunStatement : Statement
    {
        public ExpressionNode RelativeLocation;

        public RunStatement(Position position, ExpressionNode relativeLocation)
        {
            this.Position = position;
            this.RelativeLocation = relativeLocation;
            this.Type = StatementType.Run;
        }

        public override string ToString()
        {
            return $"- RUN SCRIPT AT {RelativeLocation}";
        }
    }

    public sealed class RepeatStatement : Statement
    {
        public Statement Statement;
        public ExpressionNode Amount;


        public RepeatStatement(Position position, Statement statement, ExpressionNode amount)
        {
            this.Statement = statement;
            this.Amount = amount;
            this.Position = position;
            this.Type = StatementType.Repeat;
        }

        public override string ToString()
        {
            return $"- REPEAT {Amount} TIMES DO: {Statement}";
        }
    }

    public sealed class LoopStatement : Statement
    {
        public Statement Statement;
        public ExpressionNode Condition;


        public LoopStatement(Position position, Statement statement, ExpressionNode condition)
        {
            this.Statement = statement;
            this.Condition = condition;
            this.Position = position;
            this.Type = StatementType.Loop;
        }

        public override string ToString()
        {
            return $"- LOOP WHILE {Condition} DO: {Statement}";
        }
    }

    public sealed class FunctionStatement : Statement
    {
        public ExpressionNode Identifier;
        public Statement Statement;
        public List<ExpressionNode> ParameterNames;


        public FunctionStatement(Position position, ExpressionNode identifier, Statement statement, List<ExpressionNode> parameterNames)
        {
            this.Identifier = identifier;
            this.Statement = statement;
            this.ParameterNames = parameterNames;
            this.Position = position;
            this.Type = StatementType.Function;
        }

        public override string ToString()
        {
            string parameters = "";
            ParameterNames.ForEach(a =>
            {
                parameters += a + ",";
            });
            string output = $"- FUNCTION {Identifier} WITH PARAMETERS {parameters} : {Statement} ";



            return output;
        }
    }

    public sealed class AssignmentStatement : Statement
    {
        public ExpressionNode Identifier;
        public ExpressionNode Assignment;
        public bool Global;


        public AssignmentStatement(Position position, ExpressionNode identifier, ExpressionNode assignment, bool global)
        {
            this.Identifier = identifier;
            this.Assignment = assignment;
            this.Position = position;
            this.Global = global;
            this.Type = StatementType.Assignment;
        }

        public override string ToString()
        {
            return "- " + (Global ? "GLOBALLY" : "LOCALLY") + $" ASSIGN {Identifier} TO {Assignment}";
        }
    }

    public sealed class ExpressionStatement : Statement
    {
        public ExpressionNode Expression;


        public ExpressionStatement(Position position, ExpressionNode expression)
        {
            this.Position = position;
            this.Expression = expression;
            this.Type = StatementType.Expression;
        }

        public override string ToString()
        {
            return $"- EXPRESSION {Expression}";
        }
    }

    public sealed class StatementBlock : Statement
    {

        public List<Statement> Statements;


        public StatementBlock(List<Statement> statements)
        {
            this.Statements = statements;
            this.Type = StatementType.Block;
        }

        public override string ToString()
        {
            string output = "";

            Statements.ForEach(item =>
            {
                output += "\n" + item.ToString();
            });

            return output;
        }
    }
}
