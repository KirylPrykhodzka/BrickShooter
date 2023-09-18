using BrickShooter.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace BrickShooter.GameObjects
{
    public class Wall : ICollisionActor
    {
        public Texture2D Texture { get; set; }

        private Rectangle rectBounds;
        public Rectangle RectBounds
        {
            get
            {
                return rectBounds;
            }
            set
            {
                rectBounds = value;
                Bounds = new RectangleF(rectBounds.X, rectBounds.Y, rectBounds.Width, rectBounds.Height);
            }
        }

        public IShapeF Bounds { get; private set; }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            //no need to do anything here; if something collides with a wall, it should handle the collision itself
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
                Layers.GAME_OBJECTS);
        }
    }
}
