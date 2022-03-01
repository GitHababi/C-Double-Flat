using C_Double_Flat.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace C_Double_Flat.Core.Parser
{
    public sealed class Lexer
    {
        /// <summary>
        /// Takes 'input' and returns array of tokens for use in Parsing or other debugging.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Token[] Tokenize(string input)
        {
            return new Lexer(input).Tokenize();
        }

        private Lexer(string input)
        {
            this.Input = input.ToCharArray();
            this.Position = new(1, 1, 0);
        }

        private readonly char[] Input;
        private Position Position;
        private char CurrentChar { get { return (Position.Index < Input.Length) ? Input[Position.Index] : default; } set { } }

        /// <summary>
        /// Returns list of tokens from an instantiated Lexer class, not to be called independently.
        /// </summary>
        /// <returns></returns>
        private Token[] Tokenize()
        {
            List<Token> tokens = new();
            Token defaultValue = new();

            while (CurrentChar != default)
            {
                Token token = GetNextToken();

                if (!token.Equals(defaultValue)) tokens.Add(token);
            }

            tokens.Add(defaultValue);

            return tokens.ToArray();
        }

        /// <summary>
        /// Returns next Token from the input stream, not to be called independently.
        /// </summary>
        /// <returns></returns>
        private Token GetNextToken()
        {

            Token output = new();


            switch (CurrentChar)
            {
                case '#':
                    CompleteComment();
                    break;
                case '&':
                    output = new(Position, TokenType.And);
                    break;
                case '|':
                    output = new(Position, TokenType.Or);
                    break;
                case '+':
                    output = new(Position, TokenType.Plus);
                    break;
                case '-':
                    output = new(Position, TokenType.Minus);
                    break;
                case '/':
                    output = new(Position, TokenType.Divide);
                    break;
                case '*':
                    output = new(Position, TokenType.Multiply);
                    break;
                case ',':
                    output = new(Position, TokenType.Comma);
                    break;
                case ':':
                    output = new(Position, TokenType.Assignment);
                    break;
                case ';':
                    output = new(Position, TokenType.SemiColon);
                    break;
                case '<':
                    if (Peek() == '-')
                    {
                        output = new(Position, TokenType.Insertion);
                        Advance();
                    }
                    else if (Peek() == '=')
                    {
                        output = new(Position, TokenType.LessThanOrEqual);
                        Advance();
                    }
                    else
                        output = new(Position, TokenType.LessThan);
                    break;
                case '>':
                    if (Peek() == '=')
                    {
                        output = new(Position, TokenType.GreaterThanOrEqual);
                        Advance();
                    }
                    else
                        output = new(Position, TokenType.GreaterThan);
                    break;
                case '!':
                    if (Peek() == '=')
                    {
                        output = new(Position, TokenType.NotEqual);
                        Advance(2);
                    }
                    else output = new(Position, TokenType.Not);
                    break;
                case '=':
                    output = new(Position, TokenType.Equal);
                    break;
                case '(':
                    output = new(Position, TokenType.LeftParenthesis);
                    break;
                case ')':
                    output = new(Position, TokenType.RightParenthesis);
                    break;
                case '[':
                    output = new(Position, TokenType.LeftSquareBracket);
                    break;
                case ']':
                    output = new(Position, TokenType.RightSquareBracket);
                    break;
                case '{':
                    output = new(Position, TokenType.LeftCurlyBracket);
                    break;
                case '}':
                    output = new(Position, TokenType.RightCurlyBracket);
                    break;
                case '\'':
                    output = TokenizeString('\'');
                    break;
                case '"':
                    output = TokenizeString('"');
                    break;
                default: // Dealing with special tokens. (numbers/identifiers/keywords)
                    if (String.IsNullOrWhiteSpace(CurrentChar.ToString()))
                        break;
                    else if (Char.IsDigit(CurrentChar))
                        output = TokenizeNumber();
                    else if (Regex.IsMatch(CurrentChar.ToString(), @"[&@$\w]"))
                        output = TokenizeWord();
                    else throw new InvalidTokenException(Position, CurrentChar);
                    break;
            }
            Advance();
            return output;
        }


        /// <summary>
        /// Completes string until next line, not to be called independently.
        /// </summary>
        private void CompleteComment()
        {

            while (CurrentChar != default && CurrentChar != '\n')
            {
                Advance();
            }

        }


        /// <summary>
        /// Returns a number token when fount in input, not to be called independently.
        /// </summary>
        /// <returns></returns>
        private Token TokenizeNumber()
        {
            Token output = new(Position, TokenType.Number);
            string accumulator = CurrentChar.ToString();
            int dotCount = 0;

            while (Peek() != default && (Char.IsDigit(Peek()) || Peek() == '.'))
            {
                Advance();
                if (CurrentChar == '.' && dotCount > 0) throw new NumberFormattingException(Position);
                else accumulator += CurrentChar;
                if (CurrentChar == '.') dotCount++;

            }

            output.Data = accumulator;

            return output;
        }

        /// <summary>
        /// Returns a token that is detected from a 'word' in the input stream, and will convert from identifier into special keywords if necessary.
        /// Not to be called independently.
        /// </summary>
        /// <returns></returns>
        private Token TokenizeWord()
        {
            Token output = new(Position, TokenType.Identifier);

            string accumulator = CurrentChar.ToString();

            while (Peek() != default && Regex.IsMatch(Peek().ToString(), @"[&@$\w]"))
            {
                Advance();
                accumulator += CurrentChar;
            }

            output.Data = accumulator;
            return accumulator switch
            {
                "if" => new(output.Position, TokenType.If),
                "else" => new(output.Position, TokenType.Else),
                "loop" => new(output.Position, TokenType.Loop),
                "repeat" => new(output.Position, TokenType.Repeat),
                "return" => new(output.Position, TokenType.Return),
                "run" => new(output.Position, TokenType.Run),
                "true" => new(output.Position, TokenType.Boolean, "true"),
                "false" => new(output.Position, TokenType.Boolean, "false"),
                "asname" => new(output.Position, TokenType.AsName),
                "global" => new(output.Position, TokenType.Global),
                "local" => new(output.Position, TokenType.Local),
                "dispose" => new(output.Position, TokenType.Dispose),
                _ => output
            };
        }

        /// <summary>
        /// Returns a string token when found in the input, not to be called independently.
        /// </summary>
        /// <returns></returns>
        private Token TokenizeString(char openDelim)
        {
            Token output = new(Position, TokenType.String, "");

            bool foundEnd = false;
            while (!foundEnd && CurrentChar != default)
            {
                Advance();
                switch (CurrentChar)
                {
                    case '^':
                        Advance();
                        output.Data += CurrentChar switch
                        {
                            't' => '\t',
                            'n' => '\n',
                            _ => CurrentChar
                        };
                        break;
                    case '\'':
                        if (openDelim == '\'')
                            foundEnd = true;
                        else
                            output.Data += CurrentChar;
                        break;
                    case '"':
                        if (openDelim == '"')
                            foundEnd = true;
                        else
                            output.Data += CurrentChar;
                        break;
                    default:
                        output.Data += CurrentChar;
                        break;
                }
            }

            // At this point output.Position holds the location of the start of the string.
            if (!foundEnd) throw new TerminatingStringException(output.Position);
            return output;
        }


        /// <summary>
        /// Moves to the next character or specified by 'amount' in the input sequence
        /// </summary>
        /// <param name="amount"></param>
        private void Advance(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (CurrentChar == '\n')
                {
                    Position.Row++;
                    Position.Column = 1;
                }
                else
                    Position.Column++;
                Position.Index++;
            }


        }

        /// <summary>
        /// Checks 'amount' characters ahead and returns it. Default 'amount' is 1.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private char Peek(int amount = 1)
        {

            if (Position.Index + amount < Input.Length)
                return Input[Position.Index + amount];
            else
                return default;
        }
    }
}
