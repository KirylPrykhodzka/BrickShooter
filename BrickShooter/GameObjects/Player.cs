using BrickShooter.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Player
    {
        public Texture2D playerSprite;

        Vector2 currentPosition;
        Vector2 velocity;
        float rotation;

        public Player()
        {
            playerSprite = GlobalObjects.Content.Load<Texture2D>("Player/player");
            currentPosition = new Vector2(GlobalObjects.Graphics.GraphicsDevice.Viewport.Width / 2, GlobalObjects.Graphics.GraphicsDevice.Viewport.Height / 2);
        }

        public void Update()
        {
            UpdateVelocityAndPosition();
            UpdateRotation();
        }

        private void UpdateVelocityAndPosition()
        {
            velocity = Vector2.Zero;
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            if (pressedKeys.Contains(Keys.W))
            {
                velocity.Y -= 1;
            }
            if (pressedKeys.Contains(Keys.S))
            {
                velocity.Y += 1;
            }
            if (pressedKeys.Contains(Keys.A))
            {
                velocity.X -= 1;
            }
            if (pressedKeys.Contains(Keys.D))
            {
                velocity.X += 1;
            }
            if(velocity.X != 0 && velocity.Y != 0)
            {
                velocity *= (float)Math.Sqrt(2) / 2;
            }
            velocity *= PlayerConstants.MAX_VELOCITY;
            currentPosition += velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
        }

        private void UpdateRotation()
        {
            var mouseState = Mouse.GetState();
            var diffX = mouseState.X - currentPosition.X;
            var diffY = mouseState.Y - currentPosition.Y;
            rotation = (float)Math.Atan2(diffY, diffX);
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                playerSprite,
                new Vector2(currentPosition.X, currentPosition.Y),
                null,
                Color.White,
                rotation,
                new Vector2(playerSprite.Width / 2f, playerSprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.GAME_OBJECTS);
        }
    }
}
