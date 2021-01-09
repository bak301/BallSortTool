using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BallSortSolutionFinder
{
    class IntStackCompare : IComparer<Stack<int>>
    {
        public static IntStackCompare instance = new IntStackCompare();

        public int Compare([AllowNull] Stack<int> x, [AllowNull] Stack<int> y)
        {
            if (x.Count == y.Count)
            {

                var xList = x.ToArray();
                var yList = y.ToArray();

                for (int i = 0; i < xList.Length; i++)
                {
                    if (xList[i] != yList[i])
                    {
                        return xList[i] - yList[i];
                    }
                }
                return 0;
            }
            else
            {
                return x.Count - y.Count;
            }
        }
    }
}
