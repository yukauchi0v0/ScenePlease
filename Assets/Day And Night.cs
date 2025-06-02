using UnityEngine;

public class OptimizedDayNightController : MonoBehaviour
{
    public TimeControlInput input;

    [Header("漸變控制")]
    [SerializeField] private float transitionSpeed = 1.5f;
    [SerializeField] private float dayDurationInSeconds = 60f;

    [Header("MIDI 控制滑桿啟用")]
    [SerializeField] private bool midiToggleEnabled = true; // 是否開啟 MIDI 切換功能
    [SerializeField] private float sliderToggleThreshold = 0.5f; // 超過這個值就視為開啟滑桿控制

    [Header("快捷鍵對應時間點")]
    [SerializeField] private float dayTimeValue = 0.1f;
    [SerializeField] private float duskTimeValue = 0.55f;
    [SerializeField] private float nightTimeValue = 0.85f;

    [Header("光線與顏色")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Gradient fogGradient;
    [SerializeField] private Gradient ambientGradient;
    [SerializeField] private Gradient directionLightGradient;
    [SerializeField] private Gradient skyboxTintGradient;

    private float currentTime = 0f;
    private float manualTargetTime = 0f;

    private enum ControlMode
    {
        Auto,
        Slider,
        ManualLerp
    }

    private ControlMode mode = ControlMode.Auto;

    void Start()
    {
        manualTargetTime = currentTime;
    }

    void Update()
    {
        if (input == null) return;

        // MIDI 開關：控制是否使用滑桿模式
        bool useSliderControl = midiToggleEnabled && input.sliderToggleValue > sliderToggleThreshold;

        // 快捷鍵輸入：指定目標時間，並轉為手動漸變
        if (input.setDay)
        {
            manualTargetTime = dayTimeValue;
            mode = ControlMode.ManualLerp;
        }
        else if (input.setDusk)
        {
            manualTargetTime = duskTimeValue;
            mode = ControlMode.ManualLerp;
        }
        else if (input.setNight)
        {
            manualTargetTime = nightTimeValue;
            mode = ControlMode.ManualLerp;
        }

        // 決定控制模式
        if (useSliderControl)
        {
            mode = ControlMode.Slider;
        }
        else if (mode != ControlMode.ManualLerp)
        {
            mode = ControlMode.Auto;
        }

        // 根據模式設定目標時間
        switch (mode)
        {
            case ControlMode.Auto:
                manualTargetTime += Time.deltaTime / dayDurationInSeconds;
                manualTargetTime = Mathf.Repeat(manualTargetTime, 1f);
                break;

            case ControlMode.Slider:
                manualTargetTime = Mathf.Clamp01(input.sliderValue);
                break;

            case ControlMode.ManualLerp:
                if (Mathf.Abs(currentTime - manualTargetTime) < 0.005f)
                {
                    mode = ControlMode.Auto;
                }
                break;
        }

        // 漸變靠近目標時間
        currentTime = Mathf.Lerp(currentTime, manualTargetTime, Time.deltaTime * transitionSpeed);

        UpdateLighting(currentTime);
        RotateSkybox();
        input.ResetFlags();
    }

    void UpdateLighting(float time)
    {
        float sunPosition = Mathf.Repeat(time + 0.25f, 1f);
        directionalLight.transform.rotation = Quaternion.Euler(sunPosition * 360f, 0f, 0f);

        RenderSettings.fogColor = fogGradient.Evaluate(time);
        RenderSettings.ambientLight = ambientGradient.Evaluate(time);
        directionalLight.color = directionLightGradient.Evaluate(time);
        skyboxMaterial.SetColor("_Tint", skyboxTintGradient.Evaluate(time));
    }

    void RotateSkybox()
    {
        float currentRotation = skyboxMaterial.GetFloat("_Rotation");
        float newRotation = currentRotation + 1f * Time.deltaTime;
        skyboxMaterial.SetFloat("_Rotation", newRotation);
    }

    private void OnApplicationQuit()
    {
        skyboxMaterial.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f));
    }
}
