using BrickShooter.Physics.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        private readonly List<(IMaterialObject materialObject, Vector2 movement)> scheduledMovements = new();

        public void ScheduleMovement(IMaterialObject materialObject, Vector2 movement)
        {
            scheduledMovements.Add((materialObject, movement));
        }

        public void ApplyScheduledMovements()
        {
            foreach (var (materialObject, movement) in scheduledMovements)
            {
                MoveObject(materialObject, movement);
            }
            scheduledMovements.Clear();
        }

        public void MoveObject(IMaterialObject materialObject, Vector2 movement)
        {
            materialObject.Position += movement;
        }
    }
}
