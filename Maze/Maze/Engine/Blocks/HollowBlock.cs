using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Engine.Blocks
{
  public class HollowBlock : Block
  {
    public HollowBlock(Core core, int id) 
        : base(core, id)
    {
      blocksMotion = false;
    }
  }
}
