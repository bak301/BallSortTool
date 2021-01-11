using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class Movement : IEqualityComparer<Movement>
    {
        public int From { get; set; }
        public int To { get; set; }

        public Movement(int from, int to)
        {
            From = from;
            To = to;
        }

        public bool IsValid(List<List<int>> currentState)
        {
            List<int> fromStack = currentState[From];
            List<int> toStack = currentState[To];

            bool IsFromStackEmpty = fromStack.Count == 0;
            bool IsToStackFull = toStack.Count == 4;
            bool IsToStackEmpty = toStack.Count == 0;
            bool IsItemMatched;
            bool IsFromStackCompleted = Solver.IsStackCompleted(fromStack);
            bool IsMoveFromSingleTypeStackToEmpty = fromStack.IsSingleType() && toStack.Count == 0;

            if (!IsFromStackEmpty && !IsToStackEmpty)
            {
                IsItemMatched = fromStack.Last() == toStack.Last();
            }
            else
            {
                IsItemMatched = false;
            }

            bool result = IsFromStackEmpty == false
                && IsFromStackCompleted == false
                && IsToStackFull == false
                && IsMoveFromSingleTypeStackToEmpty == false
                && (IsItemMatched == true || IsToStackEmpty == true);

            return result;
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
