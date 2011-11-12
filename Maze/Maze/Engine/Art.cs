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

namespace Maze.Engine
{
  public enum TileType
  {
    Wall = 0, 
    Ceil = 1,
    Floor = 2,
    Jail = 3,
    Hal9000 = 7,
    Eye = 8,
    Computer = 4,
    BrokenWall = 9,
    Window = 10
  }

  public class Art
  {
    const int TILE_SIZE = 32;

    Core core;
    Texture2D tiles;

    public Art(Core core)
    {
      this.core = core;
    }

    public void LoadContent(ContentManager content)
    {
      tiles = content.Load<Texture2D>("Tiles");
    }

    public Texture2D GetTile(TileType tileType)
    {
      Rectangle source = new Rectangle((int)tileType * TILE_SIZE % tiles.Width, (int)tileType * TILE_SIZE / tiles.Width, TILE_SIZE, TILE_SIZE);

      Color[] colors = new Color[TILE_SIZE * TILE_SIZE];
      tiles.GetData<Color>(0, source, colors, 0, colors.Length);

      Texture2D texture = new Texture2D(core.device, TILE_SIZE, TILE_SIZE);
      texture.SetData<Color>(colors);

      return texture;
    }

    public Texture2D GetTiles()
    {
      return tiles;
    }

    public int GetTileSize()
    {
      return TILE_SIZE;
    }
  }
}
