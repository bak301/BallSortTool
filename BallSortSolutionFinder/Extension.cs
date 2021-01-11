using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsCompleted(this Stack<int> stack, int stack_size)
        {
            List<int> numbers = stack.ToList();
            if (numbers.Count == stack_size)
            {
                for (int i = 0; i < stack_size - 1; i++)
                {
                    if (numbers[i] != numbers[i + 1]) return false;
                }

                return true; // all elements are equal to eachother
            }
            else
            {
                return false;
            }
        }
    }
}
