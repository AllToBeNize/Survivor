using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _isDestroyed = false;

    public static T Instance
    {
        get
        {
            if (_isDestroyed)
            {
                Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' was destroyed. Returning null.\nStackTrace:\n{System.Environment.StackTrace}");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject($"{typeof(T).Name} (Singleton)");
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }


    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // 当前对象不是单例，销毁自己，但不影响 _isDestroyed
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        _isDestroyed = false;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        // 只有当前实例是单例时才标记销毁
        if (_instance == this)
        {
            _isDestroyed = true;
            _instance = null;
        }
    }
}
