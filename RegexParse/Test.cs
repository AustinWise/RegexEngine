namespace TestNS
{
    static class Test
    {
        public static bool Match(string s)
        {
            return Match(new System.IO.StringReader(s)); 
        }
        public static bool Match(System.IO.TextReader tr)
        {
            int ch;
            goto state0;
        state0:
            {
                ch = tr.Read();
                if (ch == 'a')
                    goto state1;
                return false;
            }
        state1:
            {
                ch = tr.Read();
                if (ch == 's')
                    goto state2;
                return false;
            }
        state2:
            {
                ch = tr.Read();
                if (ch == 'd')
                    goto state3;
                return false;
            }
        state3:
            {
                ch = tr.Read();
                if (ch == 'f')
                    goto state4;
                return false;
            }
        state4:
            {
                ch = tr.Read();
                if (ch == '2')
                    goto state5;
                if (ch == '3')
                    goto state6;
                if (ch == '4')
                    goto state7;
                return ch == -1;
            }
        state5:
            {
                ch = tr.Read();
                if (ch == '2')
                    goto state5;
                if (ch == '3')
                    goto state6;
                if (ch == '4')
                    goto state7;
                return ch == -1;
            }
        state6:
            {
                ch = tr.Read();
                if (ch == '2')
                    goto state5;
                if (ch == '3')
                    goto state6;
                if (ch == '4')
                    goto state7;
                return ch == -1;
            }
        state7:
            {
                ch = tr.Read();
                if (ch == '2')
                    goto state5;
                if (ch == '3')
                    goto state6;
                if (ch == '4')
                    goto state7;
                return ch == -1;
            }
        }
    }
}
