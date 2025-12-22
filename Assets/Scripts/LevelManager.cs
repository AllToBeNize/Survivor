using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    [Header("Scene Config")]
    [SerializeField] private string loadingSceneName = "Loading";

    public string StartSceneName = "Start";

    public string GameSceneName = "Game";

    private string targetSceneName = "Game";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void ReturnMenu()
    {
        LoadLevel(StartSceneName);
    }

    public void Play()
    {
        LoadLevel(GameSceneName);
    }

    /// <summary>
    /// 对外统一接口
    /// </summary>
    public void LoadLevel(string sceneName)
    {
        targetSceneName = sceneName;
        SceneManager.LoadScene(loadingSceneName);
    }

    /// <summary>
    /// Loading 场景调用
    /// </summary>
    public string GetTargetScene()
    {
        return targetSceneName;
    }
}
