using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexParse
{
    class NFA : StateMachine<State>
    {
        public NFA(AstNode node)
        {
            var startAndStop = node.Visit(new NfaVistor(this));
            this.StartState = startAndStop.Item1;
            startAndStop.Item2.IsFinal = true;
        }

        class NfaVistor : IVisitor<Tuple<State, State>>
        {
            NFA nfa;
            public NfaVistor(NFA nfa)
            {
                this.nfa = nfa;
            }

            private void addTrans(State start, char val, State stop)
            {
                nfa.Transitions.Add(new Tuple<State, char?, State>(start, val, stop));
                if (!nfa.States.Contains(start))
                    nfa.States.Add(start);
                if (!nfa.States.Contains(stop))
                    nfa.States.Add(stop);
            }

            private void addLambda(State start, State stop)
            {
                nfa.Transitions.Add(new Tuple<State, char?, State>(start, null, stop));
                if (!nfa.States.Contains(start))
                    nfa.States.Add(start);
                if (!nfa.States.Contains(stop))
                    nfa.States.Add(stop);
            }

            public Tuple<State, State> VisitLiteral(LiteralNode lit)
            {
                var start = new State();
                var stop = new State();
                addTrans(start, lit.Value, stop);
                return new Tuple<State, State>(start, stop);
            }

            public Tuple<State, State> VisitKleen(KleeneStarNode kle)
            {
                var start = new State();
                var stop = new State();
                var child = kle.Child.Visit(this);
                addLambda(start, child.Item1);
                addLambda(child.Item2, stop);
                addLambda(stop, start);
                addLambda(start, stop);
                return new Tuple<State, State>(start, stop);
            }

            public Tuple<State, State> VisitAlternation(AlternationNode alt)
            {
                throw new NotSupportedException("You should use the SimplifyingRewriter in Program.");
            }

            public Tuple<State, State> VisitConcat(ConcatNode con)
            {
                var start = new State();
                var stop = new State();

                var cur = con.Nodes[0].Visit(this);
                addLambda(start, cur.Item1);

                for (int i = 1; i < con.Nodes.Count; i++)
                {
                    var next = con.Nodes[i].Visit(this);
                    addLambda(cur.Item2, next.Item1);
                    cur = next;
                }

                addLambda(cur.Item2, stop);

                return new Tuple<State, State>(start, stop);
            }

            public Tuple<State, State> VisitAlternationList(AlternationListNode alt)
            {
                if (alt.Nodes.Count == 0)
                    throw new Exception("This should never happen, but if it did the start and stop starts would not get added to the list.");

                var start = new State();
                var stop = new State();
                foreach (var ch in alt.Nodes.Select(n => n.Visit(this)))
                {
                    addLambda(start, ch.Item1);
                    addLambda(ch.Item2, stop);
                }
                return new Tuple<State, State>(start, stop);
            }
        }
    }
}
