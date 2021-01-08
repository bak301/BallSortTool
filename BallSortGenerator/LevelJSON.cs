using BallSortGeneratorRandomBall;
using BallSortSolutionFinder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BallSortGenerator
{
    public class LevelJSON
    {
        public int numStack { get; set; }
        public int[] bubbleTypes { get; set; }

        public List<Movement> step { get; set; }

        public LevelJSON(Level level, List<Movement> solution)
        {
            this.numStack = level.StackCount;
            this.bubbleTypes = level.Sequence;
            this.step = solution;
        }

        public LevelJSON()
        {

        }
    }
}
