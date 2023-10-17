using System.Collections;

namespace EventDictionaryLib;

public class Bucket<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
   public KeyValuePair<TKey, TValue>[] _items = Array.Empty<KeyValuePair<TKey, TValue>>();
   public int Length => _items.Length;

   public void Add(KeyValuePair<TKey, TValue> item)
   {
      Array.Resize(ref _items, _items.Length + 1);
      _items[^1] = item;
   }

   public void Remove(TKey key)
   {
      var removedCount = 0;

      for (int i = 0; i < _items.Length; i++)
      {
         if (_items[i].Key.Equals(key))
         {
            removedCount++;
         }
         else if (removedCount > 0)
         {
            _items[i - removedCount] = _items[i];
         }
      }

      if (removedCount > 0)
      {
         Array.Resize(ref _items, _items.Length - removedCount);
      }
   }

   public void Update(TKey key, TValue value)
   {
      for (var i = 0; i < _items.Length; i++)
      {
         if (_items[i].Key.Equals(key))
         {
            _items[i] = new KeyValuePair<TKey, TValue>(key, value);
            return;
         }
      }
   }

   public bool Contains(TKey key)
   {
      return _items.Any(item => item.Key.Equals(key));
   }

   public TValue Get(TKey key)
   {
      return _items.FirstOrDefault(item => item.Key.Equals(key)).Value;
   }
   
   public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
   {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>)_items).GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }
}