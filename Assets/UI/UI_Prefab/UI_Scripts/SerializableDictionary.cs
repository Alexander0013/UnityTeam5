using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    // ISerializationCallbackReceiver 接口中的方法，用於序列化與反序列化
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    // 實現索引器
    public TValue this[TKey key]
    {
        get
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                throw new KeyNotFoundException("Key not found: " + key);
        }
        set
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }

    // 添加和刪除字典項
    public void Add(TKey key, TValue value) => dictionary.Add(key, value);
    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
    public bool Remove(TKey key) => dictionary.Remove(key);
    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

    // 返回字典項的數量
    public int Count => dictionary.Count;

    // 返回所有值
    public IEnumerable<TValue> Values => dictionary.Values;
}
