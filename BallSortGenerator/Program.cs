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
        private const int STACK_SIZE = 4;
        private const int LEVEL_COUNT = 5;
        static void Main(string[] args)
        {
            int stackCount = int.Parse(args[0]);
            string path = args[1];
            Level[] levels = new Level[LEVEL_COUNT];

            //TestPerformanceTree(levels, stackCount);
            //TestPerformanceIterative(levels, stackCount);

            Export(levels, stackCount, path);
        }

        private static void Export(Level[] levels, int stackCount, string path)
        {
            for (int i = 0; i < LEVEL_COUNT;)
            {
                levels[i] = new Level(stackCount, STACK_SIZE);
                Solver solver = new Solver();
                Console.WriteLine($"Start solving {string.Join(',', levels[i].Sequence)} ...");

                var sw = Stopwatch.StartNew();
                solver.SolveLevelWithTree(levels[i]);
                Console.WriteLine("Solve time: " + sw.ElapsedMilliseconds);

                var solution = solver.GetSolutionFormatted();

                if (solution.Count() > 0)
                {
                    Console.WriteLine("Level Solved !");
                    WriteToJSON(path + "\\level_" + (i + 1) + ".bytes", new LevelJSON(levels[i], solution));
                    i++;
                } else
                {
                    Console.WriteLine(string.Join(',',levels[i].Sequence) + " is not solvable");
                }
            }
        }

        static void TestPerformanceTree(Level[] levels, int stackCount)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < LEVEL_COUNT; i++)
            {
                levels[i] = new Level(stackCount, STACK_SIZE);
                Solver solver = new Solver();
                solver.SolveLevelWithTree(levels[i]);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
            
        }

        static void TestPerformanceIterative(Level[] levels, int stackCount)
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < LEVEL_COUNT; i++)
            {
                levels[i] = new Level(stackCount, STACK_SIZE);
                Solver solver = new Solver();
                solver.SolveLevelIterative(levels[i]);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void WriteToJSON(string filename, Object obj)
        {
            StreamWriter file = File.CreateText(filename);
            file.Write(JsonConvert.SerializeObject(obj, Formatting.None));
            file.Close();
        }
    }
}
