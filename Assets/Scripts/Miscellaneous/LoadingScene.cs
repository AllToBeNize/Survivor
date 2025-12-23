using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Loading Animation")]
    [SerializeField] private RectTransform loadingSpinner;
    [SerializeField] private float spinSpeed = 180f;

    [Header("Time")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float minLoadingTime = 1.5f;

    [Header("Progress Smooth")]
    [SerializeField] private float progressSmoothSpeed = 2.5f;

    private string nextSceneName;
    private float displayProgress = 0f;

    private void Start()
    {
        nextSceneName = LevelManager.Instance.GetTargetScene();

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("LoadingScene: target scene is null or empty!");
            return;
        }

        StartCoroutine(LoadFlow());
    }

    private void Update()
    {
        if (loadingSpinner != null)
        {
            loadingSpinner.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);
        }
    }

    IEnumerator LoadFlow()
    {
        yield return Fade(1f, 0f);

        float startTime = Time.time;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f || Time.time - startTime < minLoadingTime)
        {
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);

            displayProgress = Mathf.MoveTowards(
                displayProgress,
                realProgress,
                Time.deltaTime * progressSmoothSpeed
            );

            if (progressBar) progressBar.value = displayProgress;
            if (progressText)
                progressText.SetText($"Loading... {Mathf.RoundToInt(displayProgress * 100)}%");

            yield return null;
        }

        displayProgress = 1f;
        if (progressBar) progressBar.value = 1f;
        if (progressText) progressText.SetText("Complete!");

        yield return new WaitForSeconds(0.2f);

        yield return Fade(0f, 1f);

        op.allowSceneActivation = true;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}
