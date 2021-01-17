using BallSortGeneratorRandomBall;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace BallSortSolutionFinder
{
    public class Solver
    {

        private int stackSize;
        public List<int?[]> Stacks { get; set; }
        public float TimeLimit { get; set; }
        public float TimeFinished { get; set; }
        public bool Solved { get; private set; }

        public Stack<Movement> ShortestSolution { get; private set; }
        public Stack<Movement> FirstSolution { get; private set; }
        public long FirstSolutionTime { get; set; }
        public int TotalNodeTraversed { get; set; }

        //private SortedSet<GameNode> visited;
        private HashSet<GameNode> visited;
        private Queue<GameNode> stateQueue;
        private int stackWinCount;
        private bool IsFirstSolutionFound;
        Stopwatch sw;

        public Solver(int stackSize, float timeLimit = float.MaxValue)
        {
            this.stackSize = stackSize;
            TimeLimit = timeLimit;
        }

        public void FindShortestSolutionDFS(Level level)
        {
            InitVariables(level);
            SortedList<int, GameNode> winNodes = SolveWithDFS(AddToWinNodes);
            if (winNodes.Count > 0)
            {
                level.Solvable = true;
                ShortestSolution = GetSolution(winNodes.Values[0]);
            }
            else
            {
                level.Solvable = false;
            }
        }

        public void FindFirstSolution(Level level)
        {
            InitVariables(level);
            SortedList<int, GameNode> winNodes = SolveWithDFS(ReturnFirstWinNode);
            if (Solved)
            {
                FirstSolution = GetSolution(winNodes.Values[0]);
            }
        }

        public void FindShortestSolutionBFS(Level level)
        {
            InitVariables(level);
            var winNodes = SolveWithBFS();
            if (winNodes.Count > 0)
            {
                level.Solvable = true;
                ShortestSolution = GetSolution(winNodes.Values[0]);
            }
            else
            {
                level.Solvable = false;
            }
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

        private SortedList<int, GameNode> SolveWithBFS()
        {
            SortedList<int, GameNode> winNodes = new SortedList<int, GameNode>();
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount+1, stackSize), null, stackSize);
            stateQueue.Enqueue(root);
            IEqualityComparer<GameNode> nodeComparer = root;
            int leastMoves = Int32.MaxValue;
            var sw = Stopwatch.StartNew();

            while (true)
            {
                TotalNodeTraversed++;
                if (stateQueue.Count == 0) break;
                GameNode currentNode = stateQueue.Dequeue();
#if DEBUG
                //ShowGame(currentNode);
#endif
                List<Movement> moves = currentNode.GetValidMoves();
                
                if (moves.Count > 0)
                {
                    currentNode.Childs = new List<GameNode>();
                    foreach (var move in moves)
                    {
                        GameNode newNode = currentNode.GenerateChildNode(move);
                        if (newNode.IsWin(stackWinCount))
                        {
                            if (leastMoves > newNode.MoveCount)
                                leastMoves = newNode.MoveCount;

                            winNodes.TryAdd(newNode.MoveCount ,newNode);
                        }
                        else if (newNode.MoveCount < leastMoves - 1)
                        {                        
                            if (visited.TryGetValue(newNode, out GameNode matchedNode))
                            {
                                if (matchedNode.MoveCount > newNode.MoveCount)
                                {
                                    visited.Remove(matchedNode);

                                    if (matchedNode.Parent != null)
                                    {
                                        matchedNode.Parent.Childs.Remove(matchedNode);
                                    }
                                    currentNode.Childs.Add(newNode);
                                    visited.Add(newNode);
                                }
                            } else
                            {
                                currentNode.Childs.Add(newNode);
                                visited.Add(newNode);
                            }
                        }
                    }
                    var orderedNodes = currentNode.Childs.OrderByDescending(node => node.GetScoreWithNoMoveCountPenalty());
                    foreach (var node in orderedNodes)
                    {
                        stateQueue.Enqueue(node);
                    }
                }
            }
            TimeFinished = sw.ElapsedMilliseconds / 1000f;
            return winNodes;
        }

        private SortedList<int, GameNode> SolveWithDFS(Action<SortedList<int, GameNode>, GameNode> OnWinNodeFound)
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1, stackSize));
            root.SortedStacks = root.SortStacks();
            GameNode currentNode = root;
            SortedList<int, GameNode> winNodes = new SortedList<int, GameNode>();
            int leastMoves = Int32.MaxValue;

            sw = Stopwatch.StartNew();
            while (true && Solved == false && (sw.ElapsedMilliseconds / 1000f < TimeLimit))
            {
                TotalNodeTraversed++;
                if (currentNode.HaveChild())
                {
                    try
                    {
                        currentNode = currentNode.Childs.First(node => node.Winnable == true);
                    }
                    catch (InvalidOperationException)
                    {
                        currentNode.MarkUnwinnable();
                        if (currentNode.CompareTo(root) == 0) break;
                    }
                }

                var moves = currentNode.GetValidMoves();

                if (moves.Count == 0)
                {
                    currentNode.MarkUnwinnable();
                }

                if (currentNode.Winnable == false)
                {
                    currentNode = currentNode.Parent;
                    continue;
                }
#if DEBUG
                //ShowGame(currentNode);
#endif
                currentNode.Childs = new List<GameNode>();

                foreach (var move in moves)
                {
                    GameNode newNode = currentNode.GenerateChildNode(move);
                    if (newNode.IsWin(stackWinCount))
                    {
                        if (leastMoves > newNode.MoveCount)
                            leastMoves = newNode.MoveCount;

                        OnWinNodeFound.Invoke(winNodes, newNode);
                    } else if (newNode.MoveCount < leastMoves - 1)
                    {
                        if (visited.TryGetValue(newNode, out GameNode matchedNode))
                        {
                            if (matchedNode.MoveCount > newNode.MoveCount)
                            {
                                visited.Remove(matchedNode);

                                if (matchedNode.Parent != null)
                                {
                                    matchedNode.Parent.Childs.Remove(matchedNode);
                                }
                                currentNode.Childs.Add(newNode);
                                visited.Add(newNode);
                            }
                        } else
                        {
                            currentNode.Childs.Add(newNode);
                            visited.Add(newNode);
                        }
                    }
                }
                currentNode.Childs = currentNode.Childs.OrderByDescending(node => node.GetScoreWithNoMoveCountPenalty()).ToList();
            }
            TimeFinished = sw.ElapsedMilliseconds / 1000f;
            return winNodes;
        }

        private void AddToWinNodes(SortedList<int, GameNode> winNodes, GameNode newNode)
        {
            winNodes.TryAdd(newNode.MoveCount, newNode);
            if (IsFirstSolutionFound == false)
            {
                ShowFirstSolution(newNode);
            } 
        }

        private void ReturnFirstWinNode(SortedList<int, GameNode> winNodes, GameNode newNode)
        {
            winNodes.TryAdd(newNode.MoveCount, newNode);
            Solved = true;
        }

        private void ShowFirstSolution(GameNode winNode)
        {
            IsFirstSolutionFound = true;
            FirstSolution = GetSolution(winNode);
            FirstSolutionTime = sw.ElapsedMilliseconds;

            Console.WriteLine($"First Solution : {FirstSolution.Sum(mv => mv.MoveCount)} move");
            foreach (var mv in FirstSolution)
            {
                Console.Write($"{mv.From}->{mv.To}({mv.MoveCount}) ");
            }
            Console.WriteLine();
            Console.WriteLine($"First Solution found in {FirstSolutionTime} ms");
        }

        public static void ShowGame(GameNode currentState)
        {
            Console.WriteLine();
            Console.WriteLine($"{currentState.Movement.From}->{currentState.Movement.To}");

            for (int i = currentState.StackSize - 1; i >= 0; i--)
            {
                for (int j = 0; j < currentState.Stacks.Count; j++)
                {
                    if (currentState.Stacks[j][i].HasValue)
                    {
                        Console.Write($"[{currentState.Stacks[j][i]}]");
                    } else
                    {
                        Console.Write("[ ]");
                    }
                }
                Console.WriteLine();
            }
        }

        private void InitVariables(Level level)
        {
            var sequence = level.Sequence;
            Stacks = new List<int?[]>();
            Solved = false;
            TimeFinished = 0;
            ShortestSolution = new Stack<Movement>();
            FirstSolution = new Stack<Movement>();
            stateQueue = new Queue<GameNode>();
            visited = new HashSet<GameNode>(new GameNode());
            IsFirstSolutionFound = false;
            TotalNodeTraversed = 0;

            for (int i = 0; i < sequence.Length / stackSize; i++)
            {
                int?[]stack = new int?[stackSize];
                for (int j = 0; j < stackSize; j++)
                {
                    stack[j] = sequence[stackSize * i + j];
                }

                Stacks.Add(stack);
            }

            stackWinCount = Stacks.Count;

            Stacks.Add(new int?[stackSize]);
            Stacks.Add(new int?[stackSize]);  
        }

        public List<Movement> GetSolutionFormatted(Stack<Movement> solution)
        {
            return solution.ToList();
        }
    }
}
