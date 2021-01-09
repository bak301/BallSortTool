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

        public static bool IsSingleType(this Stack<int> stack)
        {
            if (stack.Count > 0)
            {
                var clone = stack.Clone();
                int reference = clone.Pop();
                while (clone.Count > 0)
                {
                    if (clone.Pop() != reference) return false;
                }
                return true;
            } else
            {
                return false;
            }
        }
    }
}
