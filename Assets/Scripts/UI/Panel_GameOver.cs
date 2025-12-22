using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_GameOver : MonoBehaviour
{
    public Transform PanelRoot;
    public Transform WinPanel;
    public Transform LosePanel;
    private void Start()
    {
        GameManager.Instance.OnGameOverd += OnGameOver;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOverd -= OnGameOver;
        }
    }

    private void OnGameOver(GameOverReason reason)
    {
        WinPanel.gameObject.SetActive(reason == GameOverReason.Win);
        LosePanel.gameObject.SetActive(reason == GameOverReason.Dead);
        PanelRoot.gameObject.SetActive(true);
    }
}
