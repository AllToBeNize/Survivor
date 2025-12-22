using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _applicationIsQuitting;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning(
                    $"[MonoSingleton] Instance '{typeof(T)}' requested after application quit. Returning null."
                );
                return null;
            }

            lock (_lock)
            {
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
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}
