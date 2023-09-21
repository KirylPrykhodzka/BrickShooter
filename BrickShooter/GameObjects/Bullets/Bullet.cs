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
                new(-sprite.Width / 2, -sprite.Height /2),
                new(0, -sprite.Height /2),
                new(0, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };
        }

        public override void OnCollision(IMaterialObject otherCollider)
        {
            PhysicsSystem.RemoveMobileObject(this);
            BulletFactory.Return(this);
            base.OnCollision(otherCollider);
        }

        public void Shoot(Point from, Vector2 to)
        {
            Debug.WriteLine("Bam!");
            PhysicsSystem.AddMobileObject(this);
            Position = from;
            Velocity = new Vector2(Math.Clamp(to.X, -1f, 1f), Math.Clamp(to.Y, -1, 1)) * BulletConstants.VELOCITY;
            rotation = (float)Math.Atan2(to.Y, to.X);
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                sprite,
                new Rectangle(Position.X, Position.Y, BulletConstants.WIDTH, BulletConstants.HEIGHT),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.BULLETS);
        }
    }
}
