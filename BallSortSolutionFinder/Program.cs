using BallSortGeneratorRandomBall;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace BallSortSolutionFinder
{
    public class Program
    {
        static void Main(string[] args)
        {
            string path = @".\";
            string type = "shortest";
            bool IsExport = false;
            int stackSize = 4;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-stackSize":
                        stackSize = int.Parse(args[i + 1]);
                        break;
                    case "-path":
                        path = args[i + 1];
                        break;
                    case "-type":
                        type = args[i + 1];
                        break;
                    case "-export":
                        IsExport = bool.Parse(args[i + 1]);
                        break;
                    default:
                        break;
                }
            }

            Level level;
            using (StreamReader rd = new StreamReader(path))
            {
                string levelText = rd.ReadToEnd();
                LevelJSON json = JsonConvert.DeserializeObject<LevelJSON>(levelText);
                level = new Level(json.numStacks - 2, json.bubbleTypes);
            }

            Console.WriteLine("Move count: " + Solve(level, stackSize, type, IsExport));
        }

        public static int Solve(Level level, int stackSize, string type, bool IsExport)
        {
            
            Stack<Movement> outputSolution;
            Solver solver = new Solver();
            solver.InitVariables(level, stackSize);

            Console.WriteLine();
            switch (type)
            {
                case "first":
                    solver.FindFirstSolution();
                    outputSolution = solver.FirstSolution;
                    break;

                case "shortest":
                    solver.FindShortestSolutionBFS();
                    outputSolution = solver.ShortestSolution;
                    break;
                default:
                    throw new InvalidOperationException("No type of solver are chosen !");
            }

            if (outputSolution.Count > 0)
            {
                Console.Write(type.ToUpperInvariant() + " Solution: ");
                solver.GetSolutionFormatted(outputSolution).ForEach(mv =>
                {
                    Console.Write($" {mv.From}->{mv.To}({mv.MoveCount}) ");
                });
            }
            else
            {
                Console.WriteLine("Level not solvable ...");
            }
            Console.WriteLine($"Time spent : {solver.TimeFinished} seconds");
            Console.WriteLine($"Total Node Traversed: {solver.TotalNodeTraversed}");

            if (IsExport)
            {
                // export here
            }
            return outputSolution.Sum(mv => mv.MoveCount);
        }
    }
}
