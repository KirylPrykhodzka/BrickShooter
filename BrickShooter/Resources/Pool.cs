using System.Collections.Generic;
using System.Diagnostics;

namespace BrickShooter.Resources
{
    public class Pool<T> : IPool<T> where T : IResetable, new()
    {
        private readonly Queue<T> items = new();

        public T GetItem()
        {
            if (items.Count > 0)
            {
                Debug.WriteLine($"returning {typeof(T)} from pool");
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
