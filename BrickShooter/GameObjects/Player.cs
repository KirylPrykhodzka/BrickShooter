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
        float rotation;

        public Player()
        {
            playerSprite = GlobalObjects.Content.Load<Texture2D>("Player/player");
            currentPosition = new Vector2(GlobalObjects.Graphics.GraphicsDevice.Viewport.Width / 2, GlobalObjects.Graphics.GraphicsDevice.Viewport.Height / 2);
        }

        public void Update()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            Vector2 movement = Vector2.Zero;
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            if (pressedKeys.Contains(Keys.W))
            {
                movement.Y -= 1;
            }
            if (pressedKeys.Contains(Keys.S))
            {
                movement.Y += 1;
            }
            if (pressedKeys.Contains(Keys.A))
            {
                movement.X -= 1;
            }
            if (pressedKeys.Contains(Keys.D))
            {
                movement.X += 1;
            }
            if(movement.X != 0 && movement.Y != 0)
            {
                movement *= (float)Math.Sqrt(2) / 2;
            }
            currentPosition += new Vector2(movement.X * PlayerConstants.speed * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds, movement.Y * PlayerConstants.speed * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds);
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
