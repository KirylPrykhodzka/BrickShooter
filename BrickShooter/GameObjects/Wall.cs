using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Wall : IMaterialObject, IDrawableObject
    {
        public Texture2D Texture { get; set; }
        public Rectangle RectBounds { get; set; }
        public ColliderPolygon ColliderBounds { get; }
        public float Bounciness => 0f;

        public Wall(Texture2D texture, Rectangle rectBounds)
        {
            Texture = texture;
            RectBounds = rectBounds;
            ColliderBounds = new ColliderPolygon();
            ColliderBounds.Points.Add(new Vector2(RectBounds.X, RectBounds.Y));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X + RectBounds.Width, RectBounds.Y));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X + RectBounds.Width, RectBounds.Y + RectBounds.Height));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X, RectBounds.Y + RectBounds.Height));
            ColliderBounds.BuildEdges();

            PhysicsSystem.RegisterImmobileObject(this);
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
