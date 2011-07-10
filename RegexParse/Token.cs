using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexParse
{
    abstract class Token
    {
        protected Token(char value)
        {
            this.Name = this.GetType().Name.Substring(2);

            this.Value = value;
        }

        public string Name { get; private set; }
        public char Value { get; private set; }

        public virtual bool IsEof
        {
            get
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{1} {0}", Name, Value);
        }
    }

    class TkEof : Token
    {
        public TkEof()
            : base('\0')
        {
        }

        public override bool IsEof
        {
            get
            {
                return true;
            }
        }
    }

    class TkChar : Token
    {
        public TkChar(char ch)
            : base(ch)
        {
        }
    }

    class TkLParen : Token
    {
        public TkLParen()
            : base('(')
        {
        }
    }

    class TkRParen : Token
    {
        public TkRParen()
            : base(')')
        {
        }
    }

    class TkStar : Token
    {
        public TkStar()
            : base('*')
        {
        }
    }

    class TkPipe : Token
    {
        public TkPipe()
            : base('|')
        {
        }
    }
}
