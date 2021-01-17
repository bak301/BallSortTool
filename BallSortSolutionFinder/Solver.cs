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
        public List<int?[]> Stacks { get; set; }
        public float TimeLimit { get; set; }
        public float TimeFinished { get; set; }
        public Stack<Movement> ShortestSolution { get; private set; }
        public Stack<Movement> FirstSolution { get; private set; }
        public long FirstSolutionTime { get; set; }
        public int TotalNodeTraversed { get; set; }
        
        private int stackWinCount;

        public Solver(float timeLimit = float.MaxValue)
        {
            TimeLimit = timeLimit;
        }

        public void FindFirstSolution()
        {
            var winNode = SolveWithDFS();
            if (winNode != null) FirstSolution = GetSolution(winNode);
        }

        public void FindShortestSolutionBFS()
        {
            var winNodes = SolveWithBFS();
            if (winNodes.Count > 0)
            {
                ShortestSolution = GetSolution(winNodes.Values[0]);
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
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount+1));
            Queue<GameNode> stateQueue = new Queue<GameNode>();
            HashSet<GameNode> visited = new HashSet<GameNode>(root);
            int leastMoves = Int32.MaxValue;
            var sw = Stopwatch.StartNew();

            stateQueue.Enqueue(root);
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

        private GameNode SolveWithDFS()
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1));
            Stack<GameNode> stateStack = new Stack<GameNode>();
            HashSet<GameNode> visited = new HashSet<GameNode>(root);
            var sw = Stopwatch.StartNew();

            stateStack.Push(root);
            while (true)
            {
                TotalNodeTraversed++;
                if (stateStack.Count == 0) break;
                GameNode currentNode = stateStack.Pop();
#if DEBUG
                //ShowGame(currentNode);
#endif
                List<Movement> moves = currentNode.GetValidMoves();

                if (moves.Count > 0)
                {
                    currentNode.Childs = new List<GameNode>(moves.Count);
                    foreach (var move in moves)
                    {
                        GameNode newNode = currentNode.GenerateChildNode(move);
                        if (newNode.IsWin(stackWinCount))
                        {
                            return newNode;
                        }
                        else
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
                            }
                            else
                            {
                                currentNode.Childs.Add(newNode);
                                visited.Add(newNode);
                            }
                        }
                    }
                    var orderedNodes = currentNode.Childs.OrderByDescending(node => node.GetScoreWithNoMoveCountPenalty());
                    foreach (var node in orderedNodes)
                    {
                        stateStack.Push(node);
                    }
                }
            }
            TimeFinished = sw.ElapsedMilliseconds / 1000f;
            return null;
        }

        public static void ShowGame(GameNode currentState, int stackSize)
        {
            Console.WriteLine();
            Console.WriteLine($"{currentState.Movement.From}->{currentState.Movement.To}");

            for (int i = stackSize - 1; i >= 0; i--)
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

        public void InitVariables(Level level, int stackSize)
        {
            Stacks = new List<int?[]>(level.Sequence.Length / stackSize + 2);
            TimeFinished = 0;
            ShortestSolution = new Stack<Movement>();
            FirstSolution = new Stack<Movement>();
            TotalNodeTraversed = 0;

            for (int i = 0; i < level.Sequence.Length / stackSize; i++)
            {
                int?[]stack = new int?[stackSize];
                for (int j = 0; j < stackSize; j++)
                {
                    stack[j] = level.Sequence[stackSize * i + j];
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
