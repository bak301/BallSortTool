using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameState : IComparable<GameState>
    {
        public List<Stack<int>> stacks { get; set; }
        public Movement movement { get; set; }

        public GameState()
        {

        }

        public GameState(List<Stack<int>> stacks, Movement movement)
        {
            this.stacks = stacks;
            this.movement = movement;
        }

        public int CompareTo([AllowNull] GameState other)
        {
            bool IsMovementMatched = (movement.From == other.movement.From && movement.To == other.movement.To);
            bool IsStacksMatched = string.Join(',', stacks.Select(stack => string.Join(',', stack)))
                                == string.Join(',', other.stacks.Select(stack => string.Join(',', stack)));

            return IsMovementMatched && IsStacksMatched ? 0 : -1;
        }
    }
}
