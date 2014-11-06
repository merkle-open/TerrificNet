using System.Collections.Generic;

namespace TerrificNet.Mustache
{
    public class Parser
    {
        private readonly Scanner _scanner;

        public Parser(Scanner scanner)
        {
            _scanner = scanner;
        }

        public MustacheNode Parse(string content)
        {
            MustacheBlock currentTemplate = new MustacheTemplate();
            var stack = new Stack<MustacheBlock>();
            stack.Push(currentTemplate);
            foreach (var literal in _scanner.Scan(content))
            {
                currentTemplate = stack.Peek();

                if (literal.Type == Scanner.TokenType.Text)
                    currentTemplate.Children.Add(new MustacheLiteralNode(literal.Content));
                else if (literal.Type == Scanner.TokenType.VariableReference)
                    currentTemplate.Children.Add(new MustacheStatementNode(literal.Content));
                else if (literal.Type == Scanner.TokenType.BlockStart)
                {
                    var block = new MustacheBlock(literal.Content);
                    currentTemplate.Children.Add(block);
                    stack.Push(block);
                }
                else if (literal.Type == Scanner.TokenType.EndSection)
                {
                    stack.Pop();
                }
            }

            return currentTemplate;
        }
    }
}