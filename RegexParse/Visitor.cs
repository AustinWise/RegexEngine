using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexParse
{
    interface IVisitor<T>
    {
        T VisitLiteral(LiteralNode lit);

        T VisitKleen(KleeneStarNode kle);

        T VisitAlternation(AlternationNode alt);

        T VisitConcat(ConcatNode con);

        T VisitAlternationList(AlternationListNode alt);
    }

    class Visitor<T> : IVisitor<T>
    {
        public virtual T VisitLiteral(LiteralNode lit)
        {
            return default(T);
        }

        public virtual T VisitKleen(KleeneStarNode kle)
        {
            return default(T);
        }

        public virtual T VisitAlternation(AlternationNode alt)
        {
            return default(T);
        }

        public virtual T VisitConcat(ConcatNode con)
        {
            return default(T);
        }

        public T VisitAlternationList(AlternationListNode alt)
        {
            return default(T);
        }
    }

    class Rewriter : IVisitor<AstNode>
    {
        public virtual AstNode VisitLiteral(LiteralNode lit)
        {
            return lit;
        }

        public virtual AstNode VisitKleen(KleeneStarNode kle)
        {
            return new KleeneStarNode(kle.Child.Visit(this));
        }

        public virtual AstNode VisitAlternation(AlternationNode alt)
        {
            return new AlternationNode(alt.Left.Visit(this), alt.Right.Visit(this));
        }

        public virtual AstNode VisitConcat(ConcatNode con)
        {
            return new ConcatNode(con.Nodes.Select(n => n.Visit(this)));
        }

        public AstNode VisitAlternationList(AlternationListNode alt)
        {
            return new AlternationListNode(alt.Nodes.Select(n => n.Visit(this)));
        }
    }
}
