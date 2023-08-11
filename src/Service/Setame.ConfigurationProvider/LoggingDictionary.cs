using System.Collections;

namespace Setame.ConfigurationProvider;

public class LoggingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();
    // private readonly ILogger _logger;

    public LoggingDictionary()
    {
        //   _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public TValue this[TKey key]
    {
        get
        {
            return _internalDictionary[key];
        }
        set
        {
            _internalDictionary[key] = value;
        }
    }

    public ICollection<TKey> Keys => _internalDictionary.Keys;
    public ICollection<TValue> Values => _internalDictionary.Values;
    public int Count => _internalDictionary.Count;
    public bool IsReadOnly => _internalDictionary.IsReadOnly;

    public void Add(TKey key, TValue value) => _internalDictionary.Add(key, value);
    public bool ContainsKey(TKey key) => _internalDictionary.ContainsKey(key);
    public bool Remove(TKey key) => _internalDictionary.Remove(key);
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _internalDictionary.TryGetValue(key, out value);
    }
    public void Add(KeyValuePair<TKey, TValue> item) => _internalDictionary.Add(item);
    public void Clear() => _internalDictionary.Clear();
    public bool Contains(KeyValuePair<TKey, TValue> item) => _internalDictionary.Contains(item);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _internalDictionary.CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<TKey, TValue> item) => _internalDictionary.Remove(item);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _internalDictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _internalDictionary.GetEnumerator();
}