using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledObject : MonoBehaviour
{
    protected ObjectPool pool;

    public void SetPool(ObjectPool ownerPool)
    {
        pool = ownerPool;
    }

    public virtual void OnSpawn()
    {
    }

    public virtual void OnDespawn()
    {
    }

    public void Despawn()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
