using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maze.Engine
{
  public class FreeCamera
  {
    public Vector3 position;
    private Vector3 target;
    private Vector3 offsetDistance;

    private float yaw, pitch, roll;
    private float speed;

    private MouseState originalMouseState;

    private Matrix cameraRotation;

    public Matrix viewMatrix, projectionMatrix;

    GraphicsDevice device;

    public FreeCamera(GraphicsDevice device)
    {
      this.device = device;

      ResetCamera();
    }

    public void ResetCamera()
    {
      position = new Vector3(3.0f, 3.0f, 3.0f);
      target = Vector3.Zero;

      offsetDistance = new Vector3(0, 0, 50);

      yaw = 0.0f;
      pitch = 0.0f;
      roll = 0.0f;

      speed = .1f;

      Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
      originalMouseState = Mouse.GetState();


      cameraRotation = Matrix.Identity;
      viewMatrix = Matrix.Identity;
      projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), device.Viewport.AspectRatio, .1f, 1000.0f);
    }

    public void Update(GameTime gameTine)
    {
      HandleInput();
      UpdateViewMatrix();
    }

    private void HandleInput()
    {
      KeyboardState keyboardState = Keyboard.GetState();
      MouseState currentMouseState = Mouse.GetState();

      // Rotates camera
      if (currentMouseState != originalMouseState)
      {
        float xDifference = currentMouseState.X - originalMouseState.X;
        float yDifference = currentMouseState.Y - originalMouseState.Y;
        yaw -= xDifference * .001f;
        pitch -= yDifference * .001f;
        Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
      }

      if (currentMouseState.LeftButton == ButtonState.Pressed)
      {
        roll -= .01f;
      }

      if (currentMouseState.RightButton == ButtonState.Pressed)
      {
        roll += .01f;
      }

      // Moves camera
      if (keyboardState.IsKeyDown(Keys.W))
      {
        MoveCamera(cameraRotation.Forward);
      }
      if (keyboardState.IsKeyDown(Keys.S))
      {
        MoveCamera(-cameraRotation.Forward);
      }
      if (keyboardState.IsKeyDown(Keys.A))
      {
        MoveCamera(-cameraRotation.Right);
      }
      if (keyboardState.IsKeyDown(Keys.D))
      {
        MoveCamera(cameraRotation.Right);
      }
      if (keyboardState.IsKeyDown(Keys.E))
      {
        MoveCamera(cameraRotation.Up);
      }
      if (keyboardState.IsKeyDown(Keys.Q))
      {
        MoveCamera(-cameraRotation.Up);
      }

    }

    private void MoveCamera(Vector3 addedVector)
    {
      position += speed * addedVector;
    }

    private void UpdateViewMatrix()
    {

      cameraRotation.Forward.Normalize();
      cameraRotation.Up.Normalize();
      cameraRotation.Right.Normalize();

      cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Right, pitch);
      cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Up, yaw);
      cameraRotation *= Matrix.CreateFromAxisAngle(cameraRotation.Forward, roll);

      yaw = 0.0f;
      pitch = 0.0f;
      roll = 0.0f;

      target = position + cameraRotation.Forward;

      //We'll always use this line of code to set up the View Matrix.
      viewMatrix = Matrix.CreateLookAt(position, target, cameraRotation.Up);
    }
  }
}
