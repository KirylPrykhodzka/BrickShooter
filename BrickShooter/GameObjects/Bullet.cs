﻿using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Framework;
using BrickShooter.GameObjects.Enemies;
using BrickShooter.Physics.Models;
using BrickShooter.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace BrickShooter.GameObjects
{
    public class Bullet : MaterialObject, IDrawableObject, IResetable
    {
        private static readonly Texture2D sprite = GlobalObjects.Content.Load<Texture2D>("Bullets/Bullet");

        public OnPlayerHit OnPlayerHit;
        public OnBulletDestroy OnBulletDestroy;

        public Bullet()
        {
            Colliders.Add(new ColliderPolygon(this, nameof(Bullet), new Vector2[]
            {
                new(-BulletConstants.WIDTH / 2, -BulletConstants.HEIGHT /2),
                new(BulletConstants.WIDTH / 2, -BulletConstants.HEIGHT /2),
                new(BulletConstants.WIDTH / 2, BulletConstants.HEIGHT /2),
                new(-BulletConstants.WIDTH / 2, BulletConstants.HEIGHT /2),
            }));
            Bounciness = BulletConstants.BOUNCINESS;
        }

        public override void OnMovementCollision(MovementCollisionInfo collisionInfo)
        {
            switch (collisionInfo.CollisionObject.CollisionLayer)
            {
                case nameof(Player):
                    {
                        OnPlayerHit?.Invoke(this);
                        break;
                    }
                case nameof(Wall):
                    {
                        Rotation = Vector2.Reflect(Velocity, collisionInfo.Normal).ToAngle() + MathF.PI / 2;
                        break;
                    }
                case nameof(RedBrick):
                    {
                        OnBulletDestroy?.Invoke(this);
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
            Position = from;
            Velocity = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation)) * BulletConstants.VELOCITY;
            Rotation = rotation;
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                sprite,
                Position,
                null,
                Color.White,
                Rotation,
                new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.BULLETS);
        }

        public void Reset()
        {
            Position = Vector2.Zero;
            Rotation = 0f;
            Velocity = Vector2.Zero;
        }
    }
}
