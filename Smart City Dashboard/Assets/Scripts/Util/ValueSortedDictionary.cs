using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSortedDictionary <K, V> : Dictionary<K, V>
{
    private LinkedList<KeyValuePair<K, V>> sortedData;
    private Dictionary<K, LinkedListNode<KeyValuePair<K, V>>> dataBinding;


}
