using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexParse
{
    abstract class AstNode
    {
        protected AstNode()
        {
            this.Name = this.GetType().Name;
            this.Name = this.Name.Remove(Name.LastIndexOf("Node"));
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public abstract T Visit<T>(IVisitor<T> v);
    }

    class LiteralNode : AstNode
    {
        public LiteralNode(char val)
        {
            this.Value = val;
        }

        public char Value { get; private set; }

        public override T Visit<T>(IVisitor<T> v)
        {
            return v.VisitLiteral(this);
        }
    }

    class KleeneStarNode : AstNode
    {
        public KleeneStarNode(AstNode child)
        {
            this.Child = child;
        }

        public AstNode Child { get; private set; }

        public override T Visit<T>(IVisitor<T> v)
        {
            return v.VisitKleen(this);
        }
    }

    class AlternationNode : AstNode
    {
        public AlternationNode(AstNode left, AstNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        public AstNode Left { get; private set; }
        public AstNode Right { get; private set; }

        public override T Visit<T>(IVisitor<T> v)
        {
            return v.VisitAlternation(this);
        }
    }

    class AlternationListNode : AstNode
    {
        public AlternationListNode(IEnumerable<AstNode> nodes)
        {
            this.Nodes = nodes.ToList().AsReadOnly();
        }

        public IList<AstNode> Nodes { get; private set; }

        public override T Visit<T>(IVisitor<T> v)
        {
            return v.VisitAlternationList(this);
        }
    }

    class ConcatNode : AstNode
    {
        public ConcatNode(IEnumerable<AstNode> nodes)
        {
            this.Nodes = nodes.ToList().AsReadOnly();
        }

        public IList<AstNode> Nodes { get; private set; }

        public override T Visit<T>(IVisitor<T> v)
        {
            return v.VisitConcat(this);
        }
    }
}
