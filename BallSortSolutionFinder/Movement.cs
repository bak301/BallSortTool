using System;
using System.Collections.Generic;
using System.Text;

namespace BallSortSolutionFinder
{
    public class Movement
    {
        public int From { get; set; }
        public int To { get; set; }

        public Movement(int from, int to)
        {
            From = from;
            To = to;
        }

        public bool IsValid(List<Stack<int>> state)
        {
            Stack<int> fromStack = state[From];
            Stack<int> toStack = state[To];

            bool IsFromStackEmpty = fromStack.Count == 0;
            bool IsToStackFull = toStack.Count == 4;
            bool IsItemMatched = fromStack.Peek() == toStack.Peek();
            bool IsToStackEmpty = toStack.Count == 4;

            return IsFromStackEmpty == false
                && IsToStackFull == false
                && (IsItemMatched == true || IsToStackEmpty == true);
        }
    }
}
