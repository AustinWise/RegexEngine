using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RegexParse
{
    class CSharpOutput
    {
        StringBuilder sb = new StringBuilder();

        public CSharpOutput(DFA dfa)
        {
            var map = new Dictionary<State, int>();
            int counter = 0;
            foreach (var st in dfa.States)
            {
                map[st] = counter++;
            }

            sb.AppendLine("namespace TestNS { static class Test { public static bool Match(System.IO.TextReader tr) {");
            sb.AppendLine("int ch;");
            sb.AppendFormat("goto state{0};", map[dfa.StartState]).AppendLine();

            foreach (var st in dfa.States)
            {
                sb.AppendFormat("state{0}: {{", map[st]).AppendLine();
                sb.AppendLine("\tch = tr.Read();");
                foreach (var t in dfa.Transitions.Where(t => t.Item1 == st))
                {
                    sb.AppendFormat("\tif (ch == '{0}')", t.Item2.Value).AppendLine();
                    sb.AppendFormat("\t\tgoto state{0};", map[t.Item3]).AppendLine();
                }
                sb.AppendFormat("\treturn {0};", st.IsFinal ? "ch == -1" : "false").AppendLine();
                sb.AppendLine("}");
            }

            sb.AppendLine("}}}");
        }

        private bool test(TextReader tr)
        {
            int ch;

            goto state1;

        state1:
            {
                ch = tr.Read();
                if (ch == 'a')
                    goto state2;
                return ch == -1;
            }
        state2:
            {
                ch = tr.Read();
                if (ch == 'b')
                    goto state1;
                return false;
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
