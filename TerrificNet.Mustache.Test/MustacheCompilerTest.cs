using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TerrificNet.Mustache.Test
{
    [TestClass]
    public class MustacheCompilerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var scanner = new Scanner();
            var parser = new Parser(scanner);

            var input = "<html>{{gugus}}" +
                                             "{{#if soso}}" +
                                             "{{hallo}}" +
                                             "{{/if}}</html>";

            var result = parser.Parse(input);

            var sb = new StringBuilder();

            result.Accept(new StringBuilderVisitor(sb));

            var resultText = sb.ToString();

            Assert.AreEqual(input, resultText);
        }

        private class StringBuilderVisitor : IMustacheNodeVisitor
        {
            private readonly StringBuilder _builder;

            public StringBuilderVisitor(StringBuilder builder)
            {
                _builder = builder;
            }

            public void VisitBlockBegin(MustacheBlock block)
            {
                _builder.AppendFormat("{{{{#{0}}}}}", block.Statement);
            }

            public void VisitBlockEnd(MustacheBlock block)
            {
                _builder.AppendFormat("{{{{#{0}}}}}", block.Statement);
            }

            public void VisitStatement(MustacheStatementNode statement)
            {
                _builder.AppendFormat("{{{{{0}}}}}", statement.Statement);
            }

            public void VisitLiteral(MustacheLiteralNode literal)
            {
                _builder.Append(literal.Text);
            }
        }
    }
}
