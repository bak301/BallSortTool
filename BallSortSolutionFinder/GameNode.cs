using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class GameNode : ICloneable, IEqualityComparer<GameNode>
    {
        private static int STACK_SIZE = 4;
        public List<List<int>> Stacks { get; set; }
        public Movement Movement { get; set; }

        public GameNode Parent { get; set; }

        public LinkedList<GameNode> Childs { get; set; }

        public bool Winnable { get; private set; }

        public int depth { get; set; }

        public GameNode()
        {

        }

        public GameNode(List<List<int>> stacks, Movement movement, GameNode parent = null)
        {
            this.Winnable = true;
            this.Parent = parent;
            this.Stacks = stacks;
            this.Movement = movement;

            if (parent != null) this.depth = parent.depth + 1;
        }

        public List<Movement> GetValidMoves()
        {
            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < Stacks.Count; i++)
            {
                for (int j = 0; j < Stacks.Count; j++)
                {
                    if (i != Movement.To || j != Movement.From) // not a rewind move
                    {
                        if (Stacks[i] != Stacks[j])
                        {
                            Movement move = new Movement(i, j);
                            if (move.IsValid(Stacks))
                                moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }

        public void MakeMove()
        {
            //Console.WriteLine("Stack before");
            //Solver.ShowGame(this);

            int pickedNumber = Stacks[Movement.From].Last();
            Stacks[Movement.To].Add(pickedNumber);
            Stacks[Movement.From].RemoveAt(Stacks[Movement.From].Count - 1);

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
                if (Solver.IsStackCompleted(stack))
                    counter++;
            }

            return counter == stackWinCount;
        }


        /* ------------------------------------------------------------*/
        //public int CompareTo([AllowNull] GameNode other)
        //{
        //    if (other == null) return -1;

        //    var stackComparer = new ListIntCompare();
        //    var copyThis = (GameNode)this.Clone();
        //    var copyOther = (GameNode)other.Clone();

        //    copyThis.Stacks.Sort(stackComparer);
        //    copyOther.Stacks.Sort(stackComparer);

        //    for (int i = 0; i < copyThis.Stacks.Count; i++)
        //    {
        //        if (stackComparer.Compare(copyThis.Stacks[i], copyOther.Stacks[i]) != 0)
        //        {
        //            return -1;
        //        }
        //    }
        //    return 0;
        //}

        public object Clone()
        {
            GameNode cloneState = new GameNode
            {
                Movement = new Movement(Movement.From, Movement.To),
                Stacks = new List<List<int>>()
            };

            for (int i = 0; i < this.Stacks.Count; i++)
            {
                cloneState.Stacks.Add(this.Stacks[i].Clone());
            }

            return cloneState;
        }

        public bool Equals([AllowNull] GameNode x, [AllowNull] GameNode y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode([DisallowNull] GameNode obj)
        {
            List<List< int >> clone = new List<List<int>>(obj.Stacks.Count);
            for (int i = 0; i < obj.Stacks.Count; i++)
            {
                clone.Add(obj.Stacks[i].ToList());
            }

            clone.Sort(new ListIntCompare());

            return GetBigHash(clone);
            //return GetSmallHash(clone);
        }
        //private int GetSmallHash(List<List<int>> stacks)
        //{
        //    foreach (var list in stacks)
        //    {
        //        int listTotal = 0;
        //        for (int i = STACK_SIZE - 1; i >= 0; i--)
        //        {
        //            try
        //            {
        //                listTotal += (list[i] + 1 + stacks.IndexOf(list)) * Power(10, i + 1);
        //            }
        //            catch (ArgumentOutOfRangeException)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
        private int GetBigHash(List<List<int>> stacks)
        {
            var builder = new StringBuilder();

            /*     1
             * 2   2              2000
             * 11  3            130000 
             * 4   4          50000000 i = 3
             */
            foreach (var list in stacks)
            {
                int listTotal = 0;
                for (int i = STACK_SIZE - 1; i >= 0; i--)
                {
                    try
                    {
                        if (list[i] > 8)
                        {
                            listTotal += (list[i] + 2) * Power(10, i * 2);
                        }
                        else
                        {
                            listTotal += (list[i] + 1) * Power(10, i * 2 + 1);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
                builder.Append(listTotal);
            }
            Console.WriteLine($"str : {builder}");
            return builder.ToString().GetHashCode();
        }

        private int Power(int x, int y)
        {
            int number = 1;
            for (int i = 0; i < y; i++)
            {
                number *= x;
            }
            return number;
        }

        // STATIC CLASS
    }
}
