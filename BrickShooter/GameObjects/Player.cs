using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Extensions;
using BrickShooter.Framework;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Player : MaterialObject, IDrawableObject
    {
        private static readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>($"Player/player");

        public Player(Vector2 initialPosition)
        {
            Position = initialPosition;
            Colliders.Add(new ColliderPolygon(this, nameof(Player), PlayerConstants.INITIAL_COLLIDER_POINTS));
            Colliders.Add(new ColliderPolygon(this, "PlayerGun", PlayerConstants.INITIAL_GUN_COLLIDER_POINTS));
        }

        public Vector2 BulletSpawnPoint => PlayerConstants.PLAYER_GUN_POSITION.Rotate(Vector2.Zero, Rotation) + Position;

        public void Update()
        {
            var pressedKeys = GlobalObjects.KeyboardState.PressedKeys;
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
            //if both up and down keys are pressed, player should not move
            if (Velocity.Y != 0 && ((pressedKeys.Contains(Keys.W) && pressedKeys.Contains(Keys.S)) || !(pressedKeys.Contains(Keys.W) || pressedKeys.Contains(Keys.S))))
            {
                Decelerate('y');
            }
            //if player is trying to move in direction opposite to current movement direction, stop immediately
            else if ((Velocity.Y > 0 && pressedKeys.Contains(Keys.W)) || (Velocity.Y < 0 && pressedKeys.Contains(Keys.S)))
            {
                Velocity = new(Velocity.X, 0);
            }
            else
            {
                HandleDirectionInput(pressedKeys, Keys.W, 'y', -1);
                HandleDirectionInput(pressedKeys, Keys.S, 'y', 1);
            }

            if (Velocity.X != 0 && ((pressedKeys.Contains(Keys.A) && pressedKeys.Contains(Keys.D)) || !(pressedKeys.Contains(Keys.A) || pressedKeys.Contains(Keys.D))))
            {
                Decelerate('x');
            }
            else if((Velocity.X > 0 && pressedKeys.Contains(Keys.A)) || (Velocity.X < 0 && pressedKeys.Contains(Keys.D)))
            {
                Velocity = new(0, Velocity.Y);
            }
            else
            {
                HandleDirectionInput(pressedKeys, Keys.A, 'x', -1);
                HandleDirectionInput(pressedKeys, Keys.D, 'x', 1);
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
            float deceleration = currentVelocity * PlayerConstants.DECELERATION_FACTOR;

            if (Math.Abs(currentVelocity - deceleration) > PhysicsConstants.MIN_VELOCITY)
            {

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

        private void HandleRotationInput(IMouseState mouseState)
        {
            var diffX = mouseState.X - Position.X;
            var diffY = mouseState.Y - Position.Y;
            var newRotation = (float)Math.Atan2(diffY, diffX);
            DidRotate = newRotation != Rotation;
            Rotation = newRotation;
        }
    }
}
