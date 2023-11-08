using System.Collections.Generic;

namespace BrickShooter.Resources
{
    public interface IPool<T> where T : IResetable, new()
    {
        T GetItem();
        void Return(T item);
        void Return(IEnumerable<T> items);
    }
}
