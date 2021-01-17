using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameNode : IComparable<GameNode>, IEqualityComparer<GameNode>
    {
        public GameNode Parent { get; set; }

        public List<GameNode> Childs { get; set; }

        public List<int?[]> Stacks { get; set; }

        public SortedList<long, int?[]> SortedStacks;
        public Movement Movement { get; set; }

        public int MoveCount { get; set; }

        public GameNode()
        {

        }

        public GameNode(List<int?[]> stacks, Movement movement, GameNode parent = null)
        {
            this.Parent = parent;
            this.Stacks = stacks;
            this.Movement = movement;
            this.MoveCount = parent != null ? parent.MoveCount : 0;
        }

        public SortedList<long, int?[]> SortStacks()
        {
            var sorted = new SortedList<long, int?[]>(Stacks.Count);
            for (int i = 0; i < Stacks.Count; i++)
            {
                long index = Extension.GetIndex(Stacks[i]);

                bool succeed = sorted.TryAdd(index, Stacks[i]);
                while (!succeed)
                {
                    index -= 1;
                    succeed = sorted.TryAdd(index, Stacks[i]);
                }
            }
            return sorted;
        }

        public void MakeMove()
        {
            int? pickedNumber = Stacks[Movement.From].Pop();
            Stacks[Movement.To].Push(pickedNumber);
        }

        public bool IsWin(int stackWinCount)
        {
            int counter = 0;
            foreach (var stack in Stacks)
            {
                if (stack.IsCompleted())
                    counter++;
            }

            return counter == stackWinCount;
        }

        public List<Movement> GetValidMoves()
        {
            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < Stacks.Count; i++)
            {
                if (Stacks[i].IsCompleted() || Stacks[i].CountStack() == 0) continue;
                for (int j = 0; j < Stacks.Count; j++)
                {
                    if (Stacks[j].CountStack() == Stacks[j].Length) continue;
                    if (i != j && (i != Movement.To || j != Movement.From))
                    {
                        Movement move = new Movement(i, j);
                        if (move.IsValidAndGood(Stacks[i], Stacks[j]))
                            moves.Add(move);
                    }
                }
            }

            return moves;
        }

        public GameNode GenerateChildNode(Movement move)
        {
            var newNode = new GameNode(Stacks.CloneList(), move, this);

            do
            {
                newNode.MakeMove();
                move.MoveCount++;
                newNode.MoveCount++;
            }
            while (move.IsValidAndGood(newNode.Stacks[move.From], newNode.Stacks[move.To])); // multiple same move

            newNode.SortedStacks = newNode.SortStacks();
            return newNode;
        }

        public int CompareTo([AllowNull] GameNode other)
        {
            if (other == null) return -1;

            var stackComparer = IntStackCompare.instance;

            for (int i = 0; i < this.SortedStacks.Count; i++)
            {
                var result = stackComparer.Compare(this.SortedStacks.Values[i], other.SortedStacks.Values[i]);
                if ( result != 0)
                {
                    return result;
                }
            }
            return 0;
        }

        public bool Equals([    AllowNull] GameNode x, [AllowNull] GameNode y)
        {
#if DEBUG
            //Console.WriteLine($"Hash code x : {GetHashCode(x)} Hash code y : {GetHashCode(y)}");
            //Console.WriteLine("----------X------------");
            //Solver.ShowGame(x, 4);
            //Console.WriteLine("-----------------------");
            //Console.WriteLine("----------Y------------");
            //Solver.ShowGame(y, 4);
            //Console.WriteLine("-----------------------");
#endif
            return x.CompareTo(y) == 0;
        }

        public int GetHashCode([DisallowNull] GameNode obj)
        {
            var hashCode = new HashCode();
            for (int i = 0; i < obj.SortedStacks.Count; i++)
            {
                hashCode.Add(obj.SortedStacks.Keys[i]);
            }
            return hashCode.ToHashCode();
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
                } else if (Stacks[i].IsCompleted())
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
