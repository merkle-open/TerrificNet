using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TerrificNet.Mustache.Antlr
{
	public class MustacheTemplateEngine
	{
		private const int TokenTemplate = 0;
		private const int TokenBody = 1;
		private const int TokenStatement = 2;
		private const int TokenText = 4;
		private const int TokenSpaces = 5;
		private const int TokenBlock = 7;
		private const int TokenSetExpression = 8;
		private const int TokenVariable = 13;
		private const int TokenParameter = 16;

		public Func<object, string> Compile(string template)
		{
			var input = new AntlrInputStream(template);
			var lexer = new MustacheLexer(input);
			ITokenStream tokens = new CommonTokenStream(lexer);
			var parser = new MustacheParser(tokens);
			IParseTree tree = parser.template();

			string[] ruleNames = parser.RuleNames;

			var token = ChildHandler(tree);

			//Console.WriteLine(token.Print(0));

			//Console.WriteLine("---------");
			//Print(tree, ruleNames, 0);

			return o =>
			{
				var sb = new StringBuilder();
				token.Execute(sb, o);
				return sb.ToString();
			};
		}

		private static IStatementToken ChildHandler(ITree node)
		{
			var context = new List<IStatementToken>();

			IStatementToken lastToken = null;
			for (var i = 0; i < node.ChildCount; i++)
			{
				var child = node.GetChild(i);
				IStatementToken currentToken = null;

				if (child is IRuleNode)
				{
					var ruleNode = (IRuleNode)child;
					switch (ruleNode.RuleContext.RuleIndex)
					{
						case TokenTemplate:
						case TokenBody:
							currentToken = ChildHandler(child);
							break;
						case TokenStatement:
							currentToken = StatementHandler(child);
							break;
						default:
							Console.WriteLine(ruleNode.RuleContext.RuleIndex + " not found");
							break;
					}
				}

				if (currentToken == null)
					continue;

				if (lastToken == null || !lastToken.TryAppend(currentToken))
				{
					lastToken = currentToken;
					context.Add(currentToken);
				}
			}

			return context.Count > 1 ? new CollectionStatementToken(context) : context.FirstOrDefault();
		}

		private static IStatementToken StatementHandler(ITree node)
		{
			var context = new List<IStatementToken>();

			IStatementToken lastToken = null;
			for (var i = 0; i < node.ChildCount; i++)
			{
				var currentToken = StatementTokenHandler(node.GetChild(i));
				if (currentToken == null)
					continue;

				if (lastToken == null || !lastToken.TryAppend(currentToken))
				{
					lastToken = currentToken;
					context.Add(currentToken);
				}
			}

			return context.Count > 1 ? new CollectionStatementToken(context) : context.FirstOrDefault();
		}

		private static IStatementToken BlockHandler(ITree node)
		{
			var setExpression = node.SelectRuleChild(TokenSetExpression);
			var body = node.SelectRuleChild(TokenBody);

			if (setExpression != null)
			{
				return ExpressionTokenHandler(setExpression, ChildHandler(body));
			}

			return null;
		}

		private static IStatementToken ExpressionTokenHandler(ITree node, IStatementToken body = null)
		{
			var keyword = node.GetTerminalNodeValue(0);
			switch (keyword)
			{
				case "if":
					var parameter = node.SelectRuleChild(TokenParameter).GetTerminalNodeValue(0);
					var expression = new ExpressionStatementToken(parameter);
					return new IfStatementToken(expression, body);
			}

			return new ExpressionStatementToken(keyword);
		}

		private class ExpressionStatementToken : IStatementToken
		{
			public string Value { get; private set; }

			public ExpressionStatementToken(string value)
			{
				Value = value;
			}

			public bool TryAppend(IStatementToken token)
			{
				return false;
			}

			public string Print(int level)
			{
				return string.Format("{0}(Expression={1})", new String(' ', level), Value);
			}

			public void Execute(StringBuilder writer, object context)
			{
			}

			public object Evaluate(object context)
			{
				return GetDeepPropertyValue(context, Value);
			}

			private static object GetDeepPropertyValue(object instance, string path)
			{
				var pp = path.Split('.');
				var t = instance.GetType();
				foreach (var prop in pp)
				{
					var propInfo = t.GetProperty(prop);
					if (propInfo != null)
					{
						instance = propInfo.GetValue(instance, null);
						t = propInfo.PropertyType;
					}
					else throw new ArgumentException("Properties path is not correct");
				}
				return instance;
			}
		}

		private static IStatementToken StatementTokenHandler(ITree node)
		{
			if (node is IRuleNode)
			{
				var ruleNode = (IRuleNode)node;
				switch (ruleNode.RuleContext.RuleIndex)
				{
					case TokenText:
						if (node.ChildCount != 1)
							throw new Exception("0 or more than 1 child nodes");

						var child = node.GetChild(0);
						if (child is ITerminalNode)
						{
							var symbol = ((ITerminalNode)child).Symbol;
							if (symbol != null)
							{
								var s = symbol.Text;
								return new TextStatementToken(s);
							}
						}
						throw new Exception("non terminal under text ist not permitted");
					case TokenSpaces:
						return new TextStatementToken(" ");
					case TokenBlock:
						return BlockHandler(node);
					case TokenVariable:
						var expressionNode = ruleNode.SelectRuleChild(TokenSetExpression);
						return new VariableStatementToken((ExpressionStatementToken)ExpressionTokenHandler(expressionNode));
					default:
						Console.WriteLine(ruleNode.RuleContext.RuleIndex + " not found");
						break;
				}
			}
			return null;
		}

		private interface IStatementToken
		{
			bool TryAppend(IStatementToken token);
			string Print(int level);
			void Execute(StringBuilder writer, object context);
		}

		private class VariableStatementToken : IStatementToken
		{
			public ExpressionStatementToken Expression { get; private set; }

			public VariableStatementToken(ExpressionStatementToken expression)
			{
				Expression = expression;
			}

			public bool TryAppend(IStatementToken token)
			{
				return false;
			}

			public string Print(int level)
			{
				return string.Format("{0}[Var] {1}", new string(' ', level), Expression.Print(0));
			}

			public void Execute(StringBuilder writer, object context)
			{
				var value = Expression.Evaluate(context);
				writer.Append(value);
			}
		}

		private class IfStatementToken : IStatementToken
		{
			public ExpressionStatementToken Expression { get; private set; }
			public IStatementToken Body { get; private set; }

			public IfStatementToken(ExpressionStatementToken expression, IStatementToken body)
			{
				Expression = expression;
				Body = body;
			}

			public bool TryAppend(IStatementToken token)
			{
				return false;
			}

			public string Print(int level)
			{
				var sb = new StringBuilder();
				sb.AppendLine(string.Format("{0}[IF {1}]", new String(' ', level), Expression.Print(0)));
				sb.AppendLine(Body.Print(level + 1));
				return sb.ToString();
			}

			public void Execute(StringBuilder writer, object context)
			{
				var value = Expression.Evaluate(context);
				if ((value is bool && (bool)value) || (!(value is bool) && value != null))
					Body.Execute(writer, context);
			}
		}

		private class CollectionStatementToken : IStatementToken
		{
			private readonly IEnumerable<IStatementToken> _tokens;

			public CollectionStatementToken(IEnumerable<IStatementToken> tokens)
			{
				_tokens = tokens;
			}

			public bool TryAppend(IStatementToken token)
			{
				return false;
			}

			public string Print(int level)
			{
				var sb = new StringBuilder();
				foreach (var token in _tokens)
				{
					sb.AppendLine(token.Print(level + 1));
				}
				return sb.ToString();
			}

			public void Execute(StringBuilder writer, object context)
			{
				foreach (var token in _tokens)
				{
					token.Execute(writer, context);
				}
			}
		}

		private class TextStatementToken : IStatementToken
		{
			private readonly StringBuilder _value;

			public string Value { get { return _value.ToString(); } }

			public TextStatementToken(string value)
			{
				_value = new StringBuilder(value);
			}

			public bool TryAppend(IStatementToken token)
			{
				var textStatement = token as TextStatementToken;
				if (textStatement != null)
				{
					_value.Append(textStatement.Value);
					return true;
				}
				return false;
			}

			public string Print(int level)
			{
				return string.Format("{0}[Text] {1}", new String(' ', level), Value);
			}

			public void Execute(StringBuilder writer, object context)
			{
				writer.Append(Value);
			}
		}

		private static void Print(ITree node, string[] ruleNames, int level)
		{
			var nodeText = GetNodeText(node, ruleNames);
			Console.WriteLine("{0}{1}", new String(' ', level), nodeText);

			for (var i = 0; i < node.ChildCount; i++)
			{
				Print(node.GetChild(i), ruleNames, level + 1);
			}
		}

		private static string GetNodeText(ITree t, string[] ruleNames)
		{
			if (ruleNames != null)
			{
				if (t is IRuleNode)
				{
					var ruleIndex = ((IRuleNode)t).RuleContext.RuleIndex;
					var ruleName = ruleNames[ruleIndex];
					return string.Format("{0} ({1})", ruleName, ruleIndex);
				}
				if (t is IErrorNode)
				{
					return t.ToString();
				}
				if (t is ITerminalNode)
				{
					var symbol = ((ITerminalNode)t).Symbol;
					if (symbol != null)
					{
						var s = symbol.Text;
						return string.Format("{0} (terminal)", s);
					}
				}
			}
			// no recog for rule names
			var payload = t.Payload;
			if (payload is IToken)
			{
				return ((IToken)payload).Text;
			}
			return t.Payload.ToString();
		}
	}

	static class AntlrExtension
	{
		public static ITree SelectRuleChild(this ITree node, int ruleIndex)
		{
			for (int i = 0; i < node.ChildCount; i++)
			{
				var child = node.GetChild(i);

				if (child is IRuleNode)
				{
					var index = ((IRuleNode)child).RuleContext.RuleIndex;
					if (index == ruleIndex)
						return child;
				}
			}
			return null;
		}

		public static string GetTerminalNodeValue(this ITree node, int index)
		{
			var child = node.GetChild(index);
			if (child is ITerminalNode)
			{
				var symbol = ((ITerminalNode)child).Symbol;
				if (symbol != null)
				{
					return symbol.Text;
				}
			}
			return null;
		}
	}
}
