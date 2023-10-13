using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.Drawing
{
    public class Texture_2D : Texture2D, ITexture_2D
    {
        public Texture_2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height)
        {
        }
    }
}
