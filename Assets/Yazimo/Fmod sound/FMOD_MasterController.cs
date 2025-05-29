using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using MidiJack;

public class FMOD_MasterController : MonoBehaviour
{
    [Header("=== 雨聲控制 ===")]
    public StudioEventEmitter rainEmitter;
    public int rainKnobIndex = 0;

    [Header("=== 風聲控制 ===")]
    public StudioEventEmitter windEmitter;
    public int windKnobIndex = 1;

    [Header("=== 車流控制 ===")]
    public EventReference carAmbienceEvent;
    public string carTag = "Car";
    public float carsToMaxDensity = 10f;

    [Header("=== 高度控制風聲 ===")]
    public Transform playerTransform;
    public float minHeight = 30f;   // 地面高度
    public float maxHeight = 100f;  // 天空島高度
    public float manualHoldTime = 2f;

    private float lastRain = -1f;
    private float lastWind = -1f;
    private float currentDensity = 0f;
    private EventInstance carAmbienceInstance;

    private float windAutoValue = 0f;
    private float windManualOverride = -1f; // -1 = 沒有手動控制
    private float manualTimer = 0f;
    private bool hasTouchedWindKnob = false;
    private bool hasInitializedWind = false;



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

        float current = MidiMaster.GetKnob(rainKnobIndex);
        if (Mathf.Abs(current - lastRain) > 0.001f && rainEmitter.EventInstance.isValid())
        {
            rainEmitter.EventInstance.setParameterByName("Rain", current);
            lastRain = current;
        }
    }

   void UpdateWindControl()
{
    if (windEmitter == null || playerTransform == null) return;

    float current = MidiMaster.GetKnob(windKnobIndex);

    // 第一次初始化：記錄初始值，不判斷為“使用者操作”
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
