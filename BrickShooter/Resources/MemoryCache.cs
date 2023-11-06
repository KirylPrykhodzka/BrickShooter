using System.Collections.Generic;

namespace BrickShooter.Resources
{
    public class MemoryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly Queue<TKey> keys;
        private readonly int capacity;

        public MemoryCache(int capacity)
        {
            keys = new Queue<TKey>(capacity);
            this.capacity = capacity;
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.Count == capacity)
            {
                var oldestKey = keys.Dequeue();
                dictionary.Remove(oldestKey);
            }

            dictionary.Add(key, value);
            keys.Enqueue(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
        }
    }
}
