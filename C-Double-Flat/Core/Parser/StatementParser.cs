using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;

namespace C_Double_Flat.Core.Parser
{
    public partial class Parser
    {
        private Statement ParseStatementBlock()
        {
            List<Statement> output = new();
            ExpectThenNext(TokenType.LeftCurlyBracket);

            while (CurrentToken.Type != TokenType.EndOfFile && CurrentToken.Type != TokenType.RightCurlyBracket)
            {
                var current = CurrentToken;
                output.Add(ParseStatement());
                if (current.Equals(CurrentToken)) break;
            }
            Next();

            return new StatementBlock(output);
        }

        private Statement ParseStatement()
        {
            return CurrentToken.Type switch
            {
                TokenType.Global or TokenType.Local => ParseVarAssignment(),
                TokenType.Run => ParseRunStatement(),
                TokenType.LeftCurlyBracket => ParseStatementBlock(),
                TokenType.Return => ParseReturnStatement(),
                TokenType.Repeat => ParseRepeatStatement(),
                TokenType.Loop => ParseLoopStatement(),
                TokenType.If => ParseIfStatement(),
                TokenType.Else => ParseLoneElseStatement(),
                TokenType.Identifier or TokenType.AsName => ParseAmbiguousDeclaration(),
                TokenType.Dispose => ParseDisposeStatement(),
                TokenType.SemiColon => ParseEmptyStatement(),
                _ => ParseExpressionStatement(),
            };
        }

        private Statement ParseEmptyStatement()
        {
            var token = Next();
            return new ExpressionStatement(
                token.Position, 
                new LiteralNode(
                    new() 
                    { 
                        Position = token.Position, Type = TokenType.Number, Data = "0"
                    }
                    )
                );
        }
        private Statement ParseDisposeStatement()
        {
            var position = ExpectThenNext(TokenType.Dispose).Position;
            var disposer = ParsePrimaryExpression();
            TryGetNext(TokenType.SemiColon);
            return new DisposeStatement(position, disposer);
        }

        private Statement ParseAmbiguousDeclaration()
        {
            var position = CurrentToken.Position;
            var identifier = ParsePrimaryExpression();
            if (Peek(0).Type != TokenType.Assignment) // Filter out any regular expression statements
            {
                var expression = ParseBinaryExpression(identifier);
                TryGetNext(TokenType.SemiColon);
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
            TryGetNext(TokenType.SemiColon);
            return new RunStatement(position, relativeLocation);

        }

        /// <summary>
        /// The if statement function already integrates else statements, but the user should be able to just run an else statement no matter what.
        /// </summary>
        /// <param name="scoped"></param>
        /// <returns></returns>
        private Statement ParseLoneElseStatement()
        {
            ExpectThenNext(TokenType.Else);
            ExpectThenNext(TokenType.Assignment);
            return ParseStatement();
        }

        private Statement ParseIfStatement()
        {
            var position = ExpectThenNext(TokenType.If).Position;
            var condition = ParseBinaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var ifStatement = ParseStatement();
            Statement elseStatement = new StatementBlock(new List<Statement>());
            if (CurrentToken.Type == TokenType.Else)
            {
                Next();
                ExpectThenNext(TokenType.Assignment);
                elseStatement = ParseStatement();
            }

            return new IfStatement(position, ifStatement, elseStatement, condition);

        }


        private Statement ParseLoopStatement()
        {
            var position = ExpectThenNext(TokenType.Loop).Position;
            var condition = ParseBinaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement();

            return new LoopStatement(position, statement, condition);

        }

        private Statement ParseRepeatStatement()
        {
            var position = ExpectThenNext(TokenType.Repeat).Position;
            var amount = ParseBinaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var statement = ParseStatement();

            return new RepeatStatement(position, statement, amount);

        }

        private Statement ParseVarAssignment()
        {
            var globality = Next();
            var identifier = ParsePrimaryExpression();
            ExpectThenNext(TokenType.Assignment);
            var assignment = ParseBinaryExpression();
            TryGetNext(TokenType.SemiColon);
            return new AssignmentStatement(identifier.Position, identifier, assignment, globality.Type == TokenType.Global);
        }

        private Statement ParseReturnStatement()
        {
            var position = ExpectThenNext(TokenType.Return).Position;
            var expression = ParseBinaryExpression();
            TryGetNext(TokenType.SemiColon);
            return new ReturnStatement(position, expression);
        }

       
        private Statement ParseExpressionStatement()
        {
            var position = CurrentToken.Position;
            var expression = ParseBinaryExpression();
            TryGetNext(TokenType.SemiColon);
            return new ExpressionStatement(position, expression);
        }
    }
}
