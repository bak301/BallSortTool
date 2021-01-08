using BallSortGeneratorRandomBall;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallSortSolutionFinder
{
    public class LevelJSON
    {
        public int numStacks { get; set; }
        public int[] bubbleTypes { get; set; }

        public List<Movement> step { get; set; }

        public LevelJSON(Level level, List<Movement> solution)
        {
            this.numStacks = level.StackCount + 2;
            this.bubbleTypes = level.Sequence;
            this.step = solution;
        }

        public LevelJSON()
        {

        }
    }
}
