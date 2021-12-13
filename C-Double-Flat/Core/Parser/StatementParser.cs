using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_Double_Flat.Core.Utilities;

namespace C_Double_Flat.Core.Parser
{
    public partial class Parser
    {
        private Statement ParseStatementBlock(bool scoped = false)
        {
            List<Statement> output = new();
            ExpectThenNext(TokenType.LeftCurlyBracket);

            while (CurrentToken.Type != TokenType.EndOfFile && CurrentToken.Type != TokenType.RightCurlyBracket)
            {
                var current = CurrentToken;
                output.Add(ParseStatement(scoped));
                if (current.Equals(CurrentToken)) break;
            }
            Next();

            return new StatementBlock(output);
        }

        private Statement ParseStatement(bool scoped = false)
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Global:
                case TokenType.Local:
                    return ParseVarAssignment();
                case TokenType.Run:
                    return ParseRunStatement();
                case TokenType.Identifier:
                    if (IsAssignmentStatement()) return ParseFunctionStatement(scoped);
                    return ParseExpressionStatement();
                case TokenType.LeftCurlyBracket:
                    return ParseStatementBlock(scoped);
                case TokenType.Return:
                    return ParseReturnStatement();
                case TokenType.Repeat:
                    return ParseRepeatStatement(scoped);
                case TokenType.Loop:
                    return ParseLoopStatement(scoped);
                case TokenType.If:
                    return ParseIfStatement(scoped);
                case TokenType.Else:
                    return ParseLoneElseStatement(scoped);
                case TokenType.AsName:
                    return ParseAmbiguousAsName(scoped);
                default:
                    return ParseExpressionStatement();
            }
        }

        /// <summary>
        /// Checks if parser will hit assignment or semicolon first, determining whether it is parsing some sort of
        /// assignment or an expression
        /// </summary>
        /// <returns></returns>
        private bool IsAssignmentStatement()
        {
            var assignment = false;
            for (int peeker = 0; peeker < (Tokens.Length - Index); peeker++)
            {
                if (Peek(peeker).Type == TokenType.Assignment) { assignment = true; break; }
                if (Peek(peeker).Type == TokenType.SemiColon) { assignment = false; break; }
            }
            return assignment;
        }
        private Statement ParseAmbiguousAsName(bool scoped = false)
        {
            // Determine whether it is just an assignment or an expression statement


            if (!IsAssignmentStatement()) return ParseExpressionStatement();

            // Now it is determined to be an assignment or function declaration
            // We can use similar code to that of ParseFunctionStatement

            var identifier = ParsePrimaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement();

            var parameters = new List<ExpressionNode>();
            // Parse Parameters if necessary. 
            if (CurrentToken.Type == TokenType.Insertion)
            {
                Next();
                ExpectThenNext(TokenType.LeftParenthesis);
                parameters = ParseExpressionList();
                ExpectThenNext(TokenType.RightParenthesis);
                ExpectThenNext(TokenType.SemiColon);
            }

            if (statement.Type == StatementType.Expression)
                return new AssignmentStatement(identifier.Position, identifier, ((ExpressionStatement)statement).Expression, false);

            return new FunctionStatement(identifier.Position, identifier, statement, parameters);
        }


        private Statement ParseRunStatement()
        {
            var position = ExpectThenNext(TokenType.Run).Position;
            var relativeLocation = ParseBinaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new RunStatement(position, relativeLocation);

        }

        /// <summary>
        /// The if statement function already integrates else statements, but the user should be able to just run an else statement no matter what.
        /// </summary>
        /// <param name="scoped"></param>
        /// <returns></returns>
        private Statement ParseLoneElseStatement(bool scoped = false)
        {
            ExpectThenNext(TokenType.Else);
            ExpectThenNext(TokenType.Assignment);
            return ParseStatement(scoped);
        }

        private Statement ParseIfStatement(bool scoped = false)
        {
            var position = ExpectThenNext(TokenType.If).Position;
            ExpectThenNext(TokenType.LeftParenthesis);
            var condition = ParseBinaryExpression();
            ExpectThenNext(TokenType.RightParenthesis);
            ExpectThenNext(TokenType.Assignment);
            var ifStatement = ParseStatement(scoped);
            Statement elseStatement = new StatementBlock(new List<Statement>());
            if (CurrentToken.Type == TokenType.Else)
            {
                Next();
                ExpectThenNext(TokenType.Assignment);
                elseStatement = ParseStatement(scoped);
            }

            return new IfStatement(position, ifStatement, elseStatement, condition);

        }


        private Statement ParseLoopStatement(bool scoped = false)
        {
            var position = ExpectThenNext(TokenType.Loop).Position;
            ExpectThenNext(TokenType.LeftParenthesis);
            var condition = ParseBinaryExpression();
            ExpectThenNext(TokenType.RightParenthesis);
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement(scoped);

            return new LoopStatement(position, statement, condition);

        }

        private Statement ParseRepeatStatement(bool scoped = false)
        {
            var position = ExpectThenNext(TokenType.Repeat).Position;
            ExpectThenNext(TokenType.LeftParenthesis);
            var amount = ParseBinaryExpression();
            ExpectThenNext(TokenType.RightParenthesis);
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement(scoped);

            return new RepeatStatement(position, statement, amount);

        }

        private Statement ParseVarAssignment()
        {
            var globality = Next();
            var identifier = ParsePrimaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var assignment = ParseBinaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new AssignmentStatement(identifier.Position, identifier, assignment, globality.Type == TokenType.Global);
        }

        private Statement ParseReturnStatement()
        {
            var position = ExpectThenNext(TokenType.Return).Position;
            var expression = ParseBinaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new ReturnStatement(position, expression);
        }

        private Statement ParseFunctionStatement(bool scoped = false)
        {
            var identifier = ParsePrimaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement(true);
            var parameters = new List<ExpressionNode>();

            // Parse Parameters if necessary. 
            if (CurrentToken.Type == TokenType.Insertion)
            {
                Next();
                ExpectThenNext(TokenType.LeftParenthesis);
                parameters = ParseExpressionList();
                ExpectThenNext(TokenType.RightParenthesis);
                ExpectThenNext(TokenType.SemiColon);
            }

            // If no args declared, stmt may be var assignment, not function assignment.
            else if (statement.Type == StatementType.Expression)
                return new AssignmentStatement(identifier.Position, identifier, ((ExpressionStatement)statement).Expression, false);

            return new FunctionStatement(identifier.Position, identifier, statement, parameters);
        }


        private Statement ParseExpressionStatement()
        {
            var position = CurrentToken.Position;
            var expression = ParseBinaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new ExpressionStatement(position, expression);
        }
    }
}
