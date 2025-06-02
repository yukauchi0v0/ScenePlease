using UnityEngine;
using UnityEngine.InputSystem;

public class BirdControlSwitcher : MonoBehaviour
{
    [Header("角色與鳥")]
    public Transform player;
    public Transform bird;

    [Header("控制元件")]
    public PlayerMovement playerMovement;
    public BirdFlightControllerV2 birdFlight;
    public BirdAutoFlight birdAuto;

    [Header("攝影機控制（主攝影機）")]
    public CameraFollow cameraFollow;
    public Vector3 birdCamOffset = new Vector3(0, 5, -8);

    [Header("攝影機追蹤速度")]
    public float defaultFollowSpeed = 6f;
    public float birdFollowSpeed = 12f;

    [Header("切換限制設定")]
    public Camera mainCamera; // ⬅️ 拖入主攝影機
    public bool requireInView = true;   // ⬅️ 啟用這個條件
    public float minVisibleTime = 0.3f; // ⬅️ 至少要持續看到這麼久

    private float visibleTimer = 0f;
    private Vector3 savedOffset;
    private Transform savedTarget;
    private float savedSpeed;
    private bool isControllingBird = false;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.BirdSwitch.performed += ctx =>
        {
            if (!requireInView || IsBirdVisible())
            {
                ToggleControl();
            }
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        savedTarget = player;
        savedOffset = cameraFollow != null ? cameraFollow.offset : Vector3.zero;
        savedSpeed = cameraFollow != null ? cameraFollow.followSpeed : 6f;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // 可選：如果你希望顯示 UI 提示（鳥在視野內）
        if (requireInView && mainCamera != null)
        {
            if (IsBirdVisible())
                visibleTimer += Time.deltaTime;
            else
                visibleTimer = 0f;
        }
    }

    bool IsBirdVisible()
    {
        if (mainCamera == null || bird == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Collider birdCollider = bird.GetComponent<Collider>();

        if (birdCollider != null)
            return GeometryUtility.TestPlanesAABB(planes, birdCollider.bounds);
        else
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(bird.position, Vector3.one));
    }

    void ToggleControl()
    {
        isControllingBird = !isControllingBird;
        SetControl(isControllingBird);
    }

    void SetControl(bool toBird)
    {
        if (cameraFollow == null) return;

        if (toBird)
{
    // 原有設定
    savedOffset = cameraFollow.offset;
    savedTarget = cameraFollow.target;
    savedSpeed = cameraFollow.followSpeed;

    cameraFollow.target = bird;
    cameraFollow.offset = birdCamOffset;
    cameraFollow.followSpeed = birdFollowSpeed;

    // ✅ 啟用補償
    cameraFollow.useCompensation = true;

    cameraFollow.EnableFollow(true);
}
else
{
    cameraFollow.target = savedTarget;
    cameraFollow.offset = savedOffset;
    cameraFollow.followSpeed = defaultFollowSpeed;

    // ✅ 關閉補償
    cameraFollow.useCompensation = false;

    cameraFollow.EnableFollow(true);
}


        if (playerMovement != null)
            playerMovement.isMovementEnabled = !toBird;

        if (birdFlight != null)
            birdFlight.enabled = toBird;

        if (birdAuto != null)
            birdAuto.enabled = !toBird;
    }
}
