using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace BallSortGeneratorRandomBall
{
    class Program
    {
        static void Main(string[] args)
        {
            int stackCount = int.Parse(args[0]);
            int stackSize = 4;
            Level[] levels = new Level[100];
            
            for (int i = 0; i < 100; i++)
            {
                levels[i] = new Level(stackCount, stackSize);
                Console.WriteLine($"Number of colors : {stackCount} \t {levels[i].Sequence}");
                WriteToJSON(args[1] + "\\Levels\\level_" + (i + 1) + ".bytes", new LevelJSON(levels[i]));
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
