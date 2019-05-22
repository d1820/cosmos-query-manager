using System.Collections.Generic;

namespace CosmosManager.Utilities
{
    public class FixedLimitDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary;
        private Queue<TKey> _keys;
        private readonly int _capacity;

        public FixedLimitDictionary(int capacity)
        {
            _keys = new Queue<TKey>(capacity);
            _capacity = capacity;
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                if (_dictionary.Count == _capacity)
                {
                    var oldestKey = _keys.Dequeue();
                    _dictionary.Remove(oldestKey);
                }

                _dictionary.Add(key, value);
                _keys.Enqueue(key);
            }
            catch
            {
                //NO-OP
            }
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public TValue this[TKey key] => _dictionary[key];
    }
}