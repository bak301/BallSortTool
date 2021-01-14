using BallSortGeneratorRandomBall;
using Newtonsoft.Json;
using System;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace BallSortSolutionFinder
{
    public class Program
    {

        static void Main(string[] args)
        {
            string path = @".\";
            string type = "shortest";
            bool IsExport = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
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

            Console.WriteLine("Move count: " + Solve(level, type, IsExport));
        }

        public static int Solve(Level level, string type, bool IsExport)
        {
            Console.WriteLine();
            Stopwatch sw = Stopwatch.StartNew();
            Solver solver = new Solver();

            Stack<Movement> outputSolution;

            if (type == "first")
            {
                solver.FindFirstSolution(level);
                outputSolution = solver.FirstSolution;
            } else
            {
                solver.FindShortestSolution(level);
                outputSolution = solver.FastestSolution;
                Console.WriteLine($"Total Elapsed Time to traverse all node : {sw.ElapsedMilliseconds}");
            }

            if (outputSolution.Count > 0)
            {
                Console.Write(type.ToUpper() + " Solution: ");
                solver.GetSolutionFormatted(outputSolution).ForEach(mv =>
                {
                    Console.Write($" {mv.From}->{mv.To} ");
                });
            }
            else
            {
                Console.WriteLine("Level not solvable ...");
            }

            if (IsExport)
            {
                // export here
            }

            Console.WriteLine($"Total Node Traversed : {solver.TotalNodeTraversed}");
            return outputSolution.Sum(mv => mv.MoveCount);
        }
    }
}
