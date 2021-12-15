using C_Double_Flat.Core.Parser;
using C_Double_Flat.Core.Runtime;
using C_Double_Flat.Core.Utilities;
using NUnit.Framework;
namespace C_Double_Flat_Testing
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void StringParsing()
        {
            Lexer.Tokenize("\"String test\"");

            Assert.Throws<TerminatingStringException>(() => { Lexer.Tokenize("\"nonending"); });
        }

        [Test]
        public void NumericalParsing()
        {
            Lexer.Tokenize("0 0.1 0.2 233.4");

            Assert.Throws<NumberFormattingException>(() => { Lexer.Tokenize("0.0.1"); });
        }

        [Test]
        public void KeywordParsing()
        {
            var toks = Lexer.Tokenize("if else loop repeat asname return run global local");

            Assert.True(toks[0].Type == TokenType.If);
            Assert.True(toks[1].Type == TokenType.Else);
            Assert.True(toks[2].Type == TokenType.Loop);
            Assert.True(toks[3].Type == TokenType.Repeat);
            Assert.True(toks[4].Type == TokenType.AsName);
            Assert.True(toks[5].Type == TokenType.Return);
            Assert.True(toks[6].Type == TokenType.Run);
            Assert.True(toks[7].Type == TokenType.Global);
            Assert.True(toks[8].Type == TokenType.Local);

        }

        [Test]
        public void ExpressionInterpreting()
        {
            Interpreter.Interpret(Parser.Parse(Lexer.Tokenize("")), "");
        }
    }
}