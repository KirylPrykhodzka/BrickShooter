using BrickShooter.Constants;
using BrickShooter.Drawing;
using BrickShooter.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

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
            localColliderBounds = new Vector2[]
            {
                new(RectBounds.X, RectBounds.Y),
                new(RectBounds.X + RectBounds.Width, RectBounds.Y),
                new(RectBounds.X + RectBounds.Width, RectBounds.Y + RectBounds.Height),
                new(RectBounds.X, RectBounds.Y + RectBounds.Height),
            };

            physicsSystem.RegisterImmobileObject(this);
            DrawingSystem.Register(this);
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
