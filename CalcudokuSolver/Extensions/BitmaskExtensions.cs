using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcudokuSolver.Extensions
{
    public static class BitmaskExtensions
    {
        public static IEnumerable<int> GetBitsSet(this int mask)
        {
            int value = 1;
            while (mask > 0)
            {
                if ((mask & 1) > 0)
                    yield return value;
                value++;
                mask >>= 1;
            }
        }

        // See https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        public static int GetBitCount(this int v)
        {
            v -= ((v >> 1) & 0x55555555);                 
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
        }

    }
}
