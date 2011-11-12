using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze.Engine.Blocks
{
  public class SolidBlock : Block
  {
    public SolidBlock(Core core)
      : base(core)
    {
      
      blocksMotion = true;
    }
  }
}
