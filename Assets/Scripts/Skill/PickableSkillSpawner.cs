using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickableSkillSpawnData
{
    public PickableSkill skillPrefab;
    public Transform spawnPoint;
    public float spawnTime;
    [HideInInspector] public bool spawned = false;
}

public class PickableSkillSpawner : MonoBehaviour
{
    public List<PickableSkillSpawnData> spawnList;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float gameTime = GameManager.Instance.GetGameDuration();

            foreach (var data in spawnList)
            {
                if (!data.spawned && gameTime >= data.spawnTime)
                {
                    if (data.skillPrefab != null && data.spawnPoint != null)
                    {
                        Instantiate(data.skillPrefab, data.spawnPoint.position, data.spawnPoint.rotation);
                        data.spawned = true;
                        Debug.Log($"Spawned skill {data.skillPrefab.name} at {data.spawnPoint.position} at time {data.spawnTime}");
                    }
                }
            }

            yield return null;
        }
    }

    public void ResetSpawner()
    {
        foreach (var data in spawnList)
        {
            data.spawned = false;
        }
    }
}
