using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSwitcherInputSystem : MonoBehaviour
{
    public Transform player;
    public Transform fishTarget;
    public CameraFollow cameraFollow;
    public FishPathGenerator fishPath;
    public PlayerMovement playerMovement;

    [Header("魚群視角攝影機 Offset")]
    public Vector3 fishCamOffset = new Vector3(0, 4, -6);

    private bool controllingFish = false;
    private Vector3 savedCamOffset;

    [Header("魚群速度")]
    public float normalFishSpeed = 2f;
    public float boostedFishSpeed = 5f;

    void Start()
    {
        if (cameraFollow != null)
            savedCamOffset = cameraFollow.offset;

        if (fishPath != null)
            fishPath.speed = normalFishSpeed;
    }

    void Update()
    {
        bool bPressed = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;
        bool tPressed = Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame;

        if (bPressed || tPressed)
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
            savedCamOffset = cameraFollow.offset;
            cameraFollow.target = fishTarget;
            cameraFollow.offset = fishCamOffset;
            cameraFollow.EnableFollow(true); // 平滑切換

            if (fishPath != null)
            {
                fishPath.idleSpin = false;
                fishPath.speed = boostedFishSpeed;
            }

            if (playerMovement != null)
                playerMovement.isMovementEnabled = false;
        }
        else
        {
            cameraFollow.target = player;
            cameraFollow.offset = savedCamOffset;
            cameraFollow.EnableFollow(true); // 平滑切換

            if (fishPath != null)
            {
                fishPath.idleSpin = true;
                fishPath.speed = normalFishSpeed;
            }

            if (playerMovement != null)
                playerMovement.isMovementEnabled = true;
        }
    }
}
