using BallSortGeneratorRandomBall;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class Solver
    {

        private const int STACK_SIZE = 4;
        public List<List<int>> Stacks { get; set; }

        public bool Solved { get; set; }

        public Stack<Movement> Solution { get; private set; }
        public int TotalPossibleNode { get; private set; }

        public int TotalTraversedNode { get; set; }

        private HashSet<GameNode> visited;
        private Stack<GameNode> states;
        private int stackWinCount;

        public Solver()
        {
            Stacks = new List<List<int>>();
        }

        public void SolveLevelWithTree(Level level)
        {
            InitVariables(level);
            List<GameNode> winNodes = SolveWithTree();
            if (winNodes.Count > 0)
            {
                level.Solvable = true;
                winNodes.Sort((node1, node2) =>
                {
                    return node1.depth - node2.depth;
                });
                Solution = GetSolution(winNodes[0]);
                Console.WriteLine($"fastest route :");
            }
            else
            {
                level.Solvable = false;
            }
        }

        public void SolveLevelIterative(Level level)
        {
            InitVariables(level);
            //SolveIteratively();
        }

        private Stack<Movement> GetSolution(GameNode winNode)
        {
            Stack<Movement> moves = new Stack<Movement>();
            GetMoveRecursive(moves, winNode);
            return moves;
        }

        private void GetMoveRecursive(Stack<Movement> moves, GameNode node)
        {
            if (node.Parent != null)
            {
                moves.Push(node.Movement);
                GetMoveRecursive(moves, node.Parent);
            }
        }

        //private GameNode SolveIteratively()
        //{
        //    GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1));
        //    states.Push(root);

        //    while (Solved == false)
        //    {
        //        GameNode currentNode = states.Pop();
        //        List<Movement> moves = GetValidMoves(currentNode);

        //        foreach (var move in moves)
        //        {
        //            GameNode newNode = GenerateChildNode(currentNode, move);

        //            if (newNode.IsWin(stackWinCount))
        //            {
        //                //ShowGame(newNode);
        //                Solved = true;
        //                return newNode;
        //            }

        //            if (!visited.Contains(newNode))
        //            {
        //                visited.Add(newNode);
        //                states.Push(newNode);
        //            }
        //        }
        //    }

        //    return null;
        //}

        private List<GameNode> SolveWithTree()
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1));
            GameNode currentNode = root;
            TotalPossibleNode = 1;
            TotalTraversedNode = 0;
            List<GameNode> winNodes = new List<GameNode>();
            int leastMoves = Int32.MaxValue;

            while (true)
            {
                TotalTraversedNode++;
                if (currentNode.depth == leastMoves - 1)
                {
                    currentNode.MarkUnwinnable();

                }
                else if (currentNode.HaveChild())
                {
                    try
                    {
                        currentNode = currentNode.Childs.First(node => node.Winnable == true);
                    }
                    catch (InvalidOperationException)
                    {
                        currentNode.MarkUnwinnable();
                        if (currentNode.Equals(root)) break;
                    }
                }

                if (currentNode.Winnable == false)
                {
                    currentNode = currentNode.Parent;
                    continue;
                }

                List<Movement> moves = currentNode.GetValidMoves();

                if (moves.Count == 0)
                {
                    currentNode.MarkUnwinnable();
                }
                else
                {
                    TotalPossibleNode += moves.Count;
                    ShowGame(currentNode);
                    currentNode.Childs = new LinkedList<GameNode>();

                    foreach (var move in moves)
                    {
                        GameNode newNode = GenerateChildNode(currentNode, move);
                        
                        bool success = visited.TryGetValue(newNode, out GameNode matchedNode);
                        if (success)
                        {
                            if (matchedNode.depth > newNode.depth)
                            {
                                visited.Remove(matchedNode);

                                if (matchedNode.Parent != null)
                                {
                                    matchedNode.Parent.Childs.Remove(matchedNode);
                                    matchedNode.Parent = null;
                                }
                                currentNode.Childs.AddLast(newNode);
                                visited.Add(newNode);
                            }
                        }
                        else
                        {
                            currentNode.Childs.AddLast(newNode);
                            visited.Add(newNode);
                        }

                        if (newNode.IsWin(stackWinCount))
                        {
                            winNodes.Add(newNode);
                            if (leastMoves > newNode.depth)
                                leastMoves = newNode.depth;

                            //ShowGame(newNode);  
                            //Console.WriteLine(newNode.depth);
                        }
                    }
                }
            }

            return winNodes;
        }
        private GameNode GenerateChildNode(GameNode node, Movement move)
        {
            List<List<int>> cloneStack = new List<List<int>>();
            for (int i = 0; i < node.Stacks.Count; i++)
            {
                cloneStack.Add(node.Stacks[i].Clone());
            }

            var newNode = new GameNode(cloneStack, move, node);
            //while (move.IsValid(newNode.Stacks)) // multiple same move
            //{
            //    newNode.MakeMove();
            //}
            newNode.MakeMove();

            return newNode;
        }

        public static void ShowGame(GameNode currentState)
        {
            Console.WriteLine($"{currentState.Movement.From}->{currentState.Movement.To}");
            for (int i = STACK_SIZE - 1; i >= 0; i--)
            {
                for (int j = 0; j < currentState.Stacks.Count; j++)
                {
                    try
                    {
                        string number = currentState.Stacks[j][i].ToString("00");
                        Console.Write($"[{number}]");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.Write("[  ]");
                    }
                }
                
                Console.WriteLine();
            }
            Console.WriteLine(currentState.GetHashCode(currentState));
        }

        private void InitVariables(Level level)
        {
            var sequence = level.Sequence;
            Solution = new Stack<Movement>();
            states = new Stack<GameNode>();
            visited = new HashSet<GameNode>(new GameNode());

            for (int i = 0; i < sequence.Length / STACK_SIZE; i++)
            {
                List<int> stack = new List<int>(STACK_SIZE);
                for (int j = 0; j < STACK_SIZE; j++)
                {
                    stack.Add(sequence[STACK_SIZE * i + j]);
                }

                Stacks.Add(stack);
            }

            stackWinCount = Stacks.Count;

            Stacks.Add(new List<int>(STACK_SIZE));
            Stacks.Add(new List<int>(STACK_SIZE));
        }


        public List<Movement> GetSolutionFormatted()
        {
            return Solution.ToList();
        }


        // Utility
        public static bool IsStackCompleted(List<int> numbers)
        {
            if (numbers.Count == STACK_SIZE)
            {
                for (int i = 0; i < STACK_SIZE - 1; i++)
                {
                    if (numbers[i] != numbers[i + 1]) return false;
                }

                return true; // all elements are equal to eachother
            }
            else
            {
                return false;
            }
        }
    }
}
