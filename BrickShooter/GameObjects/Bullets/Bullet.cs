using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.GameObjects.Bullets
{
    public class Bullet : MaterialObject, IDrawableObject
    {
        private static readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>("Bullets/Bullet");

        private bool isActive = false;

        public Bullet()
        {
            //4 points describing the sprite rectangle
            initialLocalColliderPoints = new Vector2[]
            {
                new(-sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, -sprite.Height /2),
                new(sprite.Width / 2, sprite.Height /2),
                new(-sprite.Width / 2, sprite.Height /2),
            };
            Bounciness = 1f;
        }

        public override void OnCollision(MaterialObject otherCollider)
        {
            switch(otherCollider.GetType().Name)
            {
                case "Wall":
                    {
                        //rotate
                        break;
                    }
                case "Player":
                    {
                        Deactivate();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// places bullet at a specified point in space and moves it in direction determined by rotation
        /// </summary>
        /// <param name="from"></param>
        /// <param name="rotation"></param>
        public void Move(Vector2 from, float rotation)
        {
            Activate();
            Position = from;
            Velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * BulletConstants.VELOCITY;
            Rotation = rotation;
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
                Rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.BULLETS);
        }

        private void Activate()
        {
            isActive = true;
            physicsSystem.RegisterMobileObject(this);
            DrawingSystem.Register(this);
        }

        private void Deactivate()
        {
            isActive = false;
            physicsSystem.UnregisterMobileObject(this);
            DrawingSystem.Unregister(this);
            BulletFactory.Return(this);
        }
    }
}
