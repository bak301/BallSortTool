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

        public int Depth { get; set; }

        public int MoveCount { get; set; }

        public int StackSize { get; set; }

        public GameNode()
        {

        }

        public GameNode(List<Stack<int>> stacks, Movement movement, GameNode parent = null, int stackSize = 4)
        {
            this.Winnable = true;
            this.Parent = parent;
            this.Stacks = stacks;
            this.Movement = movement;
            this.StackSize = stackSize;

            if (parent != null)
            {
                this.Depth = parent.Depth + 1;
                this.MoveCount = parent.MoveCount;
            } else
            {
                this.Depth = 1;
                this.MoveCount = 0;
            }
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
                if (stack.IsCompleted(StackSize))
                    counter++;
            }

            return counter == stackWinCount;
        }

        public List<Movement> GetValidMoves()
        {
            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < Stacks.Count; i++)
            {
                for (int j = 0; j < Stacks.Count; j++)
                {
                    if (i != j && (i != Movement.To || j != Movement.From))
                    {
                        Movement move = new Movement(i, j);
                        if (move.IsValidAndGood(Stacks))
                            moves.Add(move);
                    }
                }
            }

            return moves;
        }

        public GameNode GenerateChildNode(Movement move)
        {
            List<Stack<int>> cloneStack = new List<Stack<int>>();
            for (int i = 0; i < Stacks.Count; i++)
            {
                cloneStack.Add(Stacks[i].Clone());
            }

            var newNode = new GameNode(cloneStack, move, this);

            while (move.IsValidAndGood(newNode.Stacks)) // multiple same move
            {
                newNode.MakeMove();
                move.MoveCount++;
                newNode.MoveCount++;
            }

            return newNode;
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

        // Calculate score
        public int GetScore()
        {
            double score = 0;
            Stack<int> temp;
            int nextItem, lastItem;
            
            for (int i = 0; i < Stacks.Count; i++)
            {
                if (Stacks[i].Count == 0)
                {
                    score += 40;
                } else if (Stacks[i].IsCompleted(StackSize))
                {
                    score += 100;
                }
                else
                {
                    double count = 1;
                    temp = Stacks[i].Clone();
                    lastItem = temp.Pop();
                    while (temp.Count > 0)
                    {
                        nextItem = temp.Pop();
                        if (lastItem == nextItem)
                        {
                            count++;
                        }
                        else
                        {
                            score += Math.Pow(3, count);
                            lastItem = nextItem;
                            count = 1;
                        }
                    }
                    //score += Math.Pow(3, count);
                }
            }
            return (int)score - Movement.MoveCount;
        }
    }
}
