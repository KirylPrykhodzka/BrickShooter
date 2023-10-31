using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        private readonly List<(MaterialObject materialObject, Vector2 movement)> scheduledMovements = new();

        public void ScheduleMovement(MaterialObject materialObject, Vector2 movement)
        {
            scheduledMovements.Add((materialObject, movement));
        }

        public void Move()
        {
            foreach (var scheduledMovement in scheduledMovements)
            {
                scheduledMovement.materialObject.Position += scheduledMovement.movement;
            }
            scheduledMovements.Clear();
        }
    }
}
