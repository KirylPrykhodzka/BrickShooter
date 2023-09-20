using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Extensions;
using BrickShooter.Helpers;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Player : IMobileMaterialObject
    {
        public Vector2 Velocity { get; private set; }
        public Point Position { get; set; }
        public ColliderPolygon ColliderBounds => GetGlobalColliderBounds();
        private Point[] localColliderBounds;
        private float rotation;

        private readonly Texture2D sprite;


        public Player(Point initialPosition)
        {
            sprite = GlobalObjects.Content.Load<Texture2D>("Player/player");
            Position = initialPosition;
            //4 points describing the sprite rectangle
            localColliderBounds = new Point[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(0, -sprite.Height /2),
                new(0, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };

            PhysicsSystem.AddMobileObject(this);
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
            var center = Position;
            result.Points.AddRange(localColliderBounds.Select(x => GetGlobalPosition(x)));
            result.BuildEdges();
            return result;

            Vector2 GetGlobalPosition(Point localPoint)
            {
                //get global position
                Point globalPosition = Position + localPoint;
                //rotate collider
                return globalPosition.Rotate(Position, rotation).ToVector2();
            }
        }

        public void Update()
        {
            HandleMovementInput();
            HandleRotationInput();
        }

        private void HandleMovementInput()
        {
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
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
                    Velocity = new Vector2(0, Velocity.Y);
                }
                else
                {
                    Velocity /= new Vector2(1, 1 + PlayerConstants.DECELERATION_FACTOR);
                }
            }
        }

        private void HandleRotationInput()
        {
            var mouseState = Mouse.GetState();
            var diffX = mouseState.X - Position.X;
            var diffY = mouseState.Y - Position.Y;
            rotation = (float)Math.Atan2(diffY, diffX);
        }

        public void Draw()
        {
#if DEBUG
            VisualizationHelper.VisualizeCollider(GetGlobalColliderBounds().Points);
#endif

            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Vector2(Position.X, Position.Y),
                null,
                Color.White,
                rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.PLAYER);
        }

        public void OnCollision(IMaterialObject collisionActor) { }
    }
}
