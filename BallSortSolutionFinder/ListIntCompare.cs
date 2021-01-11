using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BallSortSolutionFinder
{
    class ListIntCompare : IComparer<List<int>>
    {
        public static ListIntCompare instance = new ListIntCompare();

        public int Compare([AllowNull] List<int> x, [AllowNull] List<int> y)
        {
            if (x.Count == y.Count)
            {
                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i] != y[i])
                    {
                        return y[i] - x[i];
                    }
                }
                return 0;
            }
            else
            {
                return y.Count - x.Count;
            }
        }
    }
}
