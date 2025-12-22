using UnityEngine;

/// <summary>
/// 场景级单例（不跨场景）
/// - 不自动创建
/// - 跟随场景销毁
/// - 场景中必须手动放置
/// </summary>
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static bool _isQuitting;

    public static T Instance
    {
        get
        {
            if (_isQuitting)
                return null;

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError(
                $"[SceneSingleton] Multiple instances of {typeof(T).Name} in scene!",
                this
            );
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}
