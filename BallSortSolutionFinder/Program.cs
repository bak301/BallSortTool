using BallSortGeneratorRandomBall;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BallSortSolutionFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            string levelJSON;
            using (StreamReader rd = new StreamReader(args[0]))
            {
                levelJSON = rd.ReadToEnd();
            }
            LevelJSON json = JsonConvert.DeserializeObject<LevelJSON>(levelJSON);
            Level level = new Level(json.numStacks, json.bubbleTypes);
            Console.WriteLine(level.StackCount + "\n" + string.Join(",", level.Sequence));

            Solver solver = new Solver();
            //Console.WriteLine($"{solver.solved} + {string.Join(',', solver.Solve(level))}");
            solver.Solve(level);
            Console.WriteLine(solver.solved);
        }
    }
}
