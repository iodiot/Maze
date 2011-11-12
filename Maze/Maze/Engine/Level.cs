#region Using statements

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

using Maze.Engine.Blocks;
using Maze.Engine.Entities;
using Maze.Engine.Primitives;

#endregion

namespace Maze.Engine
{
  public class Level
  {
    #region Fields

    const int eventHorizon = -1;

    public Matrix worldMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(MathHelper.Pi);
    public Matrix inverseWorldMatrix = Matrix.Invert(Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(MathHelper.Pi));

    static VertexPositionNormalTexture[] defaultPlane = GetPlane();

    Core core;

    int width = -1;
    int height = -1;

    Dictionary<int, Block> blocks;
    int currentId = 1;
    int[,] map;

    List<VertexPositionNormalTexture> levelVertices;
    List<VertexPositionNormalTexture> billboardVertices;

    List<Entity> entities = new List<Entity>();

    public int playerSpawnX = -1;
    public int playerSpawnY = -1;

    #endregion

    #region Initialization

    public Level(Core core)
    {
      this.core = core;
    }

    /// <summary>
    /// Build level from bitmap file
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    public void LoadContent(ContentManager content, string name)
    {
      blocks = new Dictionary<int, Block>();

      Texture2D texture = content.Load<Texture2D>("Levels\\" + name);

      width = texture.Width;
      height = texture.Height;

      Color[] colors = new Color[width * height];
      texture.GetData(colors);

      // Flood fill outdoor empty regions of map with special color
      FloodFill(colors, 0, 0, width, height, colors[0], Color.Magenta);

      // Creates map with id`s where positive numbers mean pointer to some block
      map = new int[width, height];

      for (int i = 0; i < width; ++i)
        for (int j = 0; j < height; ++j)
        {
          Color c = colors[i + j * width];
          if (c != Color.Magenta)
            AddBlock(ColorToBlock(c), i, j);
          else
            map[i, j] = -1;

          if (c == Color.Red)
          {
            playerSpawnX = i;
            playerSpawnY = j;
          }

          if (c == Color.Cyan)
          {
            entities.Add(new JailEntity(core, new Vector3(i + .5f, j + .5f, 0)));
          }

          if (c == Color.Yellow)
            entities.Add(new EyeEntity(core, new Vector3(i, j, 0)));

          if (c == Color.Blue)
            entities.Add(new BlockEntity(core, new Vector3(i, j, 0)));
        }

      BuildLevelVertices();
    }

    private static void FloodFill(Color[] colors, int x, int y, int width, int height, Color emptyColor, Color fillColor)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
        return;

      if (colors[x + y * width] == emptyColor)
      {
        colors[x + y * width] = fillColor;
        FloodFill(colors, x - 1, y, width, height, emptyColor, fillColor);
        FloodFill(colors, x + 1, y, width, height, emptyColor, fillColor);
        FloodFill(colors, x, y - 1, width, height, emptyColor, fillColor);
        FloodFill(colors, x, y + 1, width, height, emptyColor, fillColor);
      }
    }

    #endregion

    #region Coords

    public Vector3 WorldToLevel(Vector3 v)
    {
      return Vector3.Transform(v, inverseWorldMatrix);
    }

    public Vector3 LevelToWorld(Vector3 v)
    {
      return Vector3.Transform(v, worldMatrix);
    }

    #endregion

    #region Block operations

    public void AddBlock(Block b, int x, int y)
    {
      map[x, y] = b.id;
      blocks.Add(b.id, b);
    }

    public Block GetBlock(int x, int y)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
        return null;

      if (map[x, y] == -1)
        return null;

      return blocks[map[x, y]];
    }

    public Block ColorToBlock(Color c)
    {
      if (c == Color.Black)
        return new SolidBlock(core, currentId++);
      else
        return new HollowBlock(core, currentId++);
    }

    #endregion

    #region Vertice operations

    public static VertexPositionNormalTexture[] GetPlane()
    {
      VertexPositionNormalTexture[] plane = new VertexPositionNormalTexture[6];
      int n = 0;

      // First triangle
      plane[n++] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(1, 1));
      plane[n++] = new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.Zero, new Vector2(1, 0));
      plane[n++] = new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Zero, new Vector2(0, 1));

      // Second triangle
      plane[n++] = new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Zero, new Vector2(0, 0));
      plane[n++] = new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Zero, new Vector2(0, 1));
      plane[n++] = new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.Zero, new Vector2(1, 0));

      for (int i = 0; i < n; ++i)
        plane[i].Normal = Vector3.Cross(plane[1].Position - plane[0].Position, plane[2].Position - plane[1].Position);

      return plane;
    }

    public VertexPositionNormalTexture[] GetPlane(TileType tileType, float radiansX, float radiansZ, Vector3 translation)
    {
      Matrix transformMatrix = Matrix.CreateRotationX(radiansX) * Matrix.CreateRotationZ(radiansZ) * Matrix.CreateTranslation(translation);
      VertexPositionNormalTexture[] resultPlane = defaultPlane.Clone() as VertexPositionNormalTexture[];

      for (int i = 0; i < resultPlane.Length; ++i)
      {
        resultPlane[i].Position = Vector3.Transform(defaultPlane[i].Position, transformMatrix);

        float u = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Width;
        float v = (float)core.art.GetTileSize() / (float)core.art.GetTiles().Height;

        float integral = (float)Math.Truncate(u * (int)tileType);
        float x = u * (int)tileType - integral;
        float y = v * integral;

        resultPlane[i].TextureCoordinate = defaultPlane[i].TextureCoordinate * new Vector2(u, v) + new Vector2(x, y);
      }

      return resultPlane;
    }

    public void BuildBlockVertices(int x, int y)
    {
      Block b = GetBlock(x, y);
      if (b == null)
        return;

      if (b is HollowBlock)
      {
        levelVertices.AddRange(GetPlane(TileType.Floor, 0, 0, new Vector3(x, y, 0)));
        levelVertices.AddRange(GetPlane(TileType.Ceil, MathHelper.Pi, 0, new Vector3(x, y + 1, 1)));
      }

      if (b is SolidBlock)
      {
        Block bottom = GetBlock(x, y + 1);
        if (bottom != null && !(bottom is SolidBlock))
          levelVertices.AddRange(GetPlane(TileType.Wall, MathHelper.PiOver2, MathHelper.Pi, new Vector3(x + 1, y + 1, 0)));

        Block top = GetBlock(x, y - 1);
        if (top != null && !(top is SolidBlock))
          levelVertices.AddRange(GetPlane(TileType.Wall, MathHelper.PiOver2, 0, new Vector3(x, y, 0)));

        Block left = GetBlock(x - 1, y);
        if (left != null && !(left is SolidBlock))
          levelVertices.AddRange(GetPlane(TileType.Wall, MathHelper.PiOver2, MathHelper.Pi + MathHelper.PiOver2, new Vector3(x, y + 1, 0)));

        Block right = GetBlock(x + 1, y);
        if (right != null && !(right is SolidBlock))
          levelVertices.AddRange(GetPlane(TileType.Wall, MathHelper.PiOver2, MathHelper.PiOver2, new Vector3(x + 1, y, 0)));
      }
    }

    private void BuildLevelVertices()
    {
      levelVertices = new List<VertexPositionNormalTexture>();

      for (int i = 0; i < width; ++i)
        for (int j = 0; j < height; ++j)
          BuildBlockVertices(i, j);

      for (int i = 0; i < entities.Count; ++i)
      {
        List<Primitive> primitives = entities[i].GetPrimitives();
        for (int j = 0; j < primitives.Count; ++j)
        {
          if ((entities[i] is BlockEntity))
            levelVertices.AddRange(primitives[j].GetVertices(entities[i].position));
        }
      }
    }

    private void BuildEntities()
    {
      for (int i = 0; i < entities.Count; ++i)
      {
        List<Primitive> primitives = entities[i].GetPrimitives();
        for (int j = 0; j < primitives.Count; ++j)
        {
          if (!(entities[i] is BlockEntity))
            billboardVertices.AddRange(primitives[j].GetVertices(entities[i].position));
        }
      }
    }

    #endregion

    #region Update and draw

    private void HandleInput()
    {
      KeyboardState state = Keyboard.GetState();

      if (state.IsKeyDown(Keys.Space))
      {

      }
    }

    public void Update(GameTime gameTime)
    {
      HandleInput();

      // Update entities
      for (int i = 0; i < entities.Count; ++i)
        entities[i].Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
      billboardVertices = new List<VertexPositionNormalTexture>();
      BuildEntities();

      core.device.SamplerStates[0] = SamplerState.PointClamp;
      core.device.Textures[0] = core.art.GetTiles();


      VertexBuffer vertexBuffer = new VertexBuffer(core.device, VertexPositionNormalTexture.VertexDeclaration, levelVertices.Count, BufferUsage.WriteOnly);
      vertexBuffer.SetData<VertexPositionNormalTexture>(levelVertices.ToArray());

      core.customEffect.CurrentTechnique = core.customEffect.Techniques["AllInOne"];
      core.customEffect.Parameters["xBillboardEnabled"].SetValue(false);
      core.customEffect.CurrentTechnique.Passes[0].Apply();

      core.device.SetVertexBuffer(vertexBuffer);
      core.device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);

      //
      vertexBuffer = new VertexBuffer(core.device, VertexPositionNormalTexture.VertexDeclaration, billboardVertices.Count, BufferUsage.WriteOnly);
      vertexBuffer.SetData<VertexPositionNormalTexture>(billboardVertices.ToArray());

      core.customEffect.CurrentTechnique = core.customEffect.Techniques["AllInOne"];
      core.customEffect.Parameters["xBillboardEnabled"].SetValue(true);
      core.customEffect.Parameters["xFogEnabled"].SetValue(false);
      core.customEffect.CurrentTechnique.Passes[0].Apply();

      core.device.SetVertexBuffer(vertexBuffer);
      core.device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
    }

    #endregion
  }
}
