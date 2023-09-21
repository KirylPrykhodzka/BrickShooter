using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.GameObjects.Bullets
{
    public class Bullet : MobileMaterialObject, IDrawableObject
    {
        public override float Bounciness => 1f;
        private static readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>("Bullets/Bullet");

        private bool isActive = false;

        public Bullet()
        {
            //4 points describing the sprite rectangle
            localColliderBounds = new Point[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };
        }

        public override void OnCollision(IMaterialObject otherCollider)
        {
            switch(otherCollider.GetType().Name)
            {
                case "Wall":
                    {
                        break;
                    }
                default:
                    {
                        Deactivate();
                        break;
                    }
            }
        }

        /// <summary>
        /// places bullet at a specified point in space and moves it in direction determined by rotation
        /// </summary>
        /// <param name="from"></param>
        /// <param name="rotation"></param>
        public void Move(Point from, float rotation)
        {
            Activate();
            Position = from;
            Velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * BulletConstants.VELOCITY;
            this.rotation = rotation;
        }

        public void Draw()
        {
            if(!isActive)
            {
                return;
            }
            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Vector2(Position.X, Position.Y),
                null,
                Color.White,
                rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.BULLETS);
        }

        private void Activate()
        {
            isActive = true;
            PhysicsSystem.RegisterMobileObject(this);
            DrawingSystem.Register(this);
        }

        private void Deactivate()
        {
            isActive = false;
            PhysicsSystem.RemoveMobileObject(this);
            DrawingSystem.Unregister(this);
            BulletFactory.Return(this);
        }
    }
}
