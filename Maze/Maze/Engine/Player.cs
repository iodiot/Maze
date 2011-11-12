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

namespace Maze.Engine
{
  public class Player
  {
    const float speed = .05f;
    const float lookAhead = .5f;

    Core core;

    public Matrix projectionMatrix;
    public Matrix viewMatrix;
    public BoundingFrustum viewFrustum;
    public Vector3 position;

    public Matrix rotation;

    public Vector3 target = new Vector3(0, 0, .5f);
    float roll = .0f;
    float oscillation = .0f;

    public Player(Core core)
    {
      this.core = core;

      position = core.level.LevelToWorld(new Vector3(core.level.playerSpawnX, core.level.playerSpawnY, .5f));

      projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, core.device.Viewport.AspectRatio, .1f, 100f);
      viewMatrix = Matrix.Identity;

      rotation = Matrix.Identity;
    }

    public int GetX()
    {
      return (int)core.level.WorldToLevel(position).X;
    }

    public int GetY()
    {
      return (int)core.level.WorldToLevel(position).Y;
    }

    private bool CanMove(int x, int y)
    {
      Block b = core.level.GetBlock(x, y);
      if (b == null) return false;

      return !b.blocksMotion;
    }

    private void MovePlayer(Vector3 direction)
    {
      int x0 = GetX();
      int y0 = GetY();

      Vector3 newPosition = position + direction * lookAhead;

      int x1 = (int)core.level.WorldToLevel(newPosition).X;
      int y1 = (int)core.level.WorldToLevel(newPosition).Y;

      if (CanMove(x1, y1) && CanMove(x1, y0) && CanMove(x0, y1))
        position = position + direction * speed;
      else if (CanMove(x1, y0))
        position = position + new Vector3(direction.X, 0, 0) * speed;
      else if (CanMove(x0, y1))
        position = position + new Vector3(0, 0, direction.Z) * speed;
    }

    private void HandleInput(GameTime gameTime)
    {
      KeyboardState keyboardState = Keyboard.GetState();
      bool isMoving = false;

      if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
      {
        MovePlayer(rotation.Forward);
        isMoving = true;
      }

      if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
      {
        MovePlayer(rotation.Backward);
        isMoving = true;
      }

      if (keyboardState.IsKeyDown(Keys.A))
      {
        MovePlayer(rotation.Left);
        isMoving = true;
      }

      if (keyboardState.IsKeyDown(Keys.Q))
      {
        MovePlayer(rotation.Up);
      }

      if (keyboardState.IsKeyDown(Keys.D))
      {
        MovePlayer(rotation.Right);
        isMoving = true;
      }

      if (isMoving)
        oscillation = (float)Math.Sin(gameTime.TotalGameTime.Milliseconds / (100.0f * MathHelper.Pi)) * .2f;

      if (keyboardState.IsKeyDown(Keys.Left))
        roll += 0.05f;

      if (keyboardState.IsKeyDown(Keys.Right))
        roll -= 0.05f;
    }

    public void Update(GameTime gameTime)
    {
      HandleInput(gameTime);

      rotation *= Matrix.CreateFromAxisAngle(rotation.Up, roll);
      roll = 0;
      target = position + rotation.Forward;

      position.Y += oscillation;
      target.Y += oscillation;
      viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
      //viewFrustum = new BoundingFrustum(Matrix.CreateLookAt(position + rotation.Backward * 2f, target, Vector3.Up) * projectionMatrix);
      position.Y -= oscillation;
      target.Y -= oscillation;
    }
  }
}
