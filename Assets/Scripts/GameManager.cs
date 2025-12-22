using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameOverReason
{
    Dead,
    Win,
}

public class GameManager : SceneSingleton<GameManager>
{
    public bool IsPause { get; private set; }

    public bool IsGameOver { get; private set; }

    public UnityAction<bool> OnPauseChanged;

    public UnityAction<GameOverReason> OnGameOverd;

    private float gameDuration = 0f;

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsPause)
        {
            gameDuration += Time.deltaTime;
        }
    }

    public float GetGameDuration()
    {
        return gameDuration;
    }

    public void SetGameOver(GameOverReason reason)
    {
        IsGameOver = true;
        SetPause(true);
        OnGameOverd?.Invoke(reason);
    }

    public void SetPause(bool isPause)
    {
        if (IsPause == isPause)
            return;

        IsPause = isPause;

        // 核心：真正暂停游戏
        Time.timeScale = IsPause ? 0f : 1f;

        // 通知外部（UI / 音效 / 输入）
        OnPauseChanged?.Invoke(IsPause);

        // 鼠标状态（如果是 FPS）
        if (IsPause)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        SetPause(!IsPause);
    }
}
