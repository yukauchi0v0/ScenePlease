using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class BirdFlightControllerV2 : MonoBehaviour
{
    [Header("飛行參數")]
    public float glideSpeed = 4f;
    public float boostSpeed = 10f;
    public float rotationSmooth = 5f;
    public float acceleration = 5f;

    [Header("動畫速度")]
    public float glideAnimSpeed = 0.4f;
    public float boostAnimSpeed = 1.5f;

    [Header("傾斜參數（壓彎效果）")]
    public float maxRollAngle = 30f;
    public float rollSmooth = 4f;

    private Vector2 moveInput;
    private float verticalInput = 0f;
    [HideInInspector] public float boostAmount = 0f;

    private Vector3 currentVelocity;
    private Vector3 currentForward = Vector3.forward;

    private Animator animator;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.VerticalMove.performed += ctx => verticalInput = ctx.ReadValue<float>();
        controls.Player.VerticalMove.canceled += ctx => verticalInput = 0f;

        controls.Player.FlyBoost.performed += ctx => boostAmount = ctx.ReadValue<float>();
        controls.Player.FlyBoost.canceled  += ctx => boostAmount = 0f;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. 移動輸入方向
        Vector3 inputDir = new Vector3(moveInput.x, verticalInput, moveInput.y);
        bool hasInput = inputDir.sqrMagnitude > 0.01f;

                if (hasInput)
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            // 限制 Y 軸仰角與俯角
            float clampedY = Mathf.Clamp(inputDir.y, -0.7f, 0.6f); // -1 垂直向下，+1 垂直向上
            Vector3 adjustedDir = new Vector3(inputDir.x, clampedY, inputDir.z).normalized;

            currentForward = adjustedDir;
        }


        // 2. 計算角度差並動態調整轉向速度與壓彎
        float angleDiff = Vector3.Angle(transform.forward, currentForward);
        float dynamicTurnSpeed = Mathf.Lerp(rotationSmooth, rotationSmooth * 4f, angleDiff / 90f);

        // 3. 平滑轉向
        Quaternion targetRot = Quaternion.LookRotation(currentForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, dynamicTurnSpeed * Time.deltaTime);

        // 4. 飛行速度
        float targetSpeed = Mathf.Lerp(glideSpeed, boostSpeed, boostAmount);
        Vector3 moveVector = transform.forward * targetSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, moveVector, acceleration * Time.deltaTime);
        transform.position += currentVelocity * Time.deltaTime;

        // 5. 動畫速度
        if (animator != null)
        {
            animator.Play("Scene");
            animator.speed = Mathf.Lerp(glideAnimSpeed, boostAnimSpeed, boostAmount);
        }

        // 6. 壓彎傾斜（X軸保留仰角、Z軸壓彎）
        float targetRoll = -moveInput.x * Mathf.Lerp(maxRollAngle, maxRollAngle * 1.8f, angleDiff / 90f);
        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newZ = Mathf.Lerp(currentZ, targetRoll, Time.deltaTime * rollSmooth);

        transform.localRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            newZ
        );
    }
}
