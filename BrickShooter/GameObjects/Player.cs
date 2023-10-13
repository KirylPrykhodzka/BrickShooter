using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Extensions;
using BrickShooter.Physics;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    [CollisionLayer("Player")]
    public class Player : MaterialObject, IDrawableObject
    {
        //stuff to store in json
        private static readonly string spriteName = "player";
        private readonly Texture2D sprite;
        private static readonly Vector2 barrelTipOffset = new(35, 12);

        public Vector2 InitialBulletPosition => (Position + barrelTipOffset).Rotate(Position, Rotation);

        public Player(Vector2 initialPosition)
        {
            sprite = GlobalObjects.Content.Load<Texture2D>($"Player/{spriteName}");
            Position = initialPosition;
            initialColliderPoints = new Vector2[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(0, -sprite.Height /2),
                new(0, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };
        }

        public void Update()
        {
            var pressedKeys = GlobalObjects.KeyboardState.GetPressedKeys();
            var mouseState = GlobalObjects.MouseState;
            HandleMovementInput(pressedKeys);
            HandleRotationInput(mouseState);
        }

        private void HandleMovementInput(Keys[] pressedKeys)
        {
            if (pressedKeys.Contains(Keys.W))
            {
                if(Velocity.Y > 0)
                {
                    Velocity = new Vector2(Velocity.X, 0);
                }
                else
                {
                    Accelerate('y', -1);
                }
            }
            if (pressedKeys.Contains(Keys.S))
            {
                if (Velocity.Y < 0)
                {
                    Velocity = new Vector2(Velocity.X, 0);
                }
                else
                {
                    Accelerate('y', 1);
                }
            }
            if (pressedKeys.Contains(Keys.A))
            {
                if (Velocity.X > 0)
                {
                    Velocity = new Vector2(0, Velocity.Y);
                }
                else
                {
                    Accelerate('x', -1);
                }
            }
            if (pressedKeys.Contains(Keys.D))
            {
                if (Velocity.X < 0)
                {
                    Velocity = new Vector2(0, Velocity.Y);
                }
                else
                {
                    Accelerate('x', 1);
                }
            }
            if (!pressedKeys.Contains(Keys.W) && !pressedKeys.Contains(Keys.S) && Velocity.Y != 0)
            {
                Decelerate('y');
            }
            if (!pressedKeys.Contains(Keys.A) && !pressedKeys.Contains(Keys.D) && Velocity.X != 0)
            {
                Decelerate('x');
            }
            //normalize diagonal movement
            if(Math.Abs(Velocity.X) == Math.Abs(Velocity.Y) && Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) > PlayerConstants.MAX_VELOCITY * Math.Sqrt(2))
            {
                Velocity *= PlayerConstants.MAX_VELOCITY * (float)Math.Sqrt(2) / (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y));
            }
        }

        private void Accelerate(char axis, int direction)
        {
            if (axis == 'x')
            {
                var diff = MathHelper.Clamp(PlayerConstants.MAX_VELOCITY * PlayerConstants.ACCELERATION_FACTOR, 0, PlayerConstants.MAX_VELOCITY - Math.Abs(Velocity.X));
                Velocity += new Vector2(diff * direction, 0);
            }
            if (axis == 'y')
            {
                var diff = MathHelper.Clamp(PlayerConstants.MAX_VELOCITY * PlayerConstants.ACCELERATION_FACTOR, 0, PlayerConstants.MAX_VELOCITY - Math.Abs(Velocity.Y));
                Velocity += new Vector2(0, diff * direction);
            }
        }

        private void Decelerate(char axis)
        {
            if (axis == 'x')
            {
                if (Math.Abs(Velocity.X) <= PhysicsConstants.MIN_VELOCITY)
                {
                    Velocity = new Vector2(0, Velocity.Y);
                }
                else
                {
                    Velocity /= new Vector2(1 + PlayerConstants.DECELERATION_FACTOR, 1);
                }

            }
            if (axis == 'y')
            {
                if (Math.Abs(Velocity.Y) <= PhysicsConstants.MIN_VELOCITY)
                {
                    Velocity = new Vector2(Velocity.X, 0);
                }
                else
                {
                    Velocity /= new Vector2(1, 1 + PlayerConstants.DECELERATION_FACTOR);
                }
            }
        }

        private void HandleRotationInput(MouseState mouseState)
        {
            var diffX = mouseState.X - Position.X;
            var diffY = mouseState.Y - Position.Y;
            var newRotation = (float)Math.Atan2(diffY, diffX);
            if(newRotation != Rotation)
            {
                Rotation = newRotation;
                DidRotate = true;
            }
            else
            {
                DidRotate = false;
            }
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Vector2(Position.X, Position.Y),
                null,
                Color.White,
                Rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.PLAYER);
        }
    }
}
