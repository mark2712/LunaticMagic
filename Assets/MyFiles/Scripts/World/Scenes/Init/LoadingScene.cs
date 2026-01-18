using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    public Slider progressBar;
    public Text progressText;

    void Awake()
    {
        // GlobalGame.UIManager.Open(UI.UILayer.Loading);
    }

    // void Start()
    // {
    //     if (ScenesManager.Instance != null)
    //     {
    //         ScenesManager.Instance.OnLoadingProgress += UpdateUI;
    //     }
    // }

    // private void UpdateUI(float progress)
    // {
    //     if (progressBar != null)
    //         progressBar.value = progress;

    //     if (progressText != null)
    //         progressText.text = (progress * 100f).ToString("F0") + "%";
    // }

    void OnDestroy()
    {
        // if (ScenesManager.Instance != null) { ScenesManager.Instance.OnLoadingProgress -= UpdateUI; }
        // GlobalGame.UIManager.Close(UI.UILayer.Loading);
    }
}
