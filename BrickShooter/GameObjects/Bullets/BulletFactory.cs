using System.Collections.Generic;

namespace BrickShooter.GameObjects.Bullets
{
    public static class BulletFactory
    {
        private static Queue<Bullet> bullets = new();

        static BulletFactory()
        {
            for(int i = 0; i < 10; i++)
            {
                Bullet bullet = new Bullet();
                bullets.Enqueue(bullet);
            }
        }

        public static Bullet GetBullet()
        {
            if(bullets.TryDequeue(out Bullet bullet))
            {
                return bullet;
            }
            return new Bullet();
        }

        public static void Return(Bullet bullet)
        {
            bullets.Enqueue(bullet);
        }
    }
}
