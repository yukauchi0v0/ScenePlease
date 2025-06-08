using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ControlSwitcherInputSystem : MonoBehaviour
{
    [Header("角色與魚")]
    public Transform player;
    public Transform fishTarget;

    [Header("控制元件")]
    public PlayerMovement playerMovement;
    public FishPathGenerator fishPath;

    [Header("攝影機控制")]
    public CameraFollow cameraFollow;
    public Vector3 fishCamOffset = new Vector3(0, 4, -6);
    public float fishFollowSpeed = 10f;
    public float defaultFollowSpeed = 6f;

    [Header("切換限制設定")]
    public Camera mainCamera;
    public bool requireInView = true;
    public float minVisibleTime = 0.3f;

    private Vector3 savedOffset;
    private float savedSpeed;
    private Transform savedTarget;
    private Vector3 fishStartPos;
    private float visibleTimer = 0f;

    private bool controllingFish = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (cameraFollow != null)
        {
            savedOffset = cameraFollow.offset;
            savedSpeed = cameraFollow.followSpeed;
            savedTarget = cameraFollow.target;
        }

        if (fishTarget != null)
            fishStartPos = fishTarget.position;
    }

    void Update()
    {
        if (requireInView && mainCamera != null)
        {
            if (IsFishVisible())
                visibleTimer += Time.deltaTime;
            else
                visibleTimer = 0f;
        }

        bool bPressed = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;
        bool tPressed = Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame;

        if ((bPressed || tPressed) && (!requireInView || visibleTimer >= minVisibleTime))
        {
            ToggleControl();
        }

        if (controllingFish)
        {
            float h = 0f, v = 0f, y = 0f;

            if (Gamepad.current != null)
            {
                h = Gamepad.current.leftStick.x.ReadValue();
                v = Gamepad.current.leftStick.y.ReadValue();
                y = Gamepad.current.rightStick.y.ReadValue();
            }
            else
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                if (Input.GetKey(KeyCode.R)) y = 1f;
                if (Input.GetKey(KeyCode.F)) y = -1f;
            }

            float inputMagnitude = new Vector3(h, y, v).magnitude;
            float speed = Mathf.Lerp(1.5f, 2f, inputMagnitude);
            Vector3 move = new Vector3(h, y, v).normalized * speed * Time.deltaTime;

            fishTarget.position += move;
        }
    }

    void ToggleControl()
    {
        controllingFish = !controllingFish;

        if (controllingFish)
        {
            // 切換到魚群
            if (cameraFollow != null)
            {
                savedOffset = cameraFollow.offset;
                savedSpeed = cameraFollow.followSpeed;
                savedTarget = cameraFollow.target;

                cameraFollow.target = fishTarget;
                cameraFollow.offset = fishCamOffset;
                cameraFollow.followSpeed = fishFollowSpeed;
                cameraFollow.EnableFollow(true);
            }

            if (fishPath != null)
            {
                fishPath.idleSpin = false;
                fishPath.enabled = true;
                fishPath.speed = 5f;
            }

            if (playerMovement != null)
                playerMovement.isMovementEnabled = false;

            FadePlayer(false);
        }
        else
        {
            // 切回玩家
            if (player != null)
                player.position = fishTarget.position;

            if (cameraFollow != null)
            {
                cameraFollow.target = savedTarget;
                cameraFollow.offset = savedOffset;
                cameraFollow.followSpeed = defaultFollowSpeed;
                cameraFollow.EnableFollow(true);
            }

            if (fishPath != null)
            {
                fishPath.idleSpin = true;
                fishPath.speed = 2f;
                fishTarget.position = fishStartPos;
            }

            if (playerMovement != null)
                playerMovement.isMovementEnabled = true;

            FadePlayer(true);
        }
    }

    void FadePlayer(bool fadeIn)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(fadeIn));
    }

    IEnumerator FadeCoroutine(bool fadeIn)
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

    bool IsFishVisible()
    {
        if (mainCamera == null || fishTarget == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Collider fishCol = fishTarget.GetComponent<Collider>();
        if (fishCol != null)
            return GeometryUtility.TestPlanesAABB(planes, fishCol.bounds);
        else
            return GeometryUtility.TestPlanesAABB(planes, new Bounds(fishTarget.position, Vector3.one));
    }
}
