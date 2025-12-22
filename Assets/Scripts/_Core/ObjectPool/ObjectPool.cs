using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly Stack<PooledObject> poolStack = new Stack<PooledObject>();
    private readonly PooledObject prefab;
    private readonly Transform root;

    public ObjectPool(PooledObject prefab, int preloadCount, Transform root = null)
    {
        this.prefab = prefab;
        this.root = root;

        for (int i = 0; i < preloadCount; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            poolStack.Push(obj);
        }
    }

    private PooledObject CreateNew()
    {
        var obj = Object.Instantiate(prefab, root);
        obj.SetPool(this);
        return obj;
    }

    public T Spawn<T>(Vector3 position, Quaternion rotation) where T : PooledObject
    {
        PooledObject obj;

        if (poolStack.Count > 0)
        {
            obj = poolStack.Pop();
        }
        else
        {
            obj = CreateNew();
        }

        var t = obj.transform;
        t.SetPositionAndRotation(position, rotation);

        obj.gameObject.SetActive(true);
        obj.OnSpawn();

        return obj as T;
    }

    public void Release(PooledObject obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        poolStack.Push(obj);
    }
}
