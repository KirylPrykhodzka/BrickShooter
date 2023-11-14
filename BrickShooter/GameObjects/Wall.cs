using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BrickShooter.GameObjects
{
    public class Wall : MaterialObject, IDrawableObject
    {
        private readonly Texture2D texture;
        private readonly Rectangle visibleBounds;

        public Wall(Texture2D texture, Point position, int width, int height, float rotation) : base()
        {
            this.texture = texture;
            this.visibleBounds = new Rectangle(position.X, position.Y, width, height);
            Position = new Vector2(position.X + width / 2, position.Y + height / 2);
            Rotation = rotation;
            var colliderPoints = new List<Vector2>
            {
                new(-(width / 2), -(height / 2)),
                new((width / 2), -(height / 2)),
                new((width / 2), (height / 2)),
                new(-(width / 2), (height / 2)),
            };
            Colliders.Add(new ColliderPolygon(this, nameof(Wall), colliderPoints));
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                texture,
                visibleBounds.Center.ToVector2(),
                visibleBounds,
                Color.White,
                Rotation,
                new Vector2(visibleBounds.Width / 2, visibleBounds.Height / 2),
                1f,
                SpriteEffects.None,
                Layers.WALLS);
        }
    }
}
