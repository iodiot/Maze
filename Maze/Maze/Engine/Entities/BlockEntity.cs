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

namespace Maze.Engine.Entities
{
  public class BlockEntity : Entity
  {
    public BlockEntity(Core core, Vector3 position)
      : base(core, position)
    {
      primitives.Add(new BlockPrimitive(core, TileType.Computer));
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
    }
  }
}
