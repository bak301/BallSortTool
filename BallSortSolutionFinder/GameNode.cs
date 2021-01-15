using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameNode : IComparable<GameNode>, IEqualityComparer<GameNode>
    {

        public List<int?[]> Stacks { get; set; }

        public SortedList<int, int?[]> SortedStacks;
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

        public GameNode(List<int?[]> stacks, Movement movement, GameNode parent = null, int stackSize = 4)
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

        public SortedList<int, int?[]> InitSortedStacks()
        {
            var sorted = new SortedList<int, int?[]>();
            for (int i = 0; i < Stacks.Count; i++)
            {
                int index = 0;
                int offset = 0;
                for (int j = 0; j < Stacks[i].Length; j++)
                {
                    if (Stacks[i][j].HasValue)
                    {
                        index += 1000 / (int)Math.Pow(10, j) * ((int)Stacks[i][j] + 1);
                    } else
                    {
                        offset++;
                    }
                }
                if (index > 0)
                {
                    index /= (int)Math.Pow(10, offset);
                } else
                {
                    index = -99999 - i;
                }

                bool succeed = sorted.TryAdd(index, Stacks[i]);
                while (!succeed)
                {
                    index -= 10000;
                    succeed = sorted.TryAdd(index, Stacks[i]);
                }
            }
            return sorted;
        }

        public void MakeMove()
        {
            //Console.WriteLine("Stack before");
            //Solver.ShowGame(this);

            int? pickedNumber = Stacks[Movement.From].Pop();
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
                if (Stacks[i].IsCompleted(StackSize)) continue;
                for (int j = 0; j < Stacks.Count; j++)
                {
                    if (Stacks[j].IsCompleted(StackSize)) continue;
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
            List<int?[]> cloneStack = Stacks.CloneList();

            var newNode = new GameNode(cloneStack, move, this);

            while (move.IsValidAndGood(newNode.Stacks)) // multiple same move
            {
                newNode.MakeMove();
                move.MoveCount++;
                newNode.MoveCount++;
            }
            newNode.SortedStacks = newNode.InitSortedStacks();
            return newNode;
        }

        public int CompareTo([AllowNull] GameNode other)
        {
            if (other == null) return -1;

            var stackComparer = IntStackCompare.instance;

            for (int i = 0; i < this.SortedStacks.Count; i++)
            {
                if (stackComparer.Compare(this.SortedStacks.Values[i], other.SortedStacks.Values[i]) != 0)
                {
                    return -1;
                }
            }
            return 0;
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
        public int GetScoreWithNoMoveCountPenalty()
        {
            double score = 0;
            int? lastItem;
            
            for (int i = 0; i < Stacks.Count; i++)
            {
                int stackCount = Stacks[i].CountStack();
                if (stackCount == 0)
                {
                    score += 40;
                } else if (Stacks[i].IsCompleted(StackSize))
                {
                    score += 100;
                }
                else
                {
                    double count = 1;
                    lastItem = Stacks[i][stackCount - 1];

                    for (int j = stackCount - 2; j >= 0; j--)
                    {
                        if (lastItem == Stacks[i][j])
                        {
                            count++;
                        }
                        else
                        {
                            score += Math.Pow(3, count);
                            lastItem = Stacks[i][j];
                            count = 1;
                        }
                    }
                    //score += Math.Pow(3, count);
                }
            }
            return (int)score;
        }

        public int GetScoreWithBigMoveCountPenalty()
        {
            return GetScoreWithNoMoveCountPenalty() - Movement.MoveCount * 1000;
        }
    }
}
