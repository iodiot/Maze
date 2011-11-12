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
  public class Entity
  {
    public Vector3 position = Vector3.Zero;

    protected Core core;

    protected List<Primitive> primitives = new List<Primitive>();

    public Entity(Core core, Vector3 position)
    {
      this.core = core;
      this.position = position;
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public List<Primitive> GetPrimitives()
    {
      return primitives;
    }

    public virtual void Use()
    {

    }
  }
}
