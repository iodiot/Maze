using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Maze.Engine.Primitives;

namespace Maze.Engine.Blocks
{
  public class Block
  {
    public bool blocksMotion = false;
    public float friction = .0f;

    protected List<Primitive> primitives = new List<Primitive>();
    protected Core core;

    public Block(Core core)
    {
      this.core = core;
    }
  }
}
