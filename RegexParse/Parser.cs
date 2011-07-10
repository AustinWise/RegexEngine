using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexParse
{
    class Parser
    {
        private readonly Scanner mSc;
        private readonly AstNode mResult;

        public Parser(Scanner sc)
        {
            if (sc == null)
                throw new ArgumentNullException();
            mSc = sc;
            mResult = Parse();
            Match<TkEof>();
        }

        public AstNode Result
        {
            get
            {
                return mResult;
            }
        }

        private void Match<T>() where T : Token
        {
            var next = mSc.Next();
            if (!(next is T))
                throw new Exception(string.Format("Wanted {0} but got {1}.", typeof(T).Name, next.Name));
        }

        private AstNode Parse()
        {
            List<AstNode> nodes = new List<AstNode>();

            while (!mSc.Current.IsEof)
            {
                if (mSc.Current is TkChar)
                    nodes.Add(new LiteralNode(mSc.Next<TkChar>().Value));
                else if (mSc.Current is TkLParen)
                {
                    Match<TkLParen>();
                    nodes.Add(Parse());
                    Match<TkRParen>();
                }
                else if (mSc.Current is TkRParen)
                    return new ConcatNode(nodes);
                else if (mSc.Current is TkPipe)
                {
                    Match<TkPipe>();
                    return new AlternationNode(new ConcatNode(nodes), Parse());
                }
                else if (mSc.Current is TkStar)
                {
                    Match<TkStar>();
                    if (nodes.Count == 0)
                        throw new Exception("Kleen star needs to be proceeded by something.");
                    var val = nodes[nodes.Count - 1];
                    nodes.Remove(val);
                    nodes.Add(new KleeneStarNode(val));
                }
                else
                    throw new InvalidOperationException("Unknown token.");
            }

            return new ConcatNode(nodes);
        }
    }
}
