using UnityEngine;
using TMPro;

public class Panel_Wave : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI countdownText;

    private void OnEnable()
    {
        if (WaveManager.Instance == null) return;

        WaveManager.Instance.OnWaveStarted += HandleWaveStarted;
        WaveManager.Instance.OnWaveProgress += HandleWaveProgress;
        WaveManager.Instance.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        if (WaveManager.Instance == null) return;

        WaveManager.Instance.OnWaveStarted -= HandleWaveStarted;
        WaveManager.Instance.OnWaveProgress -= HandleWaveProgress;
        WaveManager.Instance.OnWaveCompleted -= HandleWaveCompleted;
    }

    private void HandleWaveStarted(int waveIndex, int totalEnemies)
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = $"Wave: {waveIndex}";
        }
        if (enemiesText != null)
        {
            enemiesText.gameObject.SetActive(true);
            enemiesText.text = $"Enemies: {totalEnemies}";
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void HandleWaveProgress(int waveIndex, int remainingEnemies, float remainingTime)
    {
        bool isLastWave = waveIndex == WaveManager.Instance.TotalWaves();

        if (remainingEnemies > 0)
        {
            // Enemy phase
            if (waveText != null)
            {
                waveText.gameObject.SetActive(true);
                waveText.text = $"Wave: {waveIndex}";
            }
            if (enemiesText != null)
            {
                enemiesText.gameObject.SetActive(true);
                enemiesText.text = $"Enemies: {remainingEnemies}";
            }
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }
        else if (remainingTime > 0f)
        {
            // Countdown phase
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                if (remainingTime <= 1f && isLastWave)
                    countdownText.text = $"Wave Complete!";
                else if (isLastWave)
                    countdownText.text = $"Wave Complete: {Mathf.CeilToInt(remainingTime)}s";
                else
                    countdownText.text = $"Next wave in: {Mathf.CeilToInt(remainingTime)}s";
            }

            if (waveText != null) waveText.gameObject.SetActive(false);
            if (enemiesText != null) enemiesText.gameObject.SetActive(false);
        }
        else
        {
            // Safety fallback
            if (waveText != null) waveText.gameObject.SetActive(true);
            if (enemiesText != null) enemiesText.gameObject.SetActive(true);
            if (countdownText != null) countdownText.gameObject.SetActive(false);
        }
    }

    private void HandleWaveCompleted(int waveIndex)
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = $"Wave {waveIndex} Completed!";
        }
        if (enemiesText != null)
        {
            enemiesText.gameObject.SetActive(true);
            enemiesText.text = "Enemies: 0";
        }
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }
}
