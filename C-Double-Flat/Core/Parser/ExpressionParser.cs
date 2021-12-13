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
        private ExpressionNode ParseBinaryExpression(int parentPrecedence = 0)
        {
            // Thanks Kirill Osenkov you are amazing
            ExpressionNode left = ParsePrimaryExpression();

            while (true)
            {

                int precedence = CurrentToken.GetPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                Token operation = Next();
                ExpressionNode right = ParseBinaryExpression(precedence);

                left = new BinaryOperationNode(left, operation, right);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            ExpressionNode output;
            switch (CurrentToken.Type)
            {
                case TokenType.Number:
                case TokenType.String:
                case TokenType.Boolean:
                case TokenType.Identifier:
                    output = ParseIdentifierCall();
                    break;
                case TokenType.LeftParenthesis:
                    output = ParseParenthesizedExpression();
                    break;
                case TokenType.LeftSquareBracket:
                    output = ParseCollectionExpression();
                    break;
                case TokenType.AsName:
                    output =  ParseAsName();
                    break;
                case TokenType.Minus:
                    var minusPos = Next().Position;
                    return new BinaryOperationNode(
                        new LiteralNode(
                            new Token(minusPos, TokenType.Number, "-1")),
                            new Token(CurrentToken.Position, TokenType.Multiply),
                            ParsePrimaryExpression());
                default:
                    throw new ExpectedTokenException(CurrentToken.Position, CurrentToken.Type);

            }

            // Check if this primary node is a array assignment
            while (CurrentToken.Type == TokenType.Insertion && Peek(1).Type == TokenType.LeftSquareBracket)
            {
                ExpectThenNext(TokenType.Insertion);
                ExpectThenNext(TokenType.LeftSquareBracket);
                var location = ParseBinaryExpression();
                ExpectThenNext(TokenType.RightSquareBracket);
                output = new CollectionCallNode(output.Position, output, location);
            }
            return output;
        }

        private ExpressionNode ParseAsName()
        {
            var position = ExpectThenNext(TokenType.AsName).Position;
            ExpectThenNext(TokenType.LeftParenthesis);
            var identifier = ParseBinaryExpression();
            ExpectThenNext(TokenType.RightParenthesis);

            // If after asname, could be collection call or function call through asname
            if (CurrentToken.Type == TokenType.Insertion && Peek().Type == TokenType.LeftParenthesis) 
            {
                
                    Next(); Next();
                    var parameters = ParseExpressionList();
                    ExpectThenNext(TokenType.RightParenthesis);
                    return new FunctionCallNode(position, identifier, parameters);
                
            }
            return new AsNameNode(position, identifier);
        }

        private ExpressionNode ParseCollectionExpression()
        {
            var position = ExpectThenNext(TokenType.LeftSquareBracket).Position;
            var elements = ParseExpressionList();
            ExpectThenNext(TokenType.RightSquareBracket);
            return new CollectionLiteralNode(position, elements);
        }

        private ExpressionNode ParseParenthesizedExpression()
        {
            ExpectThenNext(TokenType.LeftParenthesis);
            var expression = ParseBinaryExpression();
            ExpectThenNext(TokenType.RightParenthesis);
            return expression;
        }

        private ExpressionNode ParseIdentifierCall()
        {
            if (Peek(1).Type == TokenType.Insertion)
            {
                if (Peek(2).Type == TokenType.LeftParenthesis) 
                    return ParseIdentifierFunctionCall();
            }
            return new LiteralNode(Next());
        }

        private ExpressionNode ParseIdentifierFunctionCall()
        {
            var caller = ExpectThenNext(TokenType.Identifier);
            ExpectThenNext(TokenType.Insertion);
            ExpectThenNext(TokenType.LeftParenthesis);
            var args = ParseExpressionList();
            ExpectThenNext(TokenType.RightParenthesis);
            return new FunctionCallNode(caller.Position, new LiteralNode(caller), args);
        }


        private List<ExpressionNode> ParseExpressionList() // TODO: fix this, not worky.
        {
            List<ExpressionNode> output = new();
            bool parseNextArgument = true;
            while (CurrentToken.Type != TokenType.RightSquareBracket &&
                   CurrentToken.Type != TokenType.RightParenthesis &&
                   CurrentToken.Type != TokenType.EndOfFile &&
                   parseNextArgument)
            {

                output.Add(ParseBinaryExpression());
                if (CurrentToken.Type == TokenType.Comma) Next();
                else parseNextArgument = false;
            }
            return output;
        }
    }
}
