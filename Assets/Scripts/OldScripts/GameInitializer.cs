using InputLayer;
using Player;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    PlayerController playerController;

    public States.SMController SMController;
    // private IInputController _inputController;
    // private PlayerInputActions _inputActions;

    private void Awake()
    {
        playerController = GameContext.PlayerController;
        // _inputActions = GameContext.InputActions;
        SMController = GameContext.SMController;
        // _inputController = GameContext.InputController;

        // Начальные состояния
        SMController.Initialize();

        // Инициализация ввода
        // _inputController.InitializeInput(_inputActions);
        // _inputActions.Enable();

        playerController.Awake();
        Debug.Log(GlobalGame.UIManager);
    }

    public void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }
    void FixedUpdate()
    {
        playerController.FixedUpdate();
        SMController.FixedUpdate();
    }
    void Update()
    {
        States.EventQueue.ProcessEvents();
        SMController.MainSM.UpdateState(); // для слоёв (паузы)

        if (GameContext.IsPause)
        {
            SMController.PauseUpdate();
        }
        else
        {
            playerController.Update();
            SMController.Update();
        }
    }
    void LateUpdate()
    {
        if (GameContext.IsPause)
        {
            SMController.PauseLateUpdate();
        }
        else
        {
            playerController.LateUpdate();
            GameContext.EmotionController.LateUpdate();
            SMController.LateUpdate();
        }
    }
}




public static class GameContext
{
    public static bool IsPause = false;
    public static PlayerController PlayerController { get; }
    public static Entities_1.BaseEntity PlayerEntity { get; }
    public static PlayerModelRotationSync PlayerModelRotationSync { get; }
    public static CamerasController CamerasController { get; }
    public static CameraPlayerController CameraPlayerController { get; }
    public static Dialog.DialogueTrigger DialogueTrigger { get; }
    public static PlayerAnimationController PlayerAnimationController { get; }
    // public static UIManager UIManager { get; }
    public static IInputController InputController { get; }
    public static PlayerInputActions InputActions { get; }
    public static EmotionController EmotionController { get; }

    public static States.SMController SMController { get; }
    public static States.SM MainSM { get; }

    static GameContext()
    {
        GameObject player = GameObject.Find("Player");
        Transform playerModel = player.transform.Find("PlayerModel");
        PlayerModelVRM playerModelVRM = new(playerModel);
        Entities_1.BaseEntity PlayerEntity = player.GetComponent<Entities_1.PlayerEntity>();
        GameObject playerCamera = GameObject.Find("PlayerCamera");
        GameObject uiManagerGameObject = GameObject.Find("UIManager");
        DialogueTrigger = new(player.transform.Find("DialogueTrigger"), PlayerEntity);

        PlayerController = new GroundPlayerController(player);
        PlayerController.OnGroundChanged += isGround => { States.Flags.Set(States.FlagName.Ground, isGround); };
        PlayerController.OnMoveChanged += isMove => { States.Flags.Set(States.FlagName.Move, isMove); };

        PlayerModelRotationSync = playerModel.GetComponent<PlayerModelRotationSync>();

        CamerasController = new();
        CameraPlayerController = new(playerCamera, CamerasController);

        PlayerAnimationController = new(playerModelVRM.model);
        EmotionController = new(playerModelVRM.model);

        // UIManager = uiManagerGameObject.GetComponent<UIManager>();

        InputActions = new PlayerInputActions();

        SMController = new();
        MainSM = SMController.MainSM;
        InputController = new InputController();
        new States.FlagsEvents(MainSM);
    }
}


