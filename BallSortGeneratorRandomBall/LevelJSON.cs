using System;
using System.Collections.Generic;
using System.Text;

namespace BallSortGeneratorRandomBall
{
    public class LevelJSON
    {
        public int numStacks { get; set; }
        public int[] bubbleTypes { get; set; }

        public LevelJSON(Level level)
        {
            this.numStacks = level.StackCount;
            this.bubbleTypes = level.Sequence;
        }

        public LevelJSON()
        {

        }
    }
}
