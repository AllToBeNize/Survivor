using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node_WinPanel : MonoBehaviour
{
    //public TextMeshProUGUI TimeText;

    //private void OnEnable()
    //{
    //    TimeText.SetText(FormatTime_HMS(GameManager.Instance.GetGameDuration()));
    //}

    public void RePlay()
    {
        LevelManager.Instance.Play();
    }

    public void ReturnMenu()
    {
        LevelManager.Instance.ReturnMenu();
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
