using System.Collections.Generic;
using System.Linq;

namespace BrickShooter.Collision
{
    /// <summary>
    /// finds all collisions between game objects.
    /// Should be reset upon reloading the level
    /// </summary>
    public static class CollisionSystem
    {
        private readonly static List<ICollisionSubject> collisionSubjects = new();
        private readonly static List<ICollisionActor> collisionObjects = new();

        public static void AddSubject(ICollisionSubject subject)
        {
            collisionSubjects.Add(subject);
        }

        public static void AddObject(ICollisionActor collisionObject)
        {
            collisionObjects.Add(collisionObject);
        }

        /// <summary>
        /// on each update, checks all collisionSubjects for collision with each other or collisionObjects
        /// if there is a collision, OnCollision is called on the subject
        /// </summary>
        public static void Run()
        {
            for (int i = 0; i < collisionSubjects.Count; i++)
            {
                var currentElement = collisionSubjects[i];

                //check collision with other subjects
                for (int j = i + 1; j < collisionSubjects.Count; j++)
                {
                    if (Collides(currentElement, collisionSubjects[j]))
                    {
                        //in this game, only subjects are bullets and player, so no physics calculation is needed upon collision
                        currentElement.OnCollision(collisionSubjects[j]);
                        collisionSubjects[j].OnCollision(currentElement);
                    }
                }

                //check collision with objects
                for (int j = 0; j < collisionObjects.Count; j++)
                {
                    if (Collides(currentElement, collisionObjects[j]))
                    {
                        //translate velocity to avoid further collision
                        currentElement.OnCollision(collisionObjects[j]);
                    }
                }
            }
        }

        private static bool Collides(ICollisionActor first, ICollisionActor second)
        {
            return false;
        }

        public static void Reset()
        {
            collisionSubjects.Clear();
            collisionObjects.Clear();
        }
    }
}
