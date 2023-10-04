using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObjectMover
    {
        void Move(MaterialObject materialObject, Vector2 positionDiff);
        void MoveWithoutObstruction(MaterialObject materialObject);
    }
}
