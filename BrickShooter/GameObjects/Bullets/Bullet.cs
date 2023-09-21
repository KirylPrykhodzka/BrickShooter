using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace BrickShooter.GameObjects.Bullets
{
    public class Bullet : MobileMaterialObject
    {
        private static readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>("Bullets/Bullet");

        public Bullet()
        {
            //4 points describing the sprite rectangle
            localColliderBounds = new Point[]
            {
                new(-BulletConstants.WIDTH / 2, -BulletConstants.HEIGHT /2),
                new(0, -BulletConstants.HEIGHT /2),
                new(0, BulletConstants.HEIGHT /2),
                new(-BulletConstants.WIDTH / 2, BulletConstants.HEIGHT /2),
            };
        }

        public override void OnCollision(IMaterialObject otherCollider)
        {
            PhysicsSystem.RemoveMobileObject(this);
            BulletFactory.Return(this);
            base.OnCollision(otherCollider);
        }

        public void Shoot(Point from, float rotation)
        {
            PhysicsSystem.AddMobileObject(this);
            Position = from;
            Velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * BulletConstants.VELOCITY;
            this.rotation = rotation + (float)(Math.PI / 2);
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Rectangle(Position.X, Position.Y, BulletConstants.WIDTH, BulletConstants.HEIGHT),
                null,
                Color.White,
                rotation,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.BULLETS);
        }
    }
}
