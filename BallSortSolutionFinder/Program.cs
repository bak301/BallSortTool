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
    class Program
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

        static int Solve(Level level)
        {
            var sw = Stopwatch.StartNew();
            Solver solver = new Solver();
            solver.SolveLevelWithTree(level);
            Console.WriteLine(sw.ElapsedMilliseconds);
            solver.GetSolutionFormatted().ForEach(mv =>
            {
                Console.Write($" {mv.From}->{mv.To} ");
            });
            return solver.Solution.Count;
        }
    }
}
