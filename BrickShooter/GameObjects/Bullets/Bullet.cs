using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
                new(sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
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
            this.rotation = rotation;
        }

        public void Draw()
        {
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
    }
}
