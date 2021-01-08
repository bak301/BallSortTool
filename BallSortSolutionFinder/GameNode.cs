using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameNode : IComparable<GameNode>, ICloneable
    {

        public List<Stack<int>> Stacks { get; set; }
        public Movement Movement { get; set; }

        public GameNode Parent { get; set; }

        public List<GameNode> Childs { get; set; }

        public bool Winnable { get; private set; }

        public GameNode()
        {

        }

        public GameNode(List<Stack<int>> stacks, Movement movement, GameNode parent = null)
        {
            this.Winnable = true;
            this.Parent = parent;
            this.Stacks = stacks;
            this.Movement = movement;
        }

        public void CheckChild()
        {
            if (Childs.Where(node => node.Winnable == true).Count() == 0)
            {
                MarkUnwinnable();
            }
        }

        public bool HaveChild()
        {
            return Childs != null;
        }

        public void MarkUnwinnable()
        {
            this.Winnable = false;
        }

        public int CompareTo([AllowNull] GameNode other)
        {
            var copyThis = (GameNode)this.Clone();
            var copyOther = (GameNode)other.Clone();

            var stackComparer = new IntStackCompare();
            copyThis.Stacks.Sort(stackComparer);
            copyOther.Stacks.Sort(stackComparer);

            bool IsMovementMatched = (Movement.From == other.Movement.From && Movement.To == other.Movement.To);
            bool IsStacksMatched = string.Join(',', copyThis.Stacks.Select(stack => string.Join(',', stack)))
                                == string.Join(',', copyOther.Stacks.Select(stack => string.Join(',', stack)));

            return IsMovementMatched && IsStacksMatched ? 0 : -1;
        }

        public object Clone()
        {
            GameNode cloneState = new GameNode
            {
                Movement = new Movement(Movement.From, Movement.To),
                Stacks = new List<Stack<int>>()
            };

            for (int i = 0; i < this.Stacks.Count; i++)
            {
                cloneState.Stacks.Add(new Stack<int>(new Stack<int>(this.Stacks[i])));
            }

            return cloneState;
        }



        // STATIC CLASS
    }
}
