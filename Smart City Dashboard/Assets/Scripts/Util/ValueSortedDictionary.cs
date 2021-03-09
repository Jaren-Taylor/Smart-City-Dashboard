using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ValueSortedDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>
{
    private LinkedList<KeyValuePair<K, V>> sortedData;
    private Dictionary<K, LinkedListNode<KeyValuePair<K, V>>> dataBinding;

    public ValueSortedDictionary()
    {
        sortedData = new LinkedList<KeyValuePair<K, V>>();
        dataBinding = new Dictionary<K, LinkedListNode<KeyValuePair<K, V>>>();
    }

    public ValueSortedDictionary(Dictionary<K, V> initialValues)
    {
        sortedData = new LinkedList<KeyValuePair<K, V>>(from kvp in initialValues orderby kvp.Value ascending select kvp);
        dataBinding = new Dictionary<K, LinkedListNode<KeyValuePair<K, V>>>();

        var enumer = sortedData.First;
        while(enumer != null)
        {
            dataBinding.Add(enumer.Value.Key, enumer);
            enumer = enumer.Next;
        } 
    }

    public V this[K key]
    {
        get => GetValue(key);
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    private V GetValue(K key) => dataBinding[key].Value.Value;
    private void SetValue(K key, V value)
    {
        //sortedData.Remove(dataBinding[key].Value);

        //dataBinding[key].Value.Value =
    }
}
