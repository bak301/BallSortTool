using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallSortSolutionFinder
{
    public static class Extension
    {
        public static List<int?[]> CloneList(this List<int?[]> list)
        {
            List<int?[]> cloneStack = new List<int?[]>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                cloneStack.Add(list[i].CloneArray());
            }
            return cloneStack;
        }
        public static int?[] CloneArray(this int?[] stack)
        {
            int?[] newArray = new int?[stack.Length];
            for (int i = 0; i < stack.Length; i++)
            {
                if (stack[i].HasValue)
                {
                    newArray[i] = stack[i];
                } else
                {
                    break;
                }
            }
            return newArray;
        }

        public static bool IsSingleType(this int?[] stack)
        {
            int stackCount = stack.CountStack();
            if (stackCount > 1)
            {
                for (int i = 1; i < stackCount; i++)
                {
                    if (stack[i] != stack[0]) return false;
                }
                return true;
            } else if (stackCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsCompleted(this int?[] stack)
        {
            if (stack.CountStack() == stack.Length)
            {
                for (int i = 0; i < stack.Length - 1; i++)
                {
                    if (stack[i] != stack[i + 1]) return false;
                }

                return true; // all elements are equal to eachother
            }
            else
            {
                return false;
            }
        }

        public static int CountStack(this int?[] stack)
        {
            int counter = 0;
            for (int i = 0; i < stack.Length; i++)
            {
                if (!stack[i].HasValue)
                {
                    return counter;
                } else
                {
                    counter++;
                }
            }
            return counter;
        }

        public static int? Peek(this int?[] stack)
        {
            for (int i = stack.Length - 1; i >= 0 ; i--)
            {
                if (stack[i].HasValue) return stack[i];
            }
            return null;
        }

        public static int? Pop(this int?[] stack)
        {
            for (int i = stack.Length - 1; i >= 0; i--)
            {
                if (stack[i].HasValue)
                {
                    int? result = stack[i];
                    stack[i] = null;
                    return result;
                }
            }
            return null;
        }

        public static void Push(this int?[] stack, int? value)
        {
            for (int i = 0; i < stack.Length; i++)
            {
                if (!stack[i].HasValue)
                {
                    stack[i] = value;
                    break;
                }
            }
        }

        public static long GetIndex(this int?[] stack)
        {
            long baseIndex = 0 ; // 12,11, 10, 11, 11 
            for (int j = 0; j < stack.Length; j++)
            {
                if (stack[j].HasValue)
                {
                    baseIndex += (long)Math.Pow(100, stack.Length - 1 - j) * (stack[j].Value + 1);
                } else
                {
                    break;
                }
            }
            return baseIndex;
        }
    }
}
