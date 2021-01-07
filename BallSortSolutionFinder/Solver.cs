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

        private List<Stack<int>> currentGameState;

        private bool solved;

        private Queue<Movement> solution;
        private Queue<List<Stack<int>>> gameStates;

        public Solver()
        {
            stacks = new List<Stack<int>>();
        }

        public Queue<Movement> Solve(Level level)
        {
            InitVariables(level);
            solved = false;
            solution = new Queue<Movement>();
            while (solved == false)
            {
                RecursiveSolution(currentGameState);
            }

            return null;
        }

        private void RecursiveSolution(List<Stack<int>> state)
        {
            List<Movement> moves = GetAvailableMovement(state);

            if (moves.Count == 0)
            {
                // no available move
                gameStates.Dequeue();
                solution.Dequeue();
            }

            foreach (var move in moves)
            {
                var tempState = new List<Stack<int>>(state);
                Move(move, tempState);

                gameStates.Enqueue(tempState);
                solution.Enqueue(move);

                if (IsWin(tempState))
                {
                    solved = true;
                    return;
                }
                else
                {
                    RecursiveSolution(state);
                }
            }
        }

        private void InitVariables(Level level)
        {
            currentGameState = stacks;
            var sequence = level.Sequence;

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

        private List<Movement> GetAvailableMovement(List<Stack<int>> gameState)
        {
            List<Movement> moves = new List<Movement>();
            for (int i = 0; i < gameState.Count; i++)
            {
                for (int j = 0; j < gameState.Count; j++)
                {
                    if (gameState[i] != gameState[j])
                    {
                        Movement move = new Movement(i, j);
                        if (move.IsValid(gameState)) 
                            moves.Add(move);
                    }
                }
            }

            return moves;
        }

        private void Move(Movement move, List<Stack<int>> gameState)
        {
            if (move.IsValid(gameState))
            {
                int pickedNumber = currentGameState[move.From].Pop();
                currentGameState[move.To].Push(pickedNumber);          
            }
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
            Stack<int> tempStack = new Stack<int>(referenceStack);
            if (tempStack.Count == STACK_SIZE)
            {
                while (tempStack.Count > 0)
                {
                    var number = tempStack.Pop();
                    if (number != tempStack.Peek())
                    {
                        return false;
                    }
                }

                return true; // all elements are equal to eachother
            } else
            {
                return false;
            }
        }
    }
}
