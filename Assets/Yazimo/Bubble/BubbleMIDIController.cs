using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleInputSystemController : MonoBehaviour
{
    [Header("要控制的粒子系統（左右）")]
    public ParticleSystem leftBubble;
    public ParticleSystem rightBubble;

    [Header("Input System MIDI 滑桿 Action")]
    public InputAction bubbleKnob;

    [Header("速度範圍 (Start Speed)")]
    public float minSpeed = 0f;
    public float maxSpeed = 3f;

    [Header("數量範圍 (Rate over Time)")]
    public float minRate = 0f;
    public float maxRate = 30f;

    [Header("大小範圍 (Start Size)")]
    public float minSize = 0.1f;
    public float maxSize = 0.5f;

    [Header("壽命範圍 (Start Lifetime)")]
    public float minLifetime = 2f;
    public float maxLifetime = 14f;

    private ParticleSystem.MainModule leftMain;
    private ParticleSystem.EmissionModule leftEmission;

    private ParticleSystem.MainModule rightMain;
    private ParticleSystem.EmissionModule rightEmission;

    void OnEnable() => bubbleKnob.Enable();
    void OnDisable() => bubbleKnob.Disable();

    void Start()
    {
        if (leftBubble != null)
        {
            leftMain = leftBubble.main;
            leftEmission = leftBubble.emission;
        }

        if (rightBubble != null)
        {
            rightMain = rightBubble.main;
            rightEmission = rightBubble.emission;
        }
    }

    void Update()
    {
        float midiValue = bubbleKnob.ReadValue<float>(); // 0 ~ 1

        float mappedSpeed = Mathf.Lerp(minSpeed, maxSpeed, midiValue);
        float mappedRate = Mathf.Lerp(minRate, maxRate, midiValue);
        float mappedSize = Mathf.Lerp(minSize, maxSize, midiValue);
        float mappedLifetime = Mathf.Lerp(minLifetime, maxLifetime, midiValue);

        if (leftBubble != null)
        {
            leftMain.startSpeed = mappedSpeed;
            leftMain.startSize = mappedSize;
            leftMain.startLifetime = mappedLifetime;
            leftEmission.rateOverTime = mappedRate;
        }

        if (rightBubble != null)
        {
            rightMain.startSpeed = mappedSpeed;
            rightMain.startSize = mappedSize;
            rightMain.startLifetime = mappedLifetime;
            rightEmission.rateOverTime = mappedRate;
        }
    }
}
