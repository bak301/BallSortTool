using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BallSortSolutionFinder
{
    public class Movement : IEqualityComparer<Movement>
    {
        public int From { get; set; }
        public int To { get; set; }

        public int MoveCount { get; set; }

        internal int Score { get; set; }

        private readonly int StackSize;

        public Movement(int from, int to, int stackSize = 4)
        {
            From = from;
            To = to;
            StackSize = stackSize;
            MoveCount = 0;
            Score = 0;
        }

        public bool IsValidAndGood(List<Stack<int>> state)
        {
            Stack<int> fromStack = state[From];
            Stack<int> toStack = state[To];

            // Valid check
            bool IsFromStackEmpty = fromStack.Count == 0;
            bool IsToStackFull = toStack.Count == 4;
            bool IsToStackEmpty = toStack.Count == 0;
            bool IsItemMatched;
            bool IsFromStackCompleted = fromStack.IsCompleted(StackSize);

            if (!IsFromStackEmpty && !IsToStackEmpty)
            {
                IsItemMatched = fromStack.Peek() == toStack.Peek();
            } else
            {
                IsItemMatched = false;
            }

            // Valid but bad move
            bool IsMoveFromSingleTypeStackToEmpty = fromStack.IsSingleType() && toStack.Count == 0;
            bool IsMoveFromSingleTypeStackToAnother = IsItemMatched  
                                                   && fromStack.IsSingleType() 
                                                   && toStack.IsSingleType() 
                                                   && fromStack.Count > toStack.Count;
            bool IsBadMove = IsItemMatched
                          && toStack.Count > 0
                          && IsMoveFromBiggerStackToLesser(fromStack, toStack);

            bool result = IsFromStackEmpty == false
                && IsFromStackCompleted == false
                && IsToStackFull == false
                && IsMoveFromSingleTypeStackToEmpty == false
                && IsMoveFromSingleTypeStackToAnother == false
                && IsBadMove == false
                && (IsItemMatched == true || IsToStackEmpty == true);

            return result;
        }

        private bool IsMoveFromBiggerStackToLesser(Stack<int> from, Stack<int> to)
        {
            var cloneFrom = from.Clone();
            var spareSpace = StackSize - to.Count;
            var sameTypeCount = 0;
            while (cloneFrom.Count > 0)
            {
                if (cloneFrom.Pop() == to.Peek())
                {
                    sameTypeCount++;
                } else
                {
                    break;
                }
            }

            return spareSpace < sameTypeCount;
        }

        public bool Equals([AllowNull] Movement x, [AllowNull] Movement y)
        {
            return x.From == y.From && x.To == y.To;
        }

        public int GetHashCode([DisallowNull] Movement obj)
        {
            throw new NotImplementedException();
        }
    }
}
