using System.Collections;

namespace EventDictionaryLib;

public class EventDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private Bucket<TKey, TValue>?[] _buckets;
    
    public ICollection<TKey> Keys => ExtractItems(kvp => kvp.Key).ToList();
    public ICollection<TValue> Values => ExtractItems(kvp => kvp.Value).ToList();

    private int _bucketsCount;
    public int Count { get; private set; }
    public bool IsReadOnly => false;

    private const double _loadFactor = 0.75;
    private const int _resizeFactor = 2;
    private const int _initialCapacity = 10;
    public int Capacity { get; private set; }

    public event Action<KeyValuePair<TKey, TValue>>? OnAdd;
    public event Action<TKey>? OnRemove;
    public event Action<TKey, TValue, TValue>? OnUpdate;

    public EventDictionary()
    {
        Capacity = _initialCapacity;
        _bucketsCount = 0;
        Count = 0;
        _buckets = new Bucket<TKey, TValue>[Capacity];
    }

    public EventDictionary(IDictionary<TKey, TValue> dictionary) : this()
    {
        foreach (var item in dictionary)
        {
            Add(item);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in _buckets)
        {
            if (bucket != null)
            {
                foreach (var item in bucket)
                {
                    yield return item;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ExceptionHelper.CheckNull(item, "Added item cannot be null");
        
        if (ContainsKey(item.Key))
        {
            ExceptionHelper.ThrowExistsKey(item.Key);
        }
        
        var bucketIndex = GetIndex(item.Key);
        
        if ((double) _bucketsCount / Capacity > _loadFactor)
        {
            Resize();
        }

        CreateBucketIfNull(bucketIndex);

        _buckets[bucketIndex]!.Add(item);
        Count++;

        OnAdd?.Invoke(item);
    }
    
    public void Clear()
    {
        Capacity = _initialCapacity;
        _buckets = new Bucket<TKey, TValue>[Capacity];
        Count = _bucketsCount = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        ExceptionHelper.CheckNull(item, "Item cannot be null");
        var bucketIndex = GetIndex(item.Key);

        return _buckets[bucketIndex] != null && _buckets[bucketIndex]!.Contains(item.Key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ExceptionHelper.CheckNull(array, "Destination array cannot be null");
        ExceptionHelper.CheckRange(arrayIndex, 0, array.Length, "Index is out of range");
        ExceptionHelper.CheckRange(Count, 0, array.Length - arrayIndex, "The destination array has insufficient space");
        
        var currentIndex = arrayIndex;
        var extractedItems = ExtractItems(item => item);

        foreach (var kvp in extractedItems)
        {
            array[currentIndex] = kvp;
            currentIndex++;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        ExceptionHelper.CheckNull(item, $"Removed item cannot be null");
        return Remove(item.Key);
    }
    
    public void Add(TKey key, TValue value)
    {
        ExceptionHelper.CheckNull(key, $"Key to add cannot be null");
        ExceptionHelper.CheckNull(value, $"Added value cannot be null");
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool ContainsKey(TKey key)
    {
        ExceptionHelper.CheckNull(key, "Key to search cannot be null");
        var bucketIndex = GetIndex(key);
        return _buckets[bucketIndex] != null && _buckets[bucketIndex]!.Contains(key);
    }

    public void Update(TKey key, TValue value)
    {
        ExceptionHelper.CheckNull(key, "Key cannot be null");
        
        var bucketIndex = GetIndex(key);

        if (_buckets[bucketIndex] == null || !_buckets[bucketIndex]!.Contains(key))
        {
            ExceptionHelper.ThrowNotFoundKey(key);
        }

        var oldValue = _buckets[bucketIndex]!.Get(key);
        _buckets[bucketIndex]!.Update(key, value);
        OnUpdate?.Invoke(key, oldValue, value);
    }
    
    public bool Remove(TKey key)
    {
        ExceptionHelper.CheckNull(key, "Removed key cannot be null");
        
        var bucketIndex = GetIndex(key);
        var currentBucket = _buckets[bucketIndex];

        if (currentBucket == null || !currentBucket.Contains(key))
        {
            return false;
        }
        
        currentBucket.Remove(key);

        if (currentBucket.Length == 0)
        {
            _buckets[bucketIndex] = null;
            _bucketsCount--;
        }

        Count--;
        OnRemove?.Invoke(key);
        
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        ExceptionHelper.CheckNull(key, $"Searched key cannot be null");
        var bucketIndex = GetIndex(key);
        
        if (!ContainsKey(key))
        {
            value = default;
            return false;
        }

        value = _buckets[bucketIndex]!.Get(key);
        return true;
    }

    public TValue this[TKey key]
    {
        get
        {
            var index = GetIndex(key);
            
            if (_buckets[index] == null || !_buckets[index]!.Contains(key))
            {
                throw new KeyNotFoundException($"Key: {key} does not exists");
            }

            return _buckets[index]!.FirstOrDefault(item => key!.Equals(item.Key)).Value;
        }
        set
        {
            var bucketIndex = GetIndex(key);
            
            if(_buckets[bucketIndex] != null && _buckets[bucketIndex]!.Contains(key))
            {
                Update(key, value);
            }
            else
            {
                Add(key, value);
            }
        }
    }

    private void CreateBucketIfNull(int bucketIndex)
    {
        _buckets[bucketIndex] ??= new Bucket<TKey, TValue>();
        _bucketsCount++;
    }

    private IEnumerable<T> ExtractItems<T>(Func<KeyValuePair<TKey, TValue>, T> selector)
    {
        return _buckets
            .Where(bucket => bucket != null)
            .SelectMany(bucket => bucket!
                .Select(selector));
    }

    private int GetIndex(TKey key)
    {
        ExceptionHelper.CheckNull(key, $"Key cannot be null");
        
        return Math.Abs(key!.GetHashCode() % Capacity);
    }
    
    private void Resize()
    {
        var extracted = ExtractItems(item => item);
        Capacity *= _resizeFactor;
        _buckets = new Bucket<TKey, TValue>[Capacity];

        Rehash(extracted);
    }

    private void Rehash(IEnumerable<KeyValuePair<TKey, TValue>> extractedItems)
    {
        foreach (var item in extractedItems)
        {
            var bucketIndex = GetIndex(item.Key);
            CreateBucketIfNull(bucketIndex);
            _buckets[bucketIndex]!.Add(item);
        }
    }
}