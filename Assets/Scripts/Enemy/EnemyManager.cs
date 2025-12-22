using UnityEngine;
using System.Collections.Generic;

public enum EnemyType
{
    Melee,
    Ranged,
    Boss
}

[System.Serializable]
public class EnemyPoolConfig
{
    public EnemyType type;
    public EnemyBase prefab;
    public int poolSize = 10;
}

public class EnemyManager : MonoSingleton<EnemyManager>
{
    [Header("Enemy Pool Configs")]
    public EnemyPoolConfig[] poolConfigs;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("Debug")]
    public bool debugMode = false;

    [Header("Spawn Settings")]
    public float spawnOffsetRadius = 1f; // random offset radius
    public float minDistanceBetweenEnemies = 1f; // minimum distance to nearby enemies

    private Dictionary<EnemyType, ObjectPool> pools = new Dictionary<EnemyType, ObjectPool>();
    private Transform playerCache = null;
    private List<EnemyBase> activeEnemies = new List<EnemyBase>();

    protected override void Awake()
    {
        base.Awake();

        foreach (var config in poolConfigs)
        {
            if (!pools.ContainsKey(config.type))
            {
                pools[config.type] = new ObjectPool(config.prefab, config.poolSize, this.transform);
            }
        }
    }

    private Transform GetPlayer()
    {
        if (playerCache == null)
        {
            var playerObj = PlayerManager.Instance.GetPlayer();
            if (playerObj != null)
            {
                playerCache = playerObj.transform;
            }
            else if (debugMode)
            {
                Debug.LogWarning("EnemyManager: Player not found!");
            }
        }
        return playerCache;
    }

    public EnemyBase SpawnEnemy(EnemyType type, Vector3 position)
    {
        if (!pools.ContainsKey(type))
        {
            if (debugMode)
                Debug.LogWarning($"EnemyManager: No pool for type {type}");
            return null;
        }

        EnemyBase enemy = pools[type].Spawn<EnemyBase>(position, Quaternion.identity);

        if (enemy.TryGetComponent(out UnityEngine.AI.NavMeshAgent agent) && agent.isOnNavMesh)
            agent.Warp(position);

        enemy.SetTarget(GetPlayer());

        activeEnemies.Add(enemy);
        enemy.GetAttribute().OnDead += () => { activeEnemies.Remove(enemy); };

        if (debugMode)
            Debug.Log($"Spawned {enemy.name} of type {type} at {position}");

        return enemy;
    }

    public EnemyBase SpawnEnemyRandom(EnemyType type)
    {
        if (spawnPoints.Length == 0)
        {
            if (debugMode)
                Debug.LogWarning("EnemyManager: No spawn points defined!");
            return null;
        }

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 spawnPos = FindValidSpawnPosition(spawn.position);

        return SpawnEnemy(type, spawnPos);
    }

    private Vector3 FindValidSpawnPosition(Vector3 origin)
    {
        Vector3 pos = origin;
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-spawnOffsetRadius, spawnOffsetRadius),
                0f,
                Random.Range(-spawnOffsetRadius, spawnOffsetRadius)
            );
            Vector3 candidate = origin + offset;

            if (UnityEngine.AI.NavMesh.SamplePosition(candidate, out UnityEngine.AI.NavMeshHit hit, spawnOffsetRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                candidate = hit.position;
            }

            bool tooClose = false;
            foreach (var enemy in activeEnemies)
            {
                if (Vector3.Distance(candidate, enemy.transform.position) < minDistanceBetweenEnemies)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                pos = candidate;
                break;
            }
        }

        return pos;
    }

    public void DespawnEnemy(EnemyType type, EnemyBase enemy)
    {
        if (pools.ContainsKey(type))
        {
            activeEnemies.Remove(enemy);
            pools[type].Release(enemy);
        }
        else if (debugMode)
        {
            Debug.LogWarning($"EnemyManager: No pool for type {type}");
        }
    }
}
