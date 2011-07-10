using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RegexParse
{
    class DFA : StateMachine<NfaSourcedState>
    {
        Stack<NfaSourcedState> worklist = new Stack<NfaSourcedState>();
        public DFA(NFA nfa)
        {
            var lc = nfa.LambdaClosure();
            var tf = nfa.TFunction();

            StartState = MakeState(new SortedSet<State>(lc[nfa.StartState]));

            while (worklist.Count != 0)
            {
                var state = worklist.Pop();

                foreach (var kvp in tf.Where(t => state.SubStates.Contains(t.Key.Item1)).GroupBy(t => t.Key.Item2))
                {
                    var key = kvp.Key;
                    var val = kvp.SelectMany(t => t.Value);

                    var st = MakeState(val);

                    this.Transitions.Add(new Tuple<NfaSourcedState, char?, NfaSourcedState>(state, key, st));
                }
            }

            foreach (var state in States)
            {
                if (state.SubStates.Any(s => s.IsFinal))
                    state.IsFinal = true;
            }
        }

        private NfaSourcedState MakeState(IEnumerable<State> states)
        {
            return MakeState(new SortedSet<State>(states));
        }

        private NfaSourcedState MakeState(SortedSet<State> states)
        {
            var q = string.Join(", ", (from s in states select s.Name).ToArray());
            var name = "{" + q + "}";
            var state = this.States.Where(s => s.Name == name).SingleOrDefault();
            if (state != null)
                return state;

            var newState = new NfaSourcedState(name, states);
            States.Add(newState);
            worklist.Push(newState);
            return newState;
        }

        public bool Evaluate(string input)
        {
            return Evaluate(new StringReader(input));
        }

        public bool Evaluate(TextReader input)
        {
            var state = StartState;

            while (input.Peek() != -1)
            {
                var trans = Transitions.Where(t => t.Item1 == state && t.Item2.Value == input.Peek()).SingleOrDefault();

                if (trans == null)
                    break;

                input.Read();
                state = trans.Item3;
            }

            return input.Peek() == -1 && state.IsFinal;
        }
    }

    class NfaSourcedState : State
    {
        public NfaSourcedState(string name, ISet<State> substates)
            : base(name)
        {
            this.SubStates = substates;
        }

        public ISet<State> SubStates { get; private set; }
    }
}
