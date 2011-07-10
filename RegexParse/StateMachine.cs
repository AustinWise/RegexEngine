using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace RegexParse
{
    abstract class StateMachine<T> where T : State
    {
        const char LAMBDA = 'λ';

        protected StateMachine()
        {
            this.States = new List<T>();
            this.Transitions = new List<Tuple<T, char?, T>>();
        }

        public State StartState { get; protected set; }
        public List<T> States { get; private set; }
        public List<Tuple<T, char?, T>> Transitions { get; private set; }

        public void ToGraphViz(TextWriter tw)
        {
            tw.WriteLine("digraph G {");

            tw.WriteLine("\"*start*\" [shape = circle, style=filled, color=black, label = \"\"];");

            foreach (var s in States)
            {
                tw.WriteLine("\"{0}\"{1};", s.Name, s.IsFinal ? " [shape = doublecircle]" : "");
            }

            foreach (var t in Transitions)
            {
                tw.WriteLine("\"{0}\" -> \"{2}\" [label = \"{1}\"];", t.Item1.Name, t.Item2.HasValue ? t.Item2.Value : LAMBDA, t.Item3.Name);
            }
            tw.WriteLine("\"{0}\" -> \"{2}\" [label = \"{1}\"];", "*start*", LAMBDA, StartState.Name);

            tw.WriteLine("}");
        }

        /// <summary>
        /// A map of all the states that can be reached from state without consuming input.
        /// </summary>
        /// <returns></returns>
        public Dictionary<State, List<State>> LambdaClosure()
        {
            var map = new Dictionary<State, List<State>>();

            foreach (var state in States)
            {
                map.Add(state, LambdaClosure(state));
            }

            return map;
        }

        private Dictionary<State, List<State>> mLambdasMemo = new Dictionary<State, List<State>>();
        /// <summary>
        /// A memoized recurive version.
        /// </summary>
        /// <param name="state"></param>
        private List<State> LambdaClosure(State state)
        {
            if (mLambdasMemo.ContainsKey(state))
                return new List<State>(mLambdasMemo[state]);

            var ret = mLambdasMemo[state] = new List<State>();
            ret.Add(state);

            foreach (var st in Transitions.Where(t => t.Item1 == state && !t.Item2.HasValue))
            {
                foreach (var lambdaState in LambdaClosure(st.Item3))
                {
                    if (!ret.Contains(lambdaState))
                        ret.Add(lambdaState);
                }
            }

            return ret;
        }

        public Dictionary<Tuple<State, char>, List<State>> TFunction()
        {
            var ret = new Dictionary<Tuple<State, char>, List<State>>();
            foreach (var startState in States)
            {
                foreach (var st in LambdaClosure(startState))
                {
                    foreach (var t in Transitions.Where(t => t.Item1 == st && t.Item2.HasValue))
                    {
                        var tup = new Tuple<State, char>(startState, t.Item2.Value);
                        var list = new List<State>();
                        foreach (var destState in LambdaClosure(t.Item3))
                        {
                            list.Add(destState);
                        }
                        ret[tup] = list;
                    }
                }
            }
            return ret;
        }
    }

    class State : IEquatable<State>, IComparable<State>
    {
        private static int sCounter = 0;

        public State()
            : this(string.Format("g{0}", Interlocked.Increment(ref sCounter)))
        {
        }

        public State(string name)
        {
            this.Name = name;
            this.IsFinal = false;
        }

        public string Name { get; private set; }
        public bool IsFinal { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State))
                return false;
            return Equals((State)obj);
        }

        public bool Equals(State other)
        {
            return this.Name.Equals(other.Name) && this.IsFinal == other.IsFinal;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ (IsFinal ? int.MaxValue : 0x0);
        }

        public int CompareTo(State other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
