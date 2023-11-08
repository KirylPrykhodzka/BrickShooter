using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;

namespace BrickShooter.Physics.Models
{
    public class CollisionPair : IResetable
    {
        public IColliderPolygon CollisionSubject { get; set; }
        public IColliderPolygon CollisionObject { get; set; }

        //for unit tests
        public CollisionPair() { }

        public CollisionPair(IColliderPolygon collisionSubject, IColliderPolygon collisionObject)
        {
            CollisionSubject = collisionSubject;
            CollisionObject = collisionObject;
        }

        public void Reset()
        {
            CollisionSubject = null;
            CollisionObject = null;
        }
    }
}
