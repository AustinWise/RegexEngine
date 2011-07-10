using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.IO;

namespace RegexParse
{
    class IlOutput
    {
        DFA dfa;
        public IlOutput(DFA dfa)
        {
            this.dfa = dfa;
        }


        public Func<string, bool> ToFunc()
        {
            var meth = new DynamicMethod("Regex_" + Guid.NewGuid().ToString(), typeof(bool), new[] { typeof(TextReader) });

            var il = meth.GetILGenerator();

            var map = new Dictionary<State, Label>();
            foreach (var st in dfa.States)
            {
                map[st] = il.DefineLabel();
            }

            il.Emit(OpCodes.Br, map[dfa.StartState]);
            var ch = il.DeclareLocal(typeof(int));

            foreach (var st in dfa.States)
            {
                il.MarkLabel(map[st]);

                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, typeof(TextReader).GetMethod("Read", new Type[0]), null);
                il.Emit(OpCodes.Stloc, ch);
                foreach (var t in dfa.Transitions.Where(t => t.Item1 == st))
                {
                    il.Emit(OpCodes.Ldloc, ch);
                    il.Emit(OpCodes.Ldc_I4, t.Item2.Value);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Brtrue, map[t.Item3]);
                }
                if (st.IsFinal)
                {
                    il.Emit(OpCodes.Ldloc, ch);
                    il.Emit(OpCodes.Ldc_I4_M1);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Ret);
                }
                else
                {
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ret);
                }
            }

            var innerFun = (Func<TextReader, bool>)meth.CreateDelegate(typeof(Func<TextReader, bool>));
            return s => innerFun(new StringReader(s));
        }
    }
}
