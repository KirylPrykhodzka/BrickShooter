using BrickShooter.Resources;

namespace BrickShooter.Tests.Resources
{
    public class ResetableObject : IResetable
    {
        public int Value { get; set; }

        public void Reset()
        {
            Value = 0;
        }
    }
}
