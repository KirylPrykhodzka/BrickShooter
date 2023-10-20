using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Physics;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Wall : MaterialObject, IDrawableObject
    {
        public Texture2D Texture { get; set; }
        public Rectangle RectBounds { get; set; }

        public Wall(Texture2D texture, Rectangle rectBounds) : base()
        {
            Texture = texture;
            RectBounds = rectBounds;
            //4 points describing the sprite rectangle
            initialColliderPoints = new Vector2[]
            {
                new(RectBounds.X, RectBounds.Y),
                new(RectBounds.X + RectBounds.Width, RectBounds.Y),
                new(RectBounds.X + RectBounds.Width, RectBounds.Y + RectBounds.Height),
                new(RectBounds.X, RectBounds.Y + RectBounds.Height),
            };
            CollisionLayer = nameof(Wall);
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                Texture,
                RectBounds,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.WALLS);
        }
    }
}
