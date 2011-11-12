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
  public class BlockPrimitive : Primitive
  {
    public TileType tileType;

    public BlockPrimitive(Core core, TileType tileType)
      : base(core)
    {
      this.tileType = tileType;

      BuildPrimitive();
    }

    protected override void BuildPrimitive()
    {
      vertices = new VertexPositionNormalTexture[36];

      // Calculate the position of the vertices on the top face
      Vector3 topLeftFront = new Vector3(.0f, 1f, .0f);
      Vector3 topLeftBack = new Vector3(.0f, 1f, 1f);
      Vector3 topRightFront = new Vector3(1f, 1f, .0f);
      Vector3 topRightBack = new Vector3(1f, 1f, 1f);

      // Calculate the position of the vertices on the bottom face
      Vector3 btmLeftFront = new Vector3(.0f, .0f, .0f);
      Vector3 btmLeftBack = new Vector3(.0f, .0f, 1f);
      Vector3 btmRightFront = new Vector3(1f, .0f, .0f);
      Vector3 btmRightBack = new Vector3(1f, .0f, 1f);

      // Normal vectors for each face (needed for lighting / display)
      Vector3 normalFront = new Vector3(.0f, .0f, 1f);
      Vector3 normalBack = new Vector3(.0f, .0f, -1f);
      Vector3 normalTop = new Vector3(.0f, 1f, .0f);
      Vector3 normalBottom = new Vector3(.0f, -1f, .0f);
      Vector3 normalLeft = new Vector3(-1f, .0f, .0f);
      Vector3 normalRight = new Vector3(1f, .0f, .0f);

      // UV texture coordinates
      float u = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Width;
      float v = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Height;

      float integral = (float)Math.Truncate(u * (int)tileType);
      float x = u * (int)tileType - integral;
      float y = v * integral;

      Vector2 textureTopLeft = new Vector2(x + u, y);
      Vector2 textureTopRight = new Vector2(x, y);
      Vector2 textureBottomLeft = new Vector2(x + u, y + v);
      Vector2 textureBottomRight = new Vector2(x, y + v);

      int n = 0;

      /*
      // Add the vertices for the FRONT face
      vertices[n++] = new VertexPositionNormalTexture(topLeftFront, normalFront, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftFront, normalFront, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmRightFront, normalFront, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight);
      
      // Add the vertices for the BACK face
      vertices[n++] = new VertexPositionNormalTexture(topLeftBack, normalBack, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftBack, normalBack, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmRightBack, normalBack, textureBottomLeft);
      */

      // Add the vertices for the TOP face
      vertices[n++] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(topLeftBack, normalTop, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(topRightFront, normalTop, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight);

      // Add the vertices for the BOTTOM face
      vertices[n++] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureBottomRight); 
      vertices[n++] = new VertexPositionNormalTexture(btmLeftBack, normalBottom, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftFront, normalBottom, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(btmRightBack, normalBottom, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmRightFront, normalBottom, textureBottomLeft);


      // Add the vertices for the LEFT face
      vertices[n++] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftFront, normalLeft, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(topLeftBack, normalLeft, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(btmLeftBack, normalLeft, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(topLeftFront, normalLeft, textureBottomRight);

      // Add the vertices for the RIGHT face
      vertices[n++] = new VertexPositionNormalTexture(topRightFront, normalRight, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmRightFront, normalRight, textureBottomRight);
      vertices[n++] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureTopRight);
      vertices[n++] = new VertexPositionNormalTexture(topRightBack, normalRight, textureTopLeft);
      vertices[n++] = new VertexPositionNormalTexture(topRightFront, normalRight, textureBottomLeft);
      vertices[n++] = new VertexPositionNormalTexture(btmRightBack, normalRight, textureTopRight);
    }
  }
}
