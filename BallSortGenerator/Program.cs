using BallSortGeneratorRandomBall;
using BallSortSolutionFinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BallSortGenerator
{
    // Random position
    // Tuple item1 = column index, item 2 = row index
    class Program
    {
        static void Main(string[] args)
        {
            int stackCount = 4;
            int stackSize = 4;
            int levelCount = 1;
            int levelOffset = 0;
            float timeLimit = float.MaxValue;
            string solutionType = "first";
            string path = "";

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-numStack":
                    case "-stackCount":
                        stackCount = int.Parse(args[i + 1]);
                        break;
                    case "-stackSize":
                        stackSize = int.Parse(args[i + 1]);
                        break;
                    case "-levelCount":
                        levelCount = int.Parse(args[i + 1]);
                        break;
                    case "-offset":
                        levelOffset = int.Parse(args[i + 1]);
                        break;
                    case "-type":
                        solutionType = args[i + 1];
                        break;
                    case "-path":
                        path = args[i + 1];
                        break;
                    case "-time":
                        timeLimit = float.Parse(args[i + 1]);
                        break;
                    case "/help":
                        ShowReadMe();
                        return;
                    default:
                        break;
                }
            }
            Level[] levels = new Level[levelCount];

            switch (solutionType)
            {
                case "shortest":
                    ExportShortestSolution(levels, stackSize, levelOffset, timeLimit, stackCount, path);
                    break;
                case "first":
                    ExportFirstSolution(levels, stackSize, levelOffset, timeLimit, stackCount, path);
                    break;
                default:
                    break;
            }
            
            
        }

        private static void ShowReadMe()
        {
            string[] lines = File.ReadAllLines(@".\README.MD");

            // Display the file contents by using a foreach loop.
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }
        }

        private static void ExportShortestSolution(Level[] levels, int stackSize, int offset, float timeLimit, int stackCount, string path)
        {
            for (int i = 0; i < levels.Length;)
            {
                levels[i] = new Level(stackCount, stackSize);
                Solver solver = new Solver(timeLimit);
                solver.InitVariables(levels[i], stackSize);
                Console.WriteLine("*********************** LEVEL " + (i + 1) + " *************************");
                Console.WriteLine($"Start solving {string.Join(',', levels[i].Sequence)} ...");

                solver.FindShortestSolutionBFS();
                Console.WriteLine("Solve time: " + solver.TimeFinished);

                var solution = solver.GetSolutionFormatted(solver.ShortestSolution);

                if (solution.Count > 0)
                {
                    Console.Write("Fastest solution: " + solution.Count + " moves");
                    solution.ForEach(mv =>
                    {
                        Console.Write($" {mv.From}->{mv.To}({mv.MoveCount}) ");
                    });
                    WriteToJSON(path + "\\level_" + (i + 1 + offset) + ".bytes", new LevelJSON(levels[i], solution));
                    i++;
                }
                else
                {
                    Console.WriteLine("Level not solvable ...");
                }

                Console.WriteLine();
                Console.WriteLine($"Total Node Traversed : {solver.TotalNodeTraversed}");
                Console.WriteLine("*******************************************************\n");
            }
        }

        private static void ExportFirstSolution(Level[] levels, int stackSize, int offset, float timeLimit, int stackCount, string path)
        {
            for (int i = 0; i < levels.Length;)
            {
                levels[i] = new Level(stackCount, stackSize);
                Solver solver = new Solver(timeLimit);
                solver.InitVariables(levels[i], stackSize);
                Console.WriteLine("*********************** LEVEL " + (i + 1) + " *************************");
                Console.WriteLine($"Start solving {string.Join(',', levels[i].Sequence)} ...");

                solver.FindFirstSolution();
                Console.WriteLine("Solve time: " + solver.TimeFinished);

                var solution = solver.GetSolutionFormatted(solver.FirstSolution);

                if (solution.Count > 0)
                {
                    Console.Write("First solution: " + solution.Count + " moves");
                    solution.ForEach(mv =>
                    {
                        Console.Write($" {mv.From}->{mv.To}({mv.MoveCount}) ");
                    });
                    WriteToJSON(path + "\\level_" + (i + 1 + offset) + ".bytes", new LevelJSON(levels[i], solution));
                    i++;
                }
                else
                {
                    Console.WriteLine("Level not solvable ...");
                }
                Console.WriteLine();
                Console.WriteLine("Total node traversed: " + solver.TotalNodeTraversed);
            }
        }

        private static void WriteToJSON(string filename, Object obj)
        {
            StreamWriter file = File.CreateText(filename);
            file.Write(JsonConvert.SerializeObject(obj, Formatting.None));
            file.Close();
        }
    }
}
