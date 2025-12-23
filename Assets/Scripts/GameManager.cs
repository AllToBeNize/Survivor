using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GamePhase
{
    None,
    Prepare,
    Playing,
    GameOver
}

public enum GameOverReason
{
    Dead,
    Win,
}

public class GameManager : SceneSingleton<GameManager>
{
    public bool IsPause { get; private set; }
    public bool IsGameOver { get; private set; }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.None;

    public UnityAction<bool> OnPauseChanged;
    public UnityAction<GameOverReason> OnGameOverd;
    public UnityAction<GamePhase> OnPhaseChanged;

    private float gameDuration = 0f;

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;

        SetPhase(GamePhase.Prepare);
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

    public void StartGame()
    {
        SetPhase(GamePhase.Playing);
        SetPause(false);
    }

    public void SetGameOver(GameOverReason reason)
    {
        if (IsGameOver)
            return;

        IsGameOver = true;
        SetPhase(GamePhase.GameOver);
        SetPause(true);
        OnGameOverd?.Invoke(reason);
    }

    public void TogglePause()
    {
        SetPause(!IsPause);
    }

    public void SetPause(bool isPause)
    {
        if (IsPause == isPause)
            return;

        IsPause = isPause;
        Time.timeScale = IsPause ? 0f : 1f;
        OnPauseChanged?.Invoke(IsPause);
        Cursor.visible = IsPause;
    }

    private void SetPhase(GamePhase phase)
    {
        if (CurrentPhase == phase)
            return;

        CurrentPhase = phase;
        OnPhaseChanged?.Invoke(phase);
    }

    protected override void OnDestroy()
    {
        Time.timeScale = 1f;
        base.OnDestroy();
    }
}
