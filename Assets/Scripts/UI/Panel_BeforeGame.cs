using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_BeforeGame : MonoBehaviour
{
    public Transform PanelRoot;

    public void StartGame()
    {
        GameManager.Instance.StartGame();
        PanelRoot.gameObject.SetActive(false);
    }
}
