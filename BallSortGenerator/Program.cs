using System;
using System.Collections.Generic;
using System.Linq;

namespace BallSortGenerator
{
    // Random position
    // Tuple item1 = column index, item 2 = row index
    class Program
    {
        static void Main(string[] args)
        {
            int colorCount = int.Parse(args[0]);
            int stackSize = 4;

            Tuple<int, int>[] level = GenerateLevel(colorCount, stackSize);

            Random rnd = new Random();
            level = level.OrderBy(x => rnd.Next()).ToArray();
            
            foreach (var tuple in level)
            {
                Console.WriteLine($"{tuple.Item1},{tuple.Item2}");
            }
        }

        private static Tuple<int, int>[] GenerateLevel(int colorCount, int stackSize)
        {
            Tuple<int, int>[] pool = new Tuple<int, int>[colorCount * stackSize];
            for (int stackIndex = 0; stackIndex < colorCount;stackIndex++)
            {
                for (int rowIndex = 0; rowIndex < stackSize; rowIndex++)
                {
                    pool[stackIndex*stackSize + rowIndex] = new Tuple<int, int>(stackIndex, rowIndex);
                }
            }

            return pool;
        }
    }
}
