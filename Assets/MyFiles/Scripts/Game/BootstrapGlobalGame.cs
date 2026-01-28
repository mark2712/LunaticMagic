using UnityEngine;

public class BootstrapGlobalGame : MonoBehaviour
{
    private void Awake()
    {
        if (GlobalGame.Bootstrap != null && GlobalGame.Bootstrap != this)
        {
            Destroy(gameObject); // Если Bootstrap уже существует и это не мы — уничтожаем себя
            return;
        }

        // Если Bootstrap еще нет — назначаем себя
        GlobalGame.Bootstrap = this;
        DontDestroyOnLoad(gameObject);
        // Debug.Log(GlobalGame.Bootstrap);
        Init();
    }

    private void Init()
    {
        GlobalGame.UIManager.Init();
        GlobalGame.Session.Init();
        GlobalGame.UIGlobalState.Init();
    }

    private void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked; 
        // Cursor.visible = false;

        // SpawnVrm spawnVrm = new();
        // spawnVrm.Spawn();
    }

    private void FixedUpdate()
    {
        GlobalGame.Session.FixedUpdate();
    }

    private void Update()
    {
        GlobalGame.Session.Update();
    }

    private void LateUpdate()
    {
        GlobalGame.Session.LateUpdate();
        GlobalGame.UIController.RenderAll();
    }
}


// private void Init()
// {
//     GlobalGame.Load();
//     // Debug.Log(GlobalGame.Settings.MaxFPS);
//     // GlobalGame.Save();
//     GlobalGame.Profiles.Load();
//     // GlobalGame.Profiles.Save();

//     GlobalGame.UIManager.Start();
//     // GlobalGame.GameSession.Start();

//     // ScenesManager.Instance.LoadScene(GameScenes.MainMenu);

//     // загрузить профили
//     // загрузить системную сессию для главного меню
// }
