using System.Collections.Generic;

namespace BrickShooter.Resources
{
    public class Factory<T> : IFactory<T> where T : IResetable, new()
    {
        private readonly Queue<T> items = new();

        public T GetItem()
        {
            if (items.Count > 0)
            {
                return items.Dequeue();
            }
            return new T();
        }

        public void Return(T item)
        {
            item.Reset();
            items.Enqueue(item);
        }
    }
}
