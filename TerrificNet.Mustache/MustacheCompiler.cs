
using System.Collections.Generic;

namespace TerrificNet.Mustache
{
    public class MustacheCompiler
    {
        public MustacheCompiler()
        {
        }

        public void Compile()
        {
            var scanner = new Scanner();
            var parser = new Parser(scanner);

            parser.Parse("");
        }
    }

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

                if (literal.Type == Scanner.LiteralType.Text)
                    currentTemplate.Children.Add(new MustacheLiteralNode(literal.Content));
                else if (literal.Type == Scanner.LiteralType.VariableReference)
                    currentTemplate.Children.Add(new MustacheStatementNode(literal.Content));
                else if (literal.Type == Scanner.LiteralType.BlockStart)
                {
                    var block = new MustacheBlock(literal.Content);
                    currentTemplate.Children.Add(block);
                    stack.Push(block);
                }
                else if (literal.Type == Scanner.LiteralType.EndSection)
                {
                    stack.Pop();
                }
            }

            return currentTemplate;
        }
    }

    public interface IMustacheNodeVisitor
    {
        void VisitBlockBegin(MustacheBlock block);
        void VisitBlockEnd(MustacheBlock block);
        void VisitStatement(MustacheStatementNode statement);
        void VisitLiteral(MustacheLiteralNode literal);
    }

    public abstract class MustacheNode
    {
        public abstract void Accept(IMustacheNodeVisitor visitor);
    }

    public class MustacheTemplate : MustacheBlock
    {
    }

    public class MustacheBlock : MustacheNode
    {
        public string Statement { get; set; }
        public IList<MustacheNode> Children { get; set; }

        public MustacheBlock(string statement = null)
        {
            Statement = statement;
            Children = new List<MustacheNode>();
        }

        public override void Accept(IMustacheNodeVisitor visitor)
        {
            visitor.VisitBlockBegin(this);
            foreach (var child in Children)
                child.Accept(visitor);

            visitor.VisitBlockEnd(this);
        }
    }

    public class MustacheStatementNode : MustacheNode
    {
        public string Statement { get; set; }

        public MustacheStatementNode(string statement)
        {
            Statement = statement;
        }

        public override void Accept(IMustacheNodeVisitor visitor)
        {
            visitor.VisitStatement(this);
        }
    }

    public class MustacheVariableReference
    {
        public string[] Parts { get; set; }

        public MustacheVariableReference(string[] parts)
        {
            Parts = parts;
        }
    }

    public class MustacheLiteralNode : MustacheNode
    {
        public string Text { get; set; }

        public MustacheLiteralNode(string text)
        {
            Text = text;
        }

        public override void Accept(IMustacheNodeVisitor visitor)
        {
            visitor.VisitLiteral(this);
        }
    }
}
