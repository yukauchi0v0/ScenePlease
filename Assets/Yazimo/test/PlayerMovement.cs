using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("基本移動速度")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 10f;
    public float acceleration = 5f;

    [Header("初始化位置")]
    public Vector3 resetPosition = new Vector3(-107.81f, 2.23f, 165.8f);

    private float currentSpeed;

    private PlayerControls controls;
    private Vector2 gamepadMoveInput = Vector2.zero; // 左搖桿
    private float gamepadVerticalInput = 0f;         // 右搖桿 Y
    private bool isGamepadSprinting = false;


    void Awake()
    {
        controls = new PlayerControls();

        // 左搖桿（水平 + 前後）
        controls.Player.Move.performed += ctx => gamepadMoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => gamepadMoveInput = Vector2.zero;

        // 右搖桿 Y 軸（上下）
        controls.Player.VerticalMove.performed += ctx => gamepadVerticalInput = ctx.ReadValue<float>();
        controls.Player.VerticalMove.canceled += ctx => gamepadVerticalInput = 0f;

        controls.Player.Sprint.performed += ctx => isGamepadSprinting = true;
        controls.Player.Sprint.canceled += ctx => isGamepadSprinting = false;

    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        // 判斷是否按下 Shift（加速）
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || isGamepadSprinting;

        float targetSpeed = isSprinting ? sprintSpeed : normalSpeed;

        // 平滑加速 / 減速
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // 鍵盤輸入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float y = 0f;
        if (Input.GetKey(KeyCode.R)) y = 1f;
        if (Input.GetKey(KeyCode.F)) y = -1f;

        // 合併鍵盤 + 手柄輸入
        Vector3 moveDir = new Vector3(
            h + gamepadMoveInput.x,
            y + gamepadVerticalInput,
            v + gamepadMoveInput.y
        );

        // 執行移動
        transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);

        // 回到指定初始位置（X鍵）
        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.position = resetPosition;
        }
    }
}
