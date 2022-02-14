using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;

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
                case TokenType.Identifier:
                case TokenType.AsName:
                    return ParseAmbiguousDeclaration();
                case TokenType.Dispose:
                    return ParseDisposeStatement();
                default:
                    return ParseExpressionStatement();
            }
        }
        private Statement ParseDisposeStatement()
        {
            var position = ExpectThenNext(TokenType.Dispose).Position;
            var disposer = ParsePrimaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new DisposeStatement(position, disposer);
        }

        private Statement ParseAmbiguousDeclaration()
        {
            var position = CurrentToken.Position;
            var identifier = ParsePrimaryExpression();
            if (Peek(0).Type != TokenType.Assignment) // Filter out any regular expression statements
            {
                var expression = ParseBinaryExpression(identifier);
                ExpectThenNext(TokenType.SemiColon);
                return new ExpressionStatement(position, expression);
            }
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement();
            if (identifier.Type == NodeType.FunctionCall)
                return new FunctionStatement(position, ((FunctionCallNode)identifier).Caller, statement, ((FunctionCallNode)identifier).Parameters);
            else return new AssignmentStatement(position, identifier, ((ExpressionStatement)statement).Expression, false);
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
            var condition = ParseBinaryExpression();
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
            var condition = ParseBinaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement(scoped);

            return new LoopStatement(position, statement, condition);

        }

        private Statement ParseRepeatStatement(bool scoped = false)
        {
            var position = ExpectThenNext(TokenType.Repeat).Position;
            var amount = ParseBinaryExpression();
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

       
        private Statement ParseExpressionStatement()
        {
            var position = CurrentToken.Position;
            var expression = ParseBinaryExpression();
            ExpectThenNext(TokenType.SemiColon);
            return new ExpressionStatement(position, expression);
        }
    }
}
