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
  public class EyeEntity : Entity
  {
    Vector3 originalPosition;

    public EyeEntity(Core core, Vector3 position) : base(core, position)
    {
      primitives.Add(new BillboardPrimitive(core, TileType.Eye));

      originalPosition = position;
    }

    public override void Update(GameTime gameTime)
    {
      // Add oscillation
      position.X = originalPosition.X + (float)Math.Sin(gameTime.TotalGameTime.Milliseconds / (100.0f * MathHelper.Pi)) * .2f;
      position.Y = originalPosition.Y +  (float)Math.Sin(gameTime.TotalGameTime.Milliseconds / (100.0f * MathHelper.Pi)) * .2f; 

      base.Update(gameTime);
    }
  }
}
