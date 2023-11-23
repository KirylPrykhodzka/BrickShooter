using BrickShooter.Constants;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace BrickShooter.GameObjects.Enemies
{
    public class RedBrick : Enemy
    {
        private readonly Texture2D texture;
        private readonly Rectangle visibleBounds;

        public RedBrick(Player player) : base(player)
        {
            texture = GlobalObjects.Content.Load<Texture2D>("Enemies/red");
            Colliders.Add(new ColliderPolygon(this, nameof(RedBrick), EnemiesConstants.INITIAL_COLLIDER_POINTS));
        }

        public override void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                texture,
                Position,
                null,
                Color.White,
                Rotation,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                1f,
                SpriteEffects.None,
                Layers.ENEMIES);
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
                case nameof(Bullet):
                    {
                        OnDeath?.Invoke(this);
                        break;
                    }
            }
        }
    }
}
