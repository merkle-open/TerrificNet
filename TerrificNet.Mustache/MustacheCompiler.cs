
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
