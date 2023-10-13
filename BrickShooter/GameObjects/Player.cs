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
        private readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>($"Player/player");

        public Player(Vector2 initialPosition)
        {
            Position = initialPosition;
            initialColliderPoints = PlayerConstants.INITIAL_COLLIDER_POINTS;
        }

        public Vector2 InitialBulletPosition => (Position + PlayerConstants.BARREL_TIP_OFFSET).Rotate(Position, Rotation);

        public void Update()
        {
            var pressedKeys = GlobalObjects.KeyboardState.GetPressedKeys();
            var mouseState = GlobalObjects.MouseState;
            HandleMovementInput(pressedKeys);
            HandleRotationInput(mouseState);
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

        private void HandleMovementInput(Keys[] pressedKeys)
        {
            HandleDirectionInput(pressedKeys, Keys.W, 'y', -1);
            HandleDirectionInput(pressedKeys, Keys.S, 'y', 1);
            HandleDirectionInput(pressedKeys, Keys.A, 'x', -1);
            HandleDirectionInput(pressedKeys, Keys.D, 'x', 1);

            if (!pressedKeys.Contains(Keys.W) && !pressedKeys.Contains(Keys.S) && Velocity.Y != 0)
            {
                Decelerate('y');
            }
            if (!pressedKeys.Contains(Keys.A) && !pressedKeys.Contains(Keys.D) && Velocity.X != 0)
            {
                Decelerate('x');
            }

            // Normalize diagonal movement
            if (Math.Abs(Velocity.X) == Math.Abs(Velocity.Y) && Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) > PlayerConstants.MAX_VELOCITY * Math.Sqrt(2))
            {
                Velocity *= PlayerConstants.MAX_VELOCITY * (float)Math.Sqrt(2) / (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y));
            }
        }

        private void HandleDirectionInput(Keys[] pressedKeys, Keys key, char axis, int direction)
        {
            if (pressedKeys.Contains(key))
            {
                Accelerate(axis, direction);
            }
        }

        private void Accelerate(char axis, int direction)
        {
            float currentVelocity = axis == 'x' ? Math.Abs(Velocity.X) : Math.Abs(Velocity.Y);
            float acceleration = MathHelper.Clamp(PlayerConstants.MAX_VELOCITY * PlayerConstants.ACCELERATION_FACTOR, 0, PlayerConstants.MAX_VELOCITY - currentVelocity);

            if (axis == 'x')
            {
                Velocity += new Vector2(acceleration * direction, 0);
            }
            else
            {
                Velocity += new Vector2(0, acceleration * direction);
            }
        }

        private void Decelerate(char axis)
        {
            float currentVelocity = axis == 'x' ? Velocity.X : Velocity.Y;

            if (Math.Abs(currentVelocity) > PhysicsConstants.MIN_VELOCITY)
            {
                float deceleration = currentVelocity * PlayerConstants.DECELERATION_FACTOR;

                if (axis == 'x')
                {
                    Velocity -= new Vector2(deceleration, 0);
                }
                else
                {
                    Velocity -= new Vector2(0, deceleration);
                }
            }
            else
            {
                Velocity = axis == 'x' ? new Vector2(0, Velocity.Y) : new Vector2(Velocity.X, 0);
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
    }
}
