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

        private const int STACK_SIZE = 4;
        static void Main(string[] args)
        {
            string path = args[0];

            
            Level level;
            using (StreamReader rd = new StreamReader(path))
            {
                string levelText = rd.ReadToEnd();
                LevelJSON json = JsonConvert.DeserializeObject<LevelJSON>(levelText);
                level = new Level(json.numStacks - 2, json.bubbleTypes);
            }

            Console.WriteLine(Solve(level));

            //for (int i = 0; i < 5; i++)
            //{
            //    Solve(level);
            //}
        }

        public static int Solve(Level level)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Solver solver = new Solver();
            solver.FindShortestSolution(level);
            
            if (solver.FastestSolution.Count > 0)
            {
                Console.Write("Fastest solution: ");
                solver.GetSolutionFormatted(solver.FastestSolution).ForEach(mv =>
                {
                    Console.Write($" {mv.From}->{mv.To} ");
                });
            } else
            {
                Console.WriteLine("Level not solvable ...");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Total Elapsed Time to traverse all node : {sw.ElapsedMilliseconds}");
            Console.WriteLine($"Total Node Traversed : {solver.TotalNodeTraversed}");
            return solver.FastestSolution.Count;
        }
    }
}
