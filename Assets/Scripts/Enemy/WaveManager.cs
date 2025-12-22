using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveEnemyConfig
{
    public EnemyType enemyType;
    public int count;
}

[System.Serializable]
public class WaveConfigData
{
    public List<WaveEnemyConfig> enemies = new List<WaveEnemyConfig>();
    public float durationAfterWave; // Time to wait after this wave before next wave
}

public class WaveManager : MonoSingleton<WaveManager>
{
    public List<WaveConfigData> waveConfigs = new List<WaveConfigData>();

    private int currentWaveIndex = 0;
    private int enemiesAliveThisWave = 0;

    private void Start()
    {
        if (waveConfigs.Count > 0)
        {
            StartCoroutine(SpawnWaveRoutine());
        }
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (currentWaveIndex < waveConfigs.Count)
        {
            WaveConfigData wave = waveConfigs[currentWaveIndex];
            enemiesAliveThisWave = 0;

            // Spawn all enemies in this wave immediately
            foreach (var enemyConfig in wave.enemies)
            {
                for (int i = 0; i < enemyConfig.count; i++)
                {
                    EnemyBase enemy = EnemyManager.Instance.SpawnEnemyRandom(enemyConfig.enemyType);
                    if (enemy != null)
                    {
                        enemy.GetAttribute().OnDead += () => { enemiesAliveThisWave--; };
                        enemiesAliveThisWave++;
                    }
                }
            }

            // Wait for all enemies to be dead
            while (enemiesAliveThisWave > 0)
            {
                yield return null;
            }

            // Wait duration between waves
            yield return new WaitForSeconds(wave.durationAfterWave);

            currentWaveIndex++;
        }

        Debug.Log("All waves completed!");
    }

    public int RemainingEnemiesInWave()
    {
        return enemiesAliveThisWave;
    }
}
