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
    public Camera mainCamera;
    public bool requireInView = true;
    public float minVisibleTime = 0.3f;

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
            savedOffset = cameraFollow.offset;
            savedTarget = cameraFollow.target;
            savedSpeed = cameraFollow.followSpeed;

            cameraFollow.target = bird;
            cameraFollow.offset = birdCamOffset;
            cameraFollow.followSpeed = birdFollowSpeed;
            cameraFollow.useCompensation = true;
            cameraFollow.EnableFollow(true);

            FadePlayer(false); // 淡出角色
        }
        else
        {
            player.position = bird.position;

            cameraFollow.target = savedTarget;
            cameraFollow.offset = savedOffset;
            cameraFollow.followSpeed = defaultFollowSpeed;
            cameraFollow.useCompensation = false;
            cameraFollow.EnableFollow(true);

            FadePlayer(true); // 淡入角色

            // ✅ 呼叫：讓鳥回去原本滑翔邏輯
            if (birdAuto != null)
            {
                birdAuto.ResetToStartPoint();
            }
        }

        if (playerMovement != null)
            playerMovement.isMovementEnabled = !toBird;

        if (birdFlight != null)
            birdFlight.enabled = toBird;

        if (birdAuto != null)
            birdAuto.enabled = !toBird;
    }

    void FadePlayer(bool visible)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(visible));
    }

    System.Collections.IEnumerator FadeCoroutine(bool fadeIn)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        var renderers = player.GetComponentsInChildren<SpriteRenderer>();

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            foreach (var r in renderers)
            {
                if (r != null)
                {
                    Color c = r.color;
                    c.a = currentAlpha;
                    r.color = c;
                }
            }

            yield return null;
        }

        foreach (var r in renderers)
        {
            if (r != null)
            {
                Color c = r.color;
                c.a = endAlpha;
                r.color = c;
            }
        }
    }
}
