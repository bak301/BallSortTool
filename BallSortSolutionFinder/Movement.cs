using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BallSortSolutionFinder
{
    public class Movement
    {
        public int From { get; set; }
        public int To { get; set; }

        public int MoveCount { get; set; }

        public Movement(int from, int to)
        {
            From = from;
            To = to;
            MoveCount = 0;
        }

        public bool IsValidAndGood(int?[] fromStack, int?[] toStack)
        {
            int fromStackCount = fromStack.CountStack();
            int toStackCount = toStack.CountStack();

            // Valid check
            bool IsFromStackEmpty = fromStackCount == 0;

            if (IsFromStackEmpty) return false;

            bool IsToStackFull = toStackCount == toStack.Length;

            if (IsToStackFull) return false;

            bool IsToStackEmpty = toStackCount == 0;
            bool IsItemMatched = !IsToStackEmpty && fromStack.Peek() == toStack.Peek();

            if (IsItemMatched == false && IsToStackEmpty == false) return false;

            // Valid but bad move
            bool IsMoveFromSingleTypeStackToEmpty = fromStack.IsSingleType() && IsToStackEmpty;

            if (IsMoveFromSingleTypeStackToEmpty) return false;

            bool IsMoveFromSingleTypeStackToAnother = IsItemMatched
                                                   && fromStack.IsSingleType()
                                                   && toStack.IsSingleType()
                                                   && fromStackCount > toStackCount;

            if (IsMoveFromSingleTypeStackToAnother) return false;

            bool IsBadMove = IsItemMatched
                          && !IsToStackEmpty
                          && IsMoveFromBiggerStackToLesser(fromStack, toStack);

            if (IsBadMove) return false;

            return true;
        }

        private bool IsMoveFromBiggerStackToLesser(int?[] from, int?[] to)
        {
            var spareSpace = to.Length - to.CountStack();
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
    }
}
