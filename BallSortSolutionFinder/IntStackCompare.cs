using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BallSortSolutionFinder
{
    class IntStackCompare : IComparer<Stack<int>>
    {
        public int Compare([AllowNull] Stack<int> x, [AllowNull] Stack<int> y)
        {
            if (x.Count == y.Count)
            {
                while (x.Count > 0)
                {
                    var xfirst = x.Pop();
                    var yfirst = y.Pop();
                    if ( xfirst == yfirst)
                    {
                        continue;
                    } else
                    {
                        return xfirst - yfirst;
                    }
                }
                return 0;
            } else
            {
                return x.Count - y.Count;
            }
        }
    }
}
