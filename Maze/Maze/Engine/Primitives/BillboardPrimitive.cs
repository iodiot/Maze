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
  public class BillboardPrimitive : Primitive
  {
    public TileType tileType;

    public BillboardPrimitive(Core core, TileType tileType)
      : base(core)
    {
      this.tileType = tileType;

      BuildPrimitive();
    }

    protected override void BuildPrimitive()
    {
      const float MAGIC_NUMBER = 666f;

      vertices = new VertexPositionNormalTexture[6];
      int n = 0;

      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(1, 1));
      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(1, 0));
      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(0, 1));
      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(0, 0));
      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(0, 1));
      vertices[n++] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Zero, new Vector2(1, 0));

      for (int i = 0; i < vertices.Length; ++i)
      {
        float u = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Width;
        float v = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Height;

        float integral = (float)Math.Truncate(u * (int)tileType);
        float x = u * (int)tileType - integral;
        float y = v * integral;

        vertices[i].TextureCoordinate = vertices[i].TextureCoordinate * new Vector2(u, v) + new Vector2(x, y) + vertices[i].TextureCoordinate * MAGIC_NUMBER;
      }
    }
  }
}
