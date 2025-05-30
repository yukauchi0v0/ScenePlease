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
    private Vector2 gamepadMoveInput = Vector2.zero;
    private float gamepadVerticalInput = 0f;
    private bool isGamepadSprinting = false;

    public bool isMovementEnabled = true;  // 新增：允許控制角色移動與否（由 ControlSwitcher 控制）

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => gamepadMoveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => gamepadMoveInput = Vector2.zero;

        controls.Player.VerticalMove.performed += ctx => gamepadVerticalInput = ctx.ReadValue<float>();
        controls.Player.VerticalMove.canceled += ctx => gamepadVerticalInput = 0f;

        controls.Player.Sprint.performed += ctx => isGamepadSprinting = true;
        controls.Player.Sprint.canceled += ctx => isGamepadSprinting = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start() => currentSpeed = normalSpeed;

    void Update()
    {
        if (!isMovementEnabled) return;  // ✅ 角色移動禁用

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || isGamepadSprinting;
        float targetSpeed = isSprinting ? sprintSpeed : normalSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float y = 0f;
        if (Input.GetKey(KeyCode.R)) y = 1f;
        if (Input.GetKey(KeyCode.F)) y = -1f;

        Vector3 moveDir = new Vector3(
            h + gamepadMoveInput.x,
            y + gamepadVerticalInput,
            v + gamepadMoveInput.y
        );

        transform.Translate(moveDir.normalized * currentSpeed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.position = resetPosition;
        }
    }
}
