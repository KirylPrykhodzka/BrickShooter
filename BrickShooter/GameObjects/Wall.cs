using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Extensions;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.GameObjects
{
    public class Wall : MaterialObject, IDrawableObject
    {
        private readonly Texture2D texture;
        private readonly Rectangle visibleBounds;

        public Wall(Texture2D texture, Rectangle visibleBounds, float rotation) : base()
        {
            this.texture = texture;
            this.visibleBounds = visibleBounds;
            Rotation = rotation;
            var colliderPoints = new List<Vector2>
            {
                new(visibleBounds.X, visibleBounds.Y),
                new(visibleBounds.X + visibleBounds.Width, visibleBounds.Y),
                new(visibleBounds.X + visibleBounds.Width, visibleBounds.Y + visibleBounds.Height),
                new(visibleBounds.X, visibleBounds.Y + visibleBounds.Height),
            };
            Colliders.Add(new ColliderPolygon(this, nameof(Wall), colliderPoints));
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                texture,
                visibleBounds,
                visibleBounds,
                Color.White,
                Rotation,
                Position,
                SpriteEffects.None,
                Layers.WALLS);
        }
    }
}
