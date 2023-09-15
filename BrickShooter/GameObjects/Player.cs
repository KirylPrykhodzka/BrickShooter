using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.GameObjects
{
    public class Player
    {
        public Texture2D playerSprite;

        public Player()
        {
            playerSprite = GlobalObjects.Content.Load<Texture2D>("Player/player");
        }

        public void Draw()
        {
            GlobalObjects.SpriteBatch.Draw(
                playerSprite,
                new Rectangle(0, 0, playerSprite.Width, playerSprite.Height),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.GAME_OBJECTS);
        }
    }
}
