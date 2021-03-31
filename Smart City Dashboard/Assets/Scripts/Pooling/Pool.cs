using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T>
{
    private Queue<T> pool = new Queue<T>();
    public HashSet<T> loaned = new HashSet<T>();

    public Pool() { }

    /*public Pool(MonoBehaviour original, int fillCount)
    {
        Fill(original, fillCount);
        MonoBehaviour.Destroy(original);
    }

    private void Fill(MonoBehaviour original, int fillCount)
    {
        for (int i = 0; i < fillCount; i++)
        {
            pool.Enqueue(MonoBehaviour.Instantiate(original));
        }
    }*/

    public void Reclaim(T poolable)
    {
        if (loaned.Contains(poolable)) loaned.Remove(poolable);
        pool.Enqueue(poolable);
    }

    public T Loan()
    {
        if (pool.Count < 1)
        {
            throw new System.Exception("Out of pool space!");
        }
        else
        {
            T poolable = pool.Dequeue();
            loaned.Add(poolable);
            return poolable;
        }
    }

    public void Clear()
    {
        pool.Clear();
        loaned.Clear();
    }

    public bool CanLoan() => pool.Count > 0;
}
