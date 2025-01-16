using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        [SerializeField] public Queue<GameObject> PoolQueue;
        [SerializeField] public GameObject PoolObject;
        [SerializeField] public int PoolSize = 20;       // 풀 크기
    }

    public static ObjectPool Instance;
    public GameObject Root;

    
    public Dictionary<Type, Pool> PoolDic = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(var pair in PoolDic)
        {
            for(int i = 0; i < pair.Value.PoolSize; i++)
            {
                pair.Value.PoolQueue.Enqueue(Instantiate(pair.Value.PoolObject));
            }
        }
    }

    public GameObject GetObj(Type type, GameObject ori, int poolSize = 20)
    {
        if (!PoolDic.ContainsKey(type))
        {
            PoolDic[type] = new();
            PoolDic[type].PoolObject = ori;
            PoolDic[type].PoolQueue = new();
            PoolDic[type].PoolSize = poolSize;

            for (int i = 0; i < PoolDic[type].PoolSize; i++)
            {
                var obj = Instantiate(PoolDic[type].PoolObject, Root.transform);
                obj.SetActive(false);
                PoolDic[type].PoolQueue.Enqueue(obj);
            }
        }
        var pool = PoolDic[type];
        if (pool.PoolQueue.Count > 0)
        {
            GameObject obj = pool.PoolQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(pool.PoolObject, Root.transform);
            pool.PoolSize++;
            obj.SetActive(true);
            return obj;
        }
    }

    public void Return(Type type, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
        obj.transform.SetParent(Root.transform);
        PoolDic[type].PoolQueue.Enqueue(obj);
    }
}
