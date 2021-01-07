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
        public List<Stack<int>> stacks { get; set; }

        public bool solved { get; set; }

        private Stack<Movement> solution;
        private SortedSet<GameState> visited;
        private Stack<GameState> states;

        public Solver()
        {
            stacks = new List<Stack<int>>();
        }

        public void Solve(Level level)
        {
            InitVariables(level);
            SolveIteratively();
        }

        private void SolveIteratively()
        {
            states.Push(new GameState(stacks, new Movement(5, 6)));
            while (solved == false)
            {
                GameState currentState = states.Pop();
                List<Movement> moves = GetAvailableMovement(currentState);
                
                //ShowGame(currentState); uncomment this too see machine solve the problem

                foreach (var move in moves)
                {
                    List<Stack<int>> newStackState = new List<Stack<int>>();
                    for (int i = 0; i < currentState.stacks.Count; i++)
                    {
                        newStackState.Add(new Stack<int>(new Stack<int>(currentState.stacks[i])));
                    }

                    Move(move, newStackState);

                    if (IsWin(newStackState))
                    {
                        solved = true;
                        return;
                    }

                    GameState newGameState = new GameState(newStackState, move);
                    if (!visited.Contains(newGameState))
                    {
                        visited.Add(newGameState);
                        states.Push(newGameState);
                    }
                }
            }
        }

        private void ShowGame(GameState currentState)
        {
            Console.WriteLine($"{currentState.movement.From}->{currentState.movement.To}");
            List<List<int>> map = new List<List<int>>();
            currentState.stacks.ForEach(stack =>
            {
                map.Add(new Stack<int>(stack).ToList());
            });

            for (int i = STACK_SIZE-1; i >= 0; i--)
            {
                for (int j = 0; j < map.Count; j++)
                {
                    try
                    {
                        Console.Write(map[j][i] + ",");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.Write("N,");
                    }
                }
                Console.WriteLine();
            }
        }

        private void InitVariables(Level level)
        {
            var sequence = level.Sequence;
            states = new Stack<GameState>();
            visited = new SortedSet<GameState>();

            for (int i = 0; i < sequence.Length / STACK_SIZE; i++)
            {
                Stack<int> stack = new Stack<int>(STACK_SIZE);
                stack.Push(sequence[STACK_SIZE * i]);
                stack.Push(sequence[STACK_SIZE * i + 1]);
                stack.Push(sequence[STACK_SIZE * i + 2]);
                stack.Push(sequence[STACK_SIZE * i + 3]);

                stacks.Add(stack);
            }

            stacks.Add(new Stack<int>(STACK_SIZE));
            stacks.Add(new Stack<int>(STACK_SIZE));
        }

        private List<Movement> GetAvailableMovement(GameState state)
        {
            Movement rewindMove = new Movement(state.movement.To, state.movement.From);
            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < state.stacks.Count; i++)
            {
                for (int j = 0; j < state.stacks.Count; j++)
                {
                    if (state.stacks[i] != state.stacks[j])
                    {
                        Movement move = new Movement(i, j);
                        if (move.IsValid(state.stacks)
                            && move.To != rewindMove.To
                            && move.From != rewindMove.From) 
                            
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
                if (IsStackCompleted(stack)) counter++;
            }

            return counter == STACK_SIZE;
        }

        private bool IsStackCompleted(Stack<int> referenceStack)
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
