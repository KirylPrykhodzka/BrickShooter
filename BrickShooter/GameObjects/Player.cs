using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Extensions;
using BrickShooter.GameObjects.Bullets;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Player : MaterialObject, IDrawableObject
    {
        private readonly Texture2D sprite;
        //for the cooldown calculation purposes
        private long lastShot = 0;

        //stuff to store in json
        private static readonly string spriteName = "player";
        //localColliderBounds
        private static readonly Vector2 barrelTipOffset = new(35, 12);

        public Player(Vector2 initialPosition)
        {
            sprite = GlobalObjects.Content.Load<Texture2D>($"Player/{spriteName}");
            Position = initialPosition;
            //4 points describing the sprite rectangle
            localColliderBounds = new Vector2[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(0, -sprite.Height /2),
                new(0, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };

            physicsSystem.RegisterMobileObject(this);
            DrawingSystem.Register(this);
        }

        public void Update()
        {
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            var mouseState = Mouse.GetState();
            HandleMovementInput(pressedKeys);
            HandleRotationInput(mouseState);
            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                var now = DateTime.Now.Ticks / 10000;
                if (now - lastShot > PlayerConstants.SHOOTING_COOLDOWN_MS)
                {
                    Shoot();
                    lastShot = now;
                }
            }
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
            if((Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > PlayerConstants.MAX_VELOCITY * Math.Sqrt(2))
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
                if (Math.Abs(Velocity.X) <= PlayerConstants.MIN_VELOCITY)
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
                if (Math.Abs(Velocity.Y) <= PlayerConstants.MIN_VELOCITY)
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

        private void Shoot()
        {
            var bullet = BulletFactory.GetBullet();
            var initialPosition = (Position + barrelTipOffset).Rotate(Position, Rotation);
            bullet.Move(initialPosition, Rotation);
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
