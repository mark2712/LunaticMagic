using UnityEngine;

public class FreeDebugCamera : MonoBehaviour
{
    [Header("Base")]
    public float baseSpeed = 1f;
    public float mouseSensitivity = 2f;

    private float speedPower = 0f; // 10^n
    private float yaw;
    private float pitch;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleMouseLook();
        HandleSpeedChange();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleSpeedChange()
    {
        if (Input.GetKeyDown(KeyCode.E))
            speedPower++;

        if (Input.GetKeyDown(KeyCode.Q))
            speedPower--;

        speedPower = Mathf.Clamp(speedPower, -3f, 6f);
    }

    void HandleMovement()
    {
        float speed = baseSpeed * Mathf.Pow(5f, speedPower);

        Vector3 move = Vector3.zero;

        // Плоскость
        move += transform.forward * Input.GetAxisRaw("Vertical");
        move += transform.right * Input.GetAxisRaw("Horizontal");

        // Высота
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up;

        if (Input.GetKey(KeyCode.LeftControl))
            move += Vector3.down;

        transform.position += move.normalized * speed * Time.deltaTime;
    }
}
