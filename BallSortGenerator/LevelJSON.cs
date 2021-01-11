using BallSortGeneratorRandomBall;
using BallSortSolutionFinder;
using System.Collections.Generic;

namespace BallSortGenerator
{
    public class LevelJSON
    {
        public int numStack { get; set; }
        public int[] bubbleTypes { get; set; }

        public List<Movement> step { get; set; }

        public LevelJSON(Level level, List<Movement> solution)
        {
            this.numStack = level.StackCount + 2;
            this.bubbleTypes = level.Sequence;
            this.step = solution;
        }

        public LevelJSON()
        {

        }
    }
}
