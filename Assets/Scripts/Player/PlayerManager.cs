using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    private GameObject player;
    private AttributeBase player_attr;

    public void InitPlayer(GameObject player)
    {
        this.player = player;
        player_attr = player.GetComponent<AttributeBase>();
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public GameObject GetPlayer()
    {
        return this.player;
    }

    public AttributeBase GetPlayerAttribute()
    {
        return player_attr;
    }
}
