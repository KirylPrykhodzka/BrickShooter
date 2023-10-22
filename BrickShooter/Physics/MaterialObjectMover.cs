﻿using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        public void MoveWithoutObstruction(MaterialObject materialObject)
        {
            materialObject.Position += materialObject.Velocity * GlobalObjects.DeltaTime;
        }
    }
}
