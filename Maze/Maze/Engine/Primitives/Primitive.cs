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

namespace Maze.Engine.Primitives
{
  public abstract class Primitive
  {
    protected Core core;
    protected VertexPositionNormalTexture[] vertices;

    public Primitive(Core core)
    {
      this.core = core;
    }

    protected virtual void BuildPrimitive()
    {
    }

    public virtual VertexPositionNormalTexture[] GetVertices(Vector3 translation)
    {
      VertexPositionNormalTexture[] r = vertices.Clone() as VertexPositionNormalTexture[];

      for (int i = 0; i < vertices.Length; ++i)
        r[i].Position += translation;

      return r;
    }
  }
}
