using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class MainMenuScene : MonoBehaviour
{
    public Button playMainButton;
    public Button playLevel1Button;
    // public GameObject GameProfilesPrefab;

    void Start()
    {
        // // UI главного меню
        // GameObject canvas = GameObject.Find("/Canvas");
        // // Открыть профили
        // Transform GameProfilesContainer = canvas.transform.Find("PanelMainMenu/GameProfilesContainer");
        // UI.UIComponent gameProfiles = new UI.ComponentProfiles(null).CreateRootNode().AddPrefab(PrefabManager.Load("Prefabs/UI/GameProfiles"), GameProfilesContainer.gameObject);

        // UIComponent gameProfiles = ComponentProfiles.Create(GameProfilesContainer.gameObject, GameProfilesPrefab, null);

        // Transform GameProfilesContainer = canvas.transform.Find("PanelMainMenu/GameProfilesContainer");
        // UIComponentContainer UIComponentContainer = GameProfilesContainer.GetComponent<UIComponentContainer>();
        // var componentProfiles = UIComponentContainer.CreateView<ComponentProfiles, Type>(null);
        // UIComponentContainer.OpenUI(componentProfiles);

        // GlobalGame.UIManager.Open(UI.UILayer.MainMenu);

        // playMainButton.onClick.AddListener(() => OnPlayClicked(GameScenes.Main));
        // playLevel1Button.onClick.AddListener(() => OnPlayClicked(GameScenes.Level1));
    }

    private float _timer;
    private float _timer2;
    private bool _createToggle;

    private void Update()
    {
        _timer += Time.deltaTime;
        _timer2 += Time.deltaTime;
        if (_timer >= 1f)
        {
            _timer = 0f;

            // // 1. Меняем имя у случайного профиля // сейчас у профиля 0
            // if (GlobalGame.Profiles.Profiles.Count > 0)
            // {
            //     // var randomProfile = GlobalGame.Profiles.Profiles.Values.ElementAt(UnityEngine.Random.Range(0, GlobalGame.Profiles.Profiles.Count));
            //     var randomProfile = GlobalGame.Profiles.Profiles.ElementAt(1).Value;
            //     randomProfile.Name.Value = "Time Name " + Time.time.ToString("F1");
            // }

            // if (Math.Round(_timer2) % 2 == 0)
            // {
            //     if (_createToggle == false)
            //     {
            //         string id = "profile_test";
            //         GlobalGame.Profiles.Create($"{id} CreatedAt " + Time.time.ToString("F1"), id, ProfileTypes.User);
            //         _createToggle = true;
            //     }
            //     else
            //     {
            //         string id = "profile_test";
            //         if (GlobalGame.Profiles.Profiles.ContainsKey(id))
            //         {
            //             GlobalGame.Profiles.Remove(id);
            //         }
            //         _createToggle = false;
            //     }
            // }
        }
    }

    void OnDestroy()
    {
        // GlobalGame.UIManager.Close(UI.UILayer.MainMenu);
    }

    void OnPlayClicked(string sceneName)
    {
        // Debug.Log(sceneName);
        ScenesManager.Instance.LoadScene(sceneName);
    }
}