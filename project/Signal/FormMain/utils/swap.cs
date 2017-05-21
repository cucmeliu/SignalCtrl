using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormMain.C
{
    class swap
    {
        public static string SwapChar(ref string s, int p1, int p2)
        {
            string swapstr;

            if (p1 == p2) return s;

            if (p1 > p2) { int p = p1; p1 = p2; p2 = p; } //Swap p1,p2

            swapstr = s.Substring(0, p1) + s[p2];
            swapstr += s.Substring(p1 + 1, p2 - p1 - 1) + s[p1];
            swapstr += s.Substring(p2 + 1, s.Length - p2 - 1);
            s = swapstr;
            return s;
        }
    }
}
