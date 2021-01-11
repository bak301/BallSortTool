using System.Collections.Generic;

namespace BallSortSolutionFinder
{
    public static class Extension
    {
        public static List<int> Clone(this List<int> list)
        {
            return new List<int>(list);
        }

        public static bool IsSingleType(this List<int> list)
        {
            if (list.Count == 1)
            {
                return true;
            }
            if (list.Count > 1)
            {
                int reference = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    if (reference != list[i]) return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
