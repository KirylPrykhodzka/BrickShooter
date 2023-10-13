using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;

namespace BrickShooter.Tests.Physics
{
    public class TestMaterialObject : MaterialObject
    {

        public TestMaterialObject(Vector2[] initialColliderPoints)
        {
            this.initialColliderPoints = initialColliderPoints;
        }
    }
}
