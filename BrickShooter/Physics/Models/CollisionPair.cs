using BrickShooter.Physics.Interfaces;

namespace BrickShooter.Physics.Models
{
    public class CollisionPair
    {
        public IColliderPolygon CollisionSubject { get; set; }
        public IColliderPolygon CollisionObject { get; set; }

        public CollisionPair(IColliderPolygon collisionSubject, IColliderPolygon collisionObject)
        {
            CollisionSubject = collisionSubject;
            CollisionObject = collisionObject;
        }
    }
}
