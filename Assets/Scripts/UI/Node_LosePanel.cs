using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node_LosePanel : MonoBehaviour
{
    //public TextMeshProUGUI EnemyNumText;
    //public TextMeshProUGUI TimeText;

    //private void OnEnable()
    //{
    //    TimeText.SetText(FormatTime_HMS(GameManager.Instance.GetGameDuration()));
    //    int enemyNum = GameManager.Instance.GetEnemyNum();
    //    EnemyNumText.SetText($"排名 {enemyNum + 1}/100");
    //}

    public void RePlay()
    {
        LevelManager.Instance.Play();
    }

    public void ReturnMenu()
    {
        LevelManager.Instance.ReturnMenu();
    }

    public void Exit()
    {

    }

    public static string FormatTime_HMS(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int secs = totalSeconds % 60;

        return $"{hours:00}:{minutes:00}:{secs:00}";
    }

}
