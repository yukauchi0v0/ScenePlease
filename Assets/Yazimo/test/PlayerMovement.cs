using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("基本移動速度")]
    public float minSpeed = 3f;
    public float maxSpeed = 8f;
    public float acceleration = 5f;

    [Header("初始化位置")]
    public Vector3 resetPosition = new Vector3(-107.81f, 2.23f, 165.8f);

    [Header("角色圖像子物件")]
    public SpriteRenderer spriteRenderer; // 💡記得手動拖入角色圖片

    private float currentSpeed;
    private PlayerControls controls;
    private Vector2 gamepadMoveInput = Vector2.zero;
    private float gamepadVerticalInput = 0f;

    public bool isMovementEnabled = true;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => gamepadMoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => gamepadMoveInput = Vector2.zero;

        controls.Player.VerticalMove.performed += ctx => gamepadVerticalInput = ctx.ReadValue<float>();
        controls.Player.VerticalMove.canceled += ctx => gamepadVerticalInput = 0f;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start() => currentSpeed = minSpeed;

    void Update()
    {
        if (!isMovementEnabled) return;

        // 鍵盤加速（Shift）
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float keyboardTargetSpeed = isSprinting ? maxSpeed : minSpeed;

        // 計算輸入強度
        float gamepadInputMagnitude = new Vector3(gamepadMoveInput.x, gamepadVerticalInput, gamepadMoveInput.y).magnitude;
        float gamepadTargetSpeed = Mathf.Lerp(minSpeed, maxSpeed, gamepadInputMagnitude);

        // 最終目標速度
        float targetSpeed = (Gamepad.current != null) ? gamepadTargetSpeed : keyboardTargetSpeed;

        // 平滑加速
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // 鍵盤輸入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float y = 0f;
        if (Input.GetKey(KeyCode.R)) y = 1f;
        if (Input.GetKey(KeyCode.F)) y = -1f;

        // 合併輸入方向
        Vector3 moveDir = new Vector3(
            h + gamepadMoveInput.x,
            y + gamepadVerticalInput,
            v + gamepadMoveInput.y
        );

        // 移動角色（空物件）
        transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);

        // 🔁 翻轉子物件圖像（SpriteRenderer）
        if (moveDir.x > 0.01f)
            spriteRenderer.flipX = false;  // 朝右
        else if (moveDir.x < -0.01f)
            spriteRenderer.flipX = true;   // 朝左

        // 位置重置
        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.position = resetPosition;
        }
    }
}
