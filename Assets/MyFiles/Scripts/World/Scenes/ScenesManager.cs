using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public string NowSceneName { get; private set; }
    private string nextSceneName;

    public event Action<float> OnLoadingProgress; // 0..1

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;

        if (!string.IsNullOrEmpty(NowSceneName))
            OnSceneClosing(NowSceneName);

        SceneManager.LoadScene(GameScenes.Loading);
        SceneManager.sceneLoaded += OnLoadSceneOpened;
    }

    private void OnLoadSceneOpened(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameScenes.Loading) return;

        SceneManager.sceneLoaded -= OnLoadSceneOpened;
        StartCoroutine(LoadTargetSceneAsync());
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            // Debug.Log(op.progress);
            OnLoadingProgress?.Invoke(op.progress / 0.9f); // обновляем UI
            yield return null;
        }

        OnLoadingProgress?.Invoke(1f); // финальный апдейт
        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        NowSceneName = nextSceneName;
        OnSceneOpening(NowSceneName);
    }

    private void OnSceneClosing(string sceneName)
    {
        Debug.Log($"Закрываем сцену {sceneName}");
        // 👉 Тут вызываешь сохранение: квесты, позиции, прогресс и т.п.
    }

    private void OnSceneOpening(string sceneName)
    {
        Debug.Log($"Открываем сцену {sceneName}");
        // 👉 Тут можно загрузить игрока, восстановить статы, прогресс и т.д.
    }
}