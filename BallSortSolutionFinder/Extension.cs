using System;
using System.Collections.Generic;
using System.Text;

namespace BallSortSolutionFinder
{
    public static class Extension
    {
        public static Stack<int> Clone(this Stack<int> stack)
        {
            return new Stack<int>(new Stack<int>(stack));
        }
    }
}
