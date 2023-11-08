using System.Collections.Generic;

namespace BrickShooter.Resources
{
    public class Pool<T> : IPool<T> where T : IResetable, new()
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

        public void Return(IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                Return(item);
            }
        }
    }
}
