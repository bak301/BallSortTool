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

        private const int STACK_SIZE = 4;
        public List<Stack<int>> Stacks { get; set; }
        public float TimeLimit { get; set; }
        public float TimeFinished { get; set; }
        public bool Solved { get; private set; }

        public Stack<Movement> ShortestSolution { get; private set; }
        public Stack<Movement> FirstSolution { get; private set; }
        public long FirstSolutionTime { get; set; }
        public int TotalNodeTraversed { get; set; }

        private SortedSet<GameNode> visited;
        private Queue<GameNode> stateQueue;
        private int stackWinCount;
        private bool IsFirstSolutionFound;
        Stopwatch sw;

        public Solver()
        {     
            TimeLimit = float.MaxValue;
        }

        public Solver(float timeLimit)
        {
            TimeLimit = timeLimit;
        }

        public void FindShortestSolutionDFS(Level level)
        {
            InitVariables(level);
            List<GameNode> winNodes = SolveWithDFS(AddToWinNodes);
            if (winNodes.Count > 0)
            {
                level.Solvable = true;
                winNodes.Sort((node1,node2) =>
                {
                    return GetSolution(node1).Sum(GetMoveCount) - GetSolution(node2).Sum(GetMoveCount);
                });
                ShortestSolution = GetSolution(winNodes[0]);
            }
            else
            {
                level.Solvable = false;
            }
        }

        public void FindFirstSolution(Level level)
        {
            InitVariables(level);
            List<GameNode> winNodes = SolveWithDFS(ReturnFirstWinNode);
            if (Solved)
            {
                FirstSolution = GetSolution(winNodes[0]);
            }
        }

        public void FindShortestSolution(Level level)
        {
            InitVariables(level);
            var winNode = SolveWithBFS();
            if (Solved)
            {
                ShortestSolution = GetSolution(winNode);
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

        private GameNode SolveWithBFS()
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount+1));
            stateQueue.Enqueue(root);
            IEqualityComparer<GameNode> nodeComparer = root;

            var sw = Stopwatch.StartNew();
            while (Solved == false)
            {
                TotalNodeTraversed++;
                GameNode currentNode = stateQueue.Dequeue();
                List<Movement> moves = currentNode.GetValidMoves();
                var childs = moves.Select(move => currentNode.GenerateChildNode(move))
                    .OrderByDescending(node => node.GetScore());

                foreach (var childNode in childs)
                {
#if DEBUG
                    ShowGame(childNode);
#endif
                    if (childNode.IsWin(stackWinCount))
                    {
                        Solved = true;
                        TimeFinished = sw.ElapsedMilliseconds;
                        return childNode;
                    }

                    if (!visited.Contains(childNode, nodeComparer))
                    {
                        visited.Add(childNode);
                        stateQueue.Enqueue(childNode);
                    }
                }
            }

            return null;
        }

        private List<GameNode> SolveWithDFS(Action<List<GameNode>, GameNode> OnWinNodeFound)
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1));
            GameNode currentNode = root;
            List<GameNode> winNodes = new List<GameNode>();
            int leastMoves = Int32.MaxValue;

            sw = Stopwatch.StartNew();
            while (true && Solved == false && (sw.ElapsedMilliseconds / 1000f < TimeLimit))
            {
                TotalNodeTraversed++;
                if (currentNode.MoveCount >= leastMoves - 1)
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
                        if (currentNode.CompareTo(root) == 0) break;
                    }
                }

                if (currentNode.Winnable == false)
                {
                    currentNode = currentNode.Parent;
                    continue;
                }

                var moves = currentNode.GetValidMoves();

                if (moves.Count == 0)
                {
                    currentNode.MarkUnwinnable();
                }
                else
                {
#if DEBUG
                    //ShowGame(currentNode);
#endif
                    currentNode.Childs = new List<GameNode>();

                    foreach (var move in moves)
                    {
                        GameNode newNode = currentNode.GenerateChildNode(move);

                        try
                        {
                            var matchedNode = visited.First(node => node.CompareTo(newNode) == 0);
                            if (matchedNode.MoveCount > newNode.MoveCount)
                            {
                                visited.Remove(matchedNode);

                                if (matchedNode.Parent != null)
                                {
                                    matchedNode.Parent.Childs.Remove(matchedNode);
                                    //matchedNode.Parent = null;
                                }
                                currentNode.Childs.Add(newNode);
                                visited.Add(newNode);
                            }
                        }
                        catch (InvalidOperationException) // can't found new node in Visited
                        {
                            currentNode.Childs.Add(newNode);
                            visited.Add(newNode);
                        }

                        if (newNode.IsWin(stackWinCount))
                        {
                            if (leastMoves > newNode.MoveCount)
                                leastMoves = newNode.MoveCount;

                            OnWinNodeFound.Invoke(winNodes, newNode);
                        }
                    }

                    currentNode.Childs = currentNode.Childs.OrderByDescending(node => node.GetScore()).ToList();
#if DEBUG
                    //foreach (var node in currentNode.Childs)
                    //{
                    //    ShowGame(node);
                    //    Console.WriteLine("Score :" + node.GetScore());
                    //}
#endif
                }
            }
            TimeFinished = sw.ElapsedMilliseconds / 1000f;
            return winNodes;
        }

        private void AddToWinNodes(List<GameNode> winNodes, GameNode newNode)
        {
            winNodes.Add(newNode);
            if (IsFirstSolutionFound == false)
            {
                ShowFirstSolution(newNode);
            } 
        }

        private void ReturnFirstWinNode(List<GameNode> winNodes, GameNode newNode)
        {
            winNodes.Add(newNode);
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
            Console.WriteLine($"{currentState.Movement.From}->{currentState.Movement.To}");
            List<List<int>> map = new List<List<int>>();
            currentState.Stacks.ForEach(stack =>
            {
                map.Add(new Stack<int>(stack).ToList());
            });

            for (int i = STACK_SIZE-1; i >= 0; i--)
            {
                for (int j = 0; j < map.Count; j++)
                {
                    try
                    {
                        string number = map[j][i].ToString("00");
                        Console.Write($"[{number}]");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.Write("[  ]");
                    }
                }
                Console.WriteLine();
            }
        }

        private void InitVariables(Level level)
        {
            var sequence = level.Sequence;
            Stacks = new List<Stack<int>>();
            Solved = false;
            TimeFinished = 0;
            ShortestSolution = new Stack<Movement>();
            FirstSolution = new Stack<Movement>();
            stateQueue = new Queue<GameNode>();
            visited = new SortedSet<GameNode>();
            IsFirstSolutionFound = false;
            TotalNodeTraversed = 0;

            for (int i = 0; i < sequence.Length / STACK_SIZE; i++)
            {
                Stack<int> stack = new Stack<int>(STACK_SIZE);
                for (int j = 0; j < STACK_SIZE; j++)
                {
                    stack.Push(sequence[STACK_SIZE * i + j]);
                }

                Stacks.Add(stack);
            }

            stackWinCount = Stacks.Count;

            Stacks.Add(new Stack<int>(STACK_SIZE));
            Stacks.Add(new Stack<int>(STACK_SIZE));  
        }

        public List<Movement> GetSolutionFormatted(Stack<Movement> solution)
        {
            return solution.ToList();
        }

        private int GetMoveCount(Movement move)
        {
            return move.MoveCount;
        }
    }
}
