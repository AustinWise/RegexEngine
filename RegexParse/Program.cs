using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RegexParse
{
    class Program
    {
        static void Main(string[] args)
        {
            var sc = new Scanner(new StringReader("asdf(2|3|4)*"));
            //TestScanner(sc);

            var parser = new Parser(sc);
            var tree = parser.Result.Visit(new SimplifyingRewriter());
            //tree.Visit(new TreePrinter());

            var nfa = new NFA(tree);
            TestNfa(nfa);

            var dfa = new DFA(nfa);
            TestDfa(dfa);

            var csharp = new CSharpOutput(dfa);
            Console.WriteLine(csharp);

            //var f = new IlOutput(dfa).ToFunc();

            //Console.WriteLine(f("asdf"));


            //Console.WriteLine(TestNS.Test.Match("asf"));
            //Console.WriteLine(new CSharpOutput(new DFA(new NFA(new Parser(new Scanner(new StringReader("asdf(2|3|4)*"))).Result.Visit(new SimplifyingRewriter())))));
        }

        private static void TestDfa(DFA dfa)
        {
            using (var tw = new StreamWriter(@"D:\Down\rtest\dfa.gv", false, Encoding.UTF8))
            {
                dfa.ToGraphViz(tw);
            }
            //Console.WriteLine(dfa.Evaluate("asdf2334"));
            //Console.WriteLine(dfa.Evaluate("v"));
            //Console.WriteLine(dfa.Evaluate(""));
            //Console.WriteLine(dfa.Evaluate("asdf2334 "));
        }

        private static void TestNfa(NFA nfa)
        {
            using (var tw = new StreamWriter(@"D:\Down\rtest\nfa.gv", false, Encoding.UTF8))
            {
                nfa.ToGraphViz(tw);
            }

            //foreach (var kvp in nfa.LambdaClosure())
            //{
            //    Console.WriteLine(kvp.Key);
            //    foreach (var s in kvp.Value)
            //    {
            //        Console.WriteLine("\t{0}", s.Name);
            //    }
            //}

            //foreach (var kvp in nfa.TFunction())
            //{
            //    Console.WriteLine("{0} {1}", kvp.Key.Item1, kvp.Key.Item2);
            //    foreach (var st in kvp.Value)
            //    {
            //        Console.WriteLine("\t{0}", st);
            //    }
            //}
        }

        private static void TestScanner(Scanner sc)
        {
            Token tk = new TkEof();
            do
            {
                tk = sc.Next();
                Console.WriteLine(tk);
            } while (!(tk is TkEof));
        }

        class SimplifyingRewriter : Rewriter
        {
            public override AstNode VisitConcat(ConcatNode con)
            {
                if (con.Nodes.Count == 0)
                    throw new Exception("Empty concat node =(");

                if (con.Nodes.Count == 1)
                    return con.Nodes[0].Visit(this);
                else
                    return base.VisitConcat(con);
            }

            public override AstNode VisitAlternation(AlternationNode alt)
            {
                var left = alt.Left.Visit(this);
                var right = alt.Right.Visit(this);

                var nodes = new List<AstNode>();
                nodes.Add(left);

                if (right is AlternationListNode)
                    nodes.AddRange((right as AlternationListNode).Nodes);
                else
                    nodes.Add(right);

                return new AlternationListNode(nodes);
            }
        }

        class TreePrinter : IVisitor<object>
        {
            int level = 0;
            private void tabs()
            {
                for (int i = 0; i < level; i++)
                {
                    Console.Write('\t');
                }
            }

            private void tabWrite(string format, params object[] args)
            {
                tabs();
                Console.WriteLine(format, args);
            }

            public object VisitLiteral(LiteralNode lit)
            {
                tabWrite("Lit: {0}", lit.Value);
                return null;
            }

            public object VisitKleen(KleeneStarNode kle)
            {
                tabWrite("Kleen");
                level++;
                kle.Child.Visit(this);
                level--;
                return null;
            }

            public object VisitAlternation(AlternationNode alt)
            {
                tabWrite("Alt");

                level++;

                tabWrite("Left");
                level++;
                alt.Left.Visit(this);
                level--;

                tabWrite("Right");
                level++;
                alt.Right.Visit(this);
                level--;

                level--;

                return null;
            }

            public object VisitConcat(ConcatNode con)
            {
                tabWrite("Concat");
                level++;
                con.Nodes.Select(n => n.Visit(this)).ToList();
                level--;
                return null;
            }

            public object VisitAlternationList(AlternationListNode alt)
            {
                tabWrite("Alt");
                level++;
                alt.Nodes.Select(n => n.Visit(this)).ToList();
                level--;
                return null;
            }
        }
    }
}
