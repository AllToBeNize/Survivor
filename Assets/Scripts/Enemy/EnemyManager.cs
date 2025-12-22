using UnityEngine;


public enum EnemyType
{
    Melee,
    Ranged,
    Boss
}
public class EnemyManager : MonoSingleton<EnemyManager>
{
    [Header("Enemy Prefab & Pool")]
    public EnemyBase enemyPrefab;
    public int poolSize = 20;
    public Transform poolRoot;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("Debug")]
    public bool debugMode = false;

    private ObjectPool pool;
    private Transform playerCache = null;

    protected override void Awake()
    {
        base.Awake();

        pool = new ObjectPool(enemyPrefab, poolSize, poolRoot);
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
    public EnemyBase SpawnEnemy(Vector3 position)
    {
        EnemyBase enemy = pool.Spawn<EnemyBase>(position, Quaternion.identity);

        if (enemy.TryGetComponent(out UnityEngine.AI.NavMeshAgent agent) && agent.isOnNavMesh)
            agent.Warp(position);

        enemy.SetTarget(GetPlayer());

        if (debugMode)
            Debug.Log($"Spawned {enemy.name} at {position}");

        return enemy;
    }

    public EnemyBase SpawnEnemyRandom()
    {
        if (spawnPoints.Length == 0)
        {
            if (debugMode)
                Debug.LogWarning("EnemyManager: No spawn points defined!");
            return null;
        }

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return SpawnEnemy(spawn.position);
    }

    public void DespawnEnemy(EnemyBase enemy)
    {
        pool.Release(enemy);
    }
}
