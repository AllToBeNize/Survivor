using System;
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

    [Header("Debug")]
    public bool debugMode = true;

    private int currentWaveIndex = 0;
    private int enemiesAliveThisWave = 0;
    private float remainingWaveTime = 0f;

    // Events
    public event System.Action<int, int> OnWaveStarted; // (waveIndex, totalEnemies)
    public event System.Action<int, int, float> OnWaveProgress; // (waveIndex, remainingEnemies, remainingTime)
    public event System.Action<int> OnWaveCompleted; // (waveIndex)

    private void Start()
    {
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.Playing)
        {
            StartWave();
        }
    }

    public void StartWave()
    {
        if (waveConfigs.Count > 0)
            StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (currentWaveIndex < waveConfigs.Count)
        {
            WaveConfigData wave = waveConfigs[currentWaveIndex];
            enemiesAliveThisWave = 0;

            // Spawn all enemies
            foreach (var enemyConfig in wave.enemies)
            {
                for (int i = 0; i < enemyConfig.count; i++)
                {
                    EnemyBase enemy = EnemyManager.Instance.SpawnEnemyRandom(enemyConfig.enemyType);
                    if (enemy != null)
                    {
                        var attr = enemy.GetAttribute();

                        // 先解绑自己之前绑定的回调（不会影响其他系统的绑定）
                        attr.OnDead -= OnEnemyDied;

                        // 再绑定
                        attr.OnDead += OnEnemyDied;

                        enemiesAliveThisWave++;
                    }
                }
            }

            int totalEnemies = enemiesAliveThisWave;
            OnWaveStarted?.Invoke(currentWaveIndex + 1, totalEnemies);

            if (debugMode)
                Debug.Log($"Wave {currentWaveIndex + 1} started. Enemies Remaining: {enemiesAliveThisWave}");

            // Wait for all enemies to die
            while (enemiesAliveThisWave > 0)
            {
                OnWaveProgress?.Invoke(currentWaveIndex + 1, enemiesAliveThisWave, 0f);
                yield return new WaitForSeconds(1f);
            }

            // Wait duration after wave
            remainingWaveTime = wave.durationAfterWave;
            while (remainingWaveTime > 0f)
            {
                OnWaveProgress?.Invoke(currentWaveIndex + 1, 0, remainingWaveTime);
                yield return new WaitForSeconds(1f);
                remainingWaveTime -= 1f;
            }

            OnWaveCompleted?.Invoke(currentWaveIndex + 1);

            if (currentWaveIndex == waveConfigs.Count - 1)
            {
                if (debugMode)
                    Debug.Log("Last wave completed. Ending game...");

                GameManager.Instance.SetGameOver(GameOverReason.Win);
            }

            currentWaveIndex++;
        }

        if (debugMode)
            Debug.Log("All waves completed!");
    }

    private void OnEnemyDied()
    {
        enemiesAliveThisWave--;
    }

    public int RemainingEnemiesInWave()
    {
        return enemiesAliveThisWave;
    }

    public float RemainingWaveTime()
    {
        return remainingWaveTime;
    }

    public int TotalWaves()
    {
        return waveConfigs.Count;
    }
}
