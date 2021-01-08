using BallSortGeneratorRandomBall;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BallSortSolutionFinder
{
    public class Solver
    {

        private const int STACK_SIZE = 4;
        public List<Stack<int>> Stacks { get; set; }

        public bool Solved { get; set; }

        public Stack<Movement> Solution { get; private set; }

        private SortedSet<GameNode> visited;
        private Stack<GameNode> states;
        private int stackWinCount;

        public Solver()
        {
            Stacks = new List<Stack<int>>();
        }

        public void SolveLevelWithTree(Level level)
        {
            InitVariables(level);
            GameNode winNode = SolveWithTree();
            if (winNode != null)
            {
                Solution = GetSolution(winNode);
            }
        }

        public void SolveLevelIterative(Level level)
        {
            InitVariables(level);
            GameNode winNode = SolveIteratively();
        }

        private Stack<Movement> GetSolution(GameNode winNode)
        {
            Stack<Movement> moves = new Stack<Movement>();
            GetMoveRecursive(moves, winNode);
            return moves;
        }

        private void GetMoveRecursive(Stack<Movement> moves, GameNode node)
        {
            moves.Push(node.Movement);
            if (node.Parent != null)
            {
                GetMoveRecursive(moves, node.Parent);
            }
        }

        private GameNode SolveIteratively()
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount+1));
            states.Push(root);

            while (Solved == false)
            {
                GameNode currentNode = states.Pop();
                List<Movement> moves = GetAvailableMovement(currentNode);
                
                //ShowGame(currentNode); //uncomment this too see machine solve the problem

                foreach (var move in moves)
                {
                    List<Stack<int>> newStackState = new List<Stack<int>>();
                    for (int i = 0; i < currentNode.Stacks.Count; i++)
                    {
                        newStackState.Add(currentNode.Stacks[i].Clone());
                    }

                    Move(move, newStackState);

                    GameNode newNode = new GameNode(newStackState, move, currentNode);

                    if (IsWin(newStackState))
                    {
                        Solved = true;
                        //ShowGame(newNode);
                        return newNode;
                    }

                    if (!visited.Contains(newNode))
                    {
                        visited.Add(newNode);
                        states.Push(newNode);
                    }
                }
            }

            return null;
        }

        private GameNode SolveWithTree()
        {
            GameNode root = new GameNode(Stacks, new Movement(stackWinCount, stackWinCount + 1));
            GameNode currentNode = root;
            while (Solved == false)
            {

                if (currentNode.HaveChild())
                {
                    if (currentNode.Childs.Where(node => node.Winnable == true).Count() > 0)
                    {
                        currentNode = currentNode.Childs.First(node => node.Winnable == true);
                    }
                    else
                    {
                        currentNode.MarkUnwinnable();
                        currentNode = currentNode.Parent;
                    }

                    continue;
                }

                List<Movement> moves = GetAvailableMovement(currentNode);
                //RemoveUnwinnable(moves, currentNode); //not sure if this is necessary, test performance was not too much different

                if (moves.Count == 0)
                {
                    currentNode.MarkUnwinnable();
                    currentNode = currentNode.Parent;
                    visited.Add(currentNode);
                    continue;
                }
                else
                {
                    ShowGame(currentNode);
                    currentNode.Childs = new List<GameNode>();

                    foreach (var move in moves)
                    {
                        List<Stack<int>> newStackState = new List<Stack<int>>();
                        for (int i = 0; i < currentNode.Stacks.Count; i++)
                        {
                            newStackState.Add(currentNode.Stacks[i].Clone());
                        }

                        Move(move, newStackState);
                        GameNode newNode = new GameNode(newStackState, move, currentNode);

                        if (!visited.Contains(newNode))
                            currentNode.Childs.Add(newNode);

                        if (IsWin(newStackState))
                        {
                            Solved = true;
                            ShowGame(newNode);
                            return newNode;
                        }
                    }

                    if (!visited.Contains(currentNode)) visited.Add(currentNode);
                }
            }

            return null;
        }

        private void RemoveUnwinnable(List<Movement> moves, GameNode currentNode)
        {
            // Check if currentNode have failed move in moves

            if (currentNode.Childs != null)
            {
                moves = moves.Where(move => currentNode.Childs
                                                .Where(node => node.Winnable == true)
                                                .Select(node => node.Movement)
                                                .Contains(move) == false)
                    .ToList();
            }
        }

        public void ShowGame(GameNode currentState)
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
                        Console.Write($"[{map[j][i]}]");
                    }
                    catch (ArgumentOutOfRangeException)
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
            Solution = new Stack<Movement>();
            states = new Stack<GameNode>();
            visited = new SortedSet<GameNode>();

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

        private List<Movement> GetAvailableMovement(GameNode state)
        {
            Movement rewindMove = new Movement(state.Movement.To, state.Movement.From);

            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < state.Stacks.Count; i++)
            {
                for (int j = 0; j < state.Stacks.Count; j++)
                {
                    if (state.Stacks[i] != state.Stacks[j])
                    {
                        Movement move = new Movement(i, j);
                        if (move.IsValid(state.Stacks)
                            && (move.To != rewindMove.To || move.From != rewindMove.From)) 
                            
                            moves.Add(move);
                    }
                }
            }

            return moves;
        }

        private void Move(Movement move, List<Stack<int>> gameState)
        {
            int pickedNumber = gameState[move.From].Pop();
            gameState[move.To].Push(pickedNumber);
        }

        private bool IsWin(List<Stack<int>> gameState)
        {
            int counter = 0;
            foreach (var stack in gameState)
            {
                if (Solver.IsStackCompleted(stack)) 
                    counter++;
            }

            return counter == stackWinCount;
        }

        public List<Movement> GetSolutionFormatted()
        {
            return Solution.ToList();
        }


        // Utility
        public static bool IsStackCompleted(Stack<int> referenceStack)
        {
            List<int> numbers = referenceStack.ToList();
            if (numbers.Count == STACK_SIZE)
            {
                for (int i = 0; i < STACK_SIZE-1; i++)
                {
                    if (numbers[i] != numbers[i + 1]) return false;
                }

                return true; // all elements are equal to eachother
            } else
            {
                return false;
            }
        }
    }
}
