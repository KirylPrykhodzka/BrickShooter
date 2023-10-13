using BrickShooter.Resources;

namespace BrickShooter.Tests.Mocks
{
    public class ResetableObjectMock : IResetable
    {
        public int Value { get; set; }

        public void Reset()
        {
            Value = 0;
        }
    }
}
