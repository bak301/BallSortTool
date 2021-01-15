using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BallSortSolutionFinder
{
    class IntStackCompare : IComparer<int?[]>
    {
        public static IntStackCompare instance = new IntStackCompare();

        public int Compare([AllowNull] int?[] x, [AllowNull] int?[] y)
        {
            int xCount = x.CountStack();
            int yCount = y.CountStack();
            if (xCount == yCount)
            {
                for (int i = 0; i < xCount; i++)
                {
                    if (x[i] != y[i])
                    {
                        return (int)(x[i] - y[i]);
                    }
                }
                return 0;
            }
            else
            {
                return xCount - yCount;
            }
        }
    }
}
