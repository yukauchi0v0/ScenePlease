using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using FMOD.Studio;

public class FMOD_MasterController : MonoBehaviour
{
    [Header("=== 雨聲控制 ===")]
    public StudioEventEmitter rainEmitter;
    public InputAction rainKnob; // ⬅ 改用 Input System

    [Header("=== 風聲控制 ===")]
    public StudioEventEmitter windEmitter;
    public InputAction windKnob; // ⬅ 改用 Input System

    [Header("=== 車流控制 ===")]
    public EventReference carAmbienceEvent;
    public string carTag = "Car";
    public float carsToMaxDensity = 10f;

    [Header("=== 高度控制風聲 ===")]
    public Transform playerTransform;
    public float minHeight = 30f;
    public float maxHeight = 100f;
    public float manualHoldTime = 2f;

    private float lastRain = -1f;
    private float lastWind = -1f;
    private float currentDensity = 0f;
    private EventInstance carAmbienceInstance;

    private float windManualOverride = -1f;
    private float manualTimer = 0f;
    private bool hasTouchedWindKnob = false;
    private bool hasInitializedWind = false;

    void OnEnable()
    {
        rainKnob.Enable();
        windKnob.Enable();
    }

    void OnDisable()
    {
        rainKnob.Disable();
        windKnob.Disable();
    }

    void Start()
    {
        if (carAmbienceEvent.IsNull)
        {
            Debug.LogWarning("Car Ambience Event 尚未指定！");
            return;
        }

        carAmbienceInstance = RuntimeManager.CreateInstance(carAmbienceEvent);
        carAmbienceInstance.start();
    }

    void Update()
    {
        UpdateRainControl();
        UpdateWindControl();
        UpdateCarDensity();
    }

    void UpdateRainControl()
    {
        if (rainEmitter == null) return;

        float current = rainKnob.ReadValue<float>();
        if (Mathf.Abs(current - lastRain) > 0.001f && rainEmitter.EventInstance.isValid())
        {
            rainEmitter.EventInstance.setParameterByName("Rain", current);
            lastRain = current;
        }
    }

    void UpdateWindControl()
    {
        if (windEmitter == null || playerTransform == null) return;

        float current = windKnob.ReadValue<float>();

        if (!hasInitializedWind)
        {
            lastWind = current;
            hasInitializedWind = true;
        }

        bool knobMoved = Mathf.Abs(current - lastWind) > 0.001f;
        float y = playerTransform.position.y;
        float autoValue = Mathf.Clamp01(Mathf.InverseLerp(minHeight, maxHeight, y));
        float finalValue;

        if (knobMoved)
        {
            hasTouchedWindKnob = true;
            lastWind = current;

            if (current <= 0.01f)
            {
                windManualOverride = 0f;
                manualTimer = -1f;
            }
            else
            {
                windManualOverride = current;
                manualTimer = 0f;
            }
        }

        if (!hasTouchedWindKnob)
        {
            finalValue = autoValue;
        }
        else if (manualTimer < 0f)
        {
            finalValue = 0f;
        }
        else
        {
            finalValue = Mathf.Lerp(windManualOverride, autoValue, manualTimer / manualHoldTime);
            manualTimer += Time.deltaTime;

            if (manualTimer >= manualHoldTime)
            {
                windManualOverride = -1f;
                hasTouchedWindKnob = false;
            }
        }

        if (windEmitter.EventInstance.isValid())
        {
            windEmitter.EventInstance.setParameterByName("Wind", finalValue);
        }
    }

    void UpdateCarDensity()
    {
        if (!carAmbienceInstance.isValid()) return;

        GameObject[] cars = GameObject.FindGameObjectsWithTag(carTag);
        int carCount = cars.Length;
        float target = Mathf.Clamp01(carCount / carsToMaxDensity);
        currentDensity = Mathf.Lerp(currentDensity, target, Time.deltaTime * 2f);
        carAmbienceInstance.setParameterByName("Traffic", currentDensity);
    }

    void OnDestroy()
    {
        if (carAmbienceInstance.isValid())
        {
            carAmbienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            carAmbienceInstance.release();
        }
    }
}
