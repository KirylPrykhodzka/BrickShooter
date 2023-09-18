using BrickShooter.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Wall 
    {
        public Texture2D Texture { get; set; }
        public Rectangle RectBounds { get; set; }

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
