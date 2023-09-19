using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Player : ICollisionSubject
    {
        private Vector2 velocity;
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }
        public ColliderPolygon ColliderBounds => GetGlobalColliderBounds();
        private Point[] localColliderBounds;
        private Point currentPosition;
        private float rotation;

        private readonly Texture2D sprite;


        public Player(Point initialPosition)
        {
            sprite = GlobalObjects.Content.Load<Texture2D>("Player/player");
            currentPosition = initialPosition;
            //4 points describing the sprite rectangle
            localColliderBounds = new Point[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };
        }

        /// <summary>
        /// ColliderBounds depend on position, rotation and scale of the object
        /// To avoid re-calculating them upon each update of position / rotation / scale,
        /// calculation logic is placed here to be called when needed
        /// </summary>
        /// <returns></returns>
        private ColliderPolygon GetGlobalColliderBounds()
        {
            var result = new ColliderPolygon();
            var center = currentPosition;
            result.Points.AddRange(localColliderBounds.Select(x => GetGlobalPosition(x)));
            result.BuildEdges();
            return result;

            Vector2 GetGlobalPosition(Point localPoint)
            {
                //get global position
                Point globalPosition = currentPosition + localPoint;
                //rotate collider
                return globalPosition.Rotate(currentPosition, rotation).ToVector2();
            }
        }

        public void Update()
        {
            UpdateVelocityAndPosition();
            UpdateRotation();
        }

        private void UpdateVelocityAndPosition()
        {
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            if (pressedKeys.Contains(Keys.W))
            {
                if(velocity.Y > 0)
                {
                    velocity.Y = 0;
                }
                else
                {
                    Accelerate('y', -1);
                }
            }
            if (pressedKeys.Contains(Keys.S))
            {
                if (velocity.Y < 0)
                {
                    velocity.Y = 0;
                }
                else
                {
                    Accelerate('y', 1);
                }
            }
            if (pressedKeys.Contains(Keys.A))
            {
                if (velocity.X > 0)
                {
                    velocity.X = 0;
                }
                else
                {
                    Accelerate('x', -1);
                }
            }
            if (pressedKeys.Contains(Keys.D))
            {
                if (velocity.X < 0)
                {
                    velocity.X = 0;
                }
                else
                {
                    Accelerate('x', 1);
                }
            }
            if (!pressedKeys.Contains(Keys.W) && !pressedKeys.Contains(Keys.S) && velocity.Y != 0)
            {
                Decelerate('y');
            }
            if (!pressedKeys.Contains(Keys.A) && !pressedKeys.Contains(Keys.D) && velocity.X != 0)
            {
                Decelerate('x');
            }
            //normalize diagonal movement
            if((Math.Abs(velocity.X) + Math.Abs(velocity.Y)) > PlayerConstants.MAX_VELOCITY * Math.Sqrt(2))
            {
                velocity *= PlayerConstants.MAX_VELOCITY * (float)Math.Sqrt(2) / (Math.Abs(velocity.X) + Math.Abs(velocity.Y));
            }

            var fixedVelocity = velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
            var positionDiff = new Point((int)fixedVelocity.X, (int)fixedVelocity.Y);
            currentPosition += positionDiff;
        }

        private void Accelerate(char axis, int direction)
        {
            if (axis == 'x')
            {
                var diff = MathHelper.Clamp(PlayerConstants.MAX_VELOCITY * PlayerConstants.ACCELERATION_FACTOR, 0, PlayerConstants.MAX_VELOCITY - Math.Abs(velocity.X));
                velocity.X += diff * direction;
            }
            if (axis == 'y')
            {
                var diff = MathHelper.Clamp(PlayerConstants.MAX_VELOCITY * PlayerConstants.ACCELERATION_FACTOR, 0, PlayerConstants.MAX_VELOCITY - Math.Abs(velocity.Y));
                velocity.Y += diff * direction;
            }
        }

        private void Decelerate(char axis)
        {
            if (axis == 'x')
            {
                if (Math.Abs(velocity.X) <= PlayerConstants.MIN_VELOCITY)
                {
                    velocity.X = 0;
                }
                else
                {
                    velocity.X /= 1 + PlayerConstants.DECELERATION_FACTOR;
                }

            }
            if (axis == 'y')
            {
                if (Math.Abs(velocity.Y) <= PlayerConstants.MIN_VELOCITY)
                {
                    velocity.Y = 0;
                }
                else
                {
                    velocity.Y /= 1 + PlayerConstants.DECELERATION_FACTOR;
                }
            }
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
#if DEBUG
            VisualizationHelper.VisualizeCollider(GetGlobalColliderBounds().Points);
#endif

            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Vector2(currentPosition.X, currentPosition.Y),
                null,
                Color.White,
                rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.PLAYER);
        }

        public void OnCollision(ICollisionActor collisionActor) { }
    }
}
