using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Framework;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System;

namespace BrickShooter.GameObjects.Enemies
{
    public abstract class Enemy : MaterialObject, IDrawableObject
    {
        private readonly Player player;

        public OnPlayerHit OnPlayerHit;
        public OnDeath OnDeath;

        public Enemy(Player player)
        {
            this.player = player;
        }

        public virtual void Update()
        {
            var playerPosition = player.Position;
            var diffX = playerPosition.X - Position.X;
            var diffY = playerPosition.Y - Position.Y;
            var newRotation = (float)Math.Atan2(diffY, diffX);
            DidRotate = newRotation != Rotation;
            Rotation = newRotation;
            Velocity = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * EnemiesConstants.VELOCITY;
        }

        public abstract void Draw();
    }
}
