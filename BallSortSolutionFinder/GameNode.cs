using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameNode : IComparable<GameNode>, ICloneable, IEqualityComparer<GameNode>
    {

        public List<Stack<int>> Stacks { get; set; }
        public Movement Movement { get; set; }

        public GameNode Parent { get; set; }

        public List<GameNode> Childs { get; set; }

        public bool Winnable { get; private set; }

        public int depth { get; set; }

        public GameNode()
        {

        }

        public GameNode(List<Stack<int>> stacks, Movement movement, GameNode parent = null)
        {
            this.Winnable = true;
            this.Parent = parent;
            this.Stacks = stacks;
            this.Movement = movement;

            if (parent != null) this.depth = parent.depth + 1;
        }

        public void MakeMove()
        {
            //Console.WriteLine("Stack before");
            //Solver.ShowGame(this);
            int pickedNumber = Stacks[Movement.From].Pop();
            Stacks[Movement.To].Push(pickedNumber);
            //Console.WriteLine("Stack after");
            //Solver.ShowGame(this);
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

        public bool IsWin(int stackWinCount)
        {
            int counter = 0;
            foreach (var stack in Stacks)
            {
                if (Solver.IsStackCompleted(stack))
                    counter++;
            }

            return counter == stackWinCount;
        }

        public int CompareTo([AllowNull] GameNode other)
        {
            if (other == null) return -1;

            var stackComparer = new IntStackCompare();
            var copyThis = (GameNode)this.Clone();
            var copyOther = (GameNode)other.Clone();

            copyThis.Stacks.Sort(stackComparer);
            copyOther.Stacks.Sort(stackComparer);

            for (int i = 0; i < copyThis.Stacks.Count; i++)
            {
                if (stackComparer.Compare(copyThis.Stacks[i], copyOther.Stacks[i]) != 0)
                {
                    return -1;
                }
            }
            return 0;
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
                cloneState.Stacks.Add(this.Stacks[i].Clone());
            }

            return cloneState;
        }

        public bool Equals([AllowNull] GameNode x, [AllowNull] GameNode y)
        {
            return x.CompareTo(y) == 0;
        }

        public int GetHashCode([DisallowNull] GameNode obj)
        {
            throw new NotImplementedException();
        }



        // STATIC CLASS
    }
}
