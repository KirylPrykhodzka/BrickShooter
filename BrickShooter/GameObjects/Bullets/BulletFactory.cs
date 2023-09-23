using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.GameObjects.Bullets
{
    public static class BulletFactory
    {
        private static readonly LinkedList<Bullet> bullets = new();

        static BulletFactory()
        {
            for(int i = 0; i < 10; i++)
            {
                Bullet bullet = new Bullet();
                bullets.AddLast(bullet);
            }
        }

        public static Bullet GetBullet()
        {
            if(bullets.Count > 0)
            {
                var result = bullets.Last.Value;
                bullets.RemoveLast();
                return result;
            }
            return new Bullet();
        }

        public static void Return(Bullet bullet)
        {
            bullet.Position = Point.Zero;
            bullet.Velocity = Vector2.Zero;
            bullets.AddLast(bullet);
        }
    }
}
