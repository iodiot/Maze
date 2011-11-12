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

#endregion

namespace Maze.Engine
{
  public class Core
  {
    #region Fields

    public Art art;
    public Level level;
    public Player player;

    public FreeCamera freeCamera;

    public GraphicsDevice device;
    public Effect customEffect;
    public SpriteBatch spriteBatch;
    public SpriteFont customFont;

    public Random random = new Random(Guid.NewGuid().GetHashCode());

    #endregion

    #region Initialization

    public Core(GraphicsDevice device, SpriteBatch spriteBatch)
    {
      this.device = device;
      this.spriteBatch = spriteBatch;
    }

    public void LoadContent(ContentManager content)
    {
      customEffect = content.Load<Effect>("CustomEffect");
      customFont = content.Load<SpriteFont>("CustomFont");

      art = new Art(this);
      art.LoadContent(content);

      level = new Level(this);
      level.LoadContent(content, "Cave");

      player = new Player(this);

      freeCamera = new FreeCamera(device);
    }

    #endregion

    #region Update and draw

    public void Update(GameTime gameTime)
    {
      player.Update(gameTime);
      level.Update(gameTime);
     // freeCamera.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
      customEffect.Parameters["xWorld"].SetValue(level.worldMatrix);
      customEffect.Parameters["xView"].SetValue(player.viewMatrix);
      customEffect.Parameters["xProjection"].SetValue(freeCamera.projectionMatrix);

      customEffect.Parameters["xCameraPosition"].SetValue(player.position);
      customEffect.Parameters["xAmbientLightColor"].SetValue(new Vector3(1f, 1f, 1f));

      customEffect.Parameters["xFogEnabled"].SetValue(false);
      customEffect.Parameters["xFogStart"].SetValue(2f);
      customEffect.Parameters["xFogEnd"].SetValue(5f);
      customEffect.Parameters["xFogColor"].SetValue(Color.Black.ToVector3());

      level.Draw(gameTime);

      /*spriteBatch.Begin();
      spriteBatch.DrawString(customFont, "Vertex count: " + level.GetVertexCount().ToString(), new Vector2(10, 10), Color.White);
      spriteBatch.End();
     
      device.BlendState = BlendState.Opaque;
      device.DepthStencilState = DepthStencilState.Default;
      device.SamplerStates[0] = SamplerState.PointClamp;*/
    }

    #endregion
  }
}
