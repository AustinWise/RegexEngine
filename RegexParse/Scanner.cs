using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RegexParse
{
    class Scanner
    {
        private readonly TextReader mTr;

        public Scanner(TextReader tr)
        {
            if (tr == null)
                throw new ArgumentNullException();
            mTr = tr;
        }

        public Token Next()
        {
            return IntToTk(mTr.Read());
        }

        public T Next<T>() where T : Token
        {
            return IntToTk(mTr.Read()) as T;
        }

        public Token Current
        {
            get
            {
                return IntToTk(mTr.Peek());
            }
        }

        public Token IntToTk(int val)
        {
            if (val == -1)
                return new TkEof();

            char ch = (char)val;

            switch (ch)
            {
                case '(':
                    return new TkLParen();
                case ')':
                    return new TkRParen();
                case '*':
                    return new TkStar();
                case '|':
                    return new TkPipe();
                default:
                    return new TkChar(ch);
            }
        }
    }
}
