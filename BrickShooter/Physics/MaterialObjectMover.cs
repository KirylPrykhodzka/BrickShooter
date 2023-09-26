namespace BrickShooter.Physics
{
    public class MaterialObjectMover : IMaterialObjectMover
    {
        public void MoveWithoutObstruction(MaterialObject materialObject)
        {
            materialObject.Position += materialObject.Velocity * (float)GlobalObjects.GameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
