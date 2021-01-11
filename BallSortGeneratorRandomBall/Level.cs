using System;
using System.Linq;

namespace BallSortGeneratorRandomBall
{
    public class Level
    {
        public int StackCount { get; set; }
        public int[] Sequence { get; set; }

        public bool Solvable { get; set; }

        public Level()
        {

        }

        public Level(int stackCount, int stackSize)
        {
            this.StackCount = stackCount;
            this.Sequence = GetSequence(stackCount, stackSize);
        }

        public Level(int stackCount, int[] sequence)
        {
            this.StackCount = stackCount;
            this.Sequence = sequence;
        }

        private int[] GetSequence(int stackCount, int stackSize)
        {
            int ballCount = stackCount * stackSize;
            int[] array = new int[ballCount];

            for (int i = 0; i < ballCount; i++)
            {
                array[i] = i / stackSize;
            }

            Random rnd = new Random();
            do
            {
                array = array.OrderBy(x => rnd.Next()).ToArray();
            } while (IsSequenceNotValid(array, stackSize));

            return array;
        }

        private bool IsSequenceNotValid(int[] array, int stackSize)
        {
            bool validationResult = false;
            for (int i = 0; i < array.Length; i += stackSize)
            {
                //validationResult =     array[i]     == array[i + 1]
                //                    && array[i + 1] == array[i + 2]
                //                    && array[i + 2] == array[i + 3];
                validationResult = IsElementEqual(array, i, stackSize, 1);
                if (validationResult == true)
                    break;
            }
            return validationResult;
        }

        private bool IsElementEqual(int[] array, int index, int maxCountComparison, int counter)
        {
            if (counter < maxCountComparison)
            {
                if (array[index] == array[index + 1])
                {
                    return IsElementEqual(array, index + 1, maxCountComparison, counter + 1);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
