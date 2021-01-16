using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BallSortSolutionFinder
{
    public class Movement : IEqualityComparer<Movement>
    {
        public int From { get; set; }
        public int To { get; set; }

        public int MoveCount { get; set; }

        private readonly int StackSize;

        public Movement(int from, int to, int stackSize)
        {
            From = from;
            To = to;
            StackSize = stackSize;
            MoveCount = 0;
        }

        public bool IsValidAndGood(List<int?[]> state)
        {
            int?[] fromStack = state[From];
            int?[] toStack = state[To];

            int fromStackCount = fromStack.CountStack();
            int toStackCount = toStack.CountStack();

            // Valid check
            bool IsFromStackEmpty = fromStackCount == 0;
            bool IsToStackFull = toStackCount == StackSize;
            bool IsToStackEmpty = toStackCount == 0;
            bool IsItemMatched;

            if (!IsFromStackEmpty && !IsToStackEmpty)
            {
                IsItemMatched = fromStack.Peek() == toStack.Peek();
            } else
            {
                IsItemMatched = false;
            }

            // Valid but bad move
            bool IsMoveFromSingleTypeStackToEmpty = fromStack.IsSingleType() && toStackCount == 0;
            bool IsMoveFromSingleTypeStackToAnother = IsItemMatched  
                                                   && fromStack.IsSingleType() 
                                                   && toStack.IsSingleType() 
                                                   && fromStackCount > toStackCount;
            bool IsBadMove = IsItemMatched
                          && toStackCount > 0
                          && IsMoveFromBiggerStackToLesser(fromStack, toStack);

            bool result = IsFromStackEmpty == false
                && IsToStackFull == false
                && IsMoveFromSingleTypeStackToEmpty == false
                && IsMoveFromSingleTypeStackToAnother == false
                && IsBadMove == false
                && (IsItemMatched == true || IsToStackEmpty == true);

            return result;
        }

        private bool IsMoveFromBiggerStackToLesser(int?[] from, int?[] to)
        {
            var spareSpace = StackSize - to.CountStack();
            var sameTypeCount = 0;
            int? toTopValue = to.Peek();

            for (int i = 0; i < from.CountStack(); i++)
            {
                if (from[i] == toTopValue)
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
