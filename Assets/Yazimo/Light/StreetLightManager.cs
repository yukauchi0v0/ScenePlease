using UnityEngine;
using UnityEngine.InputSystem;

public class StreetLightManager : MonoBehaviour
{
    public InputAction midiKnob; // MIDI 控制滑桿，綁定 <MidiDevice>/control*/value
    public StreetLightUnit[] streetLights; // 自動找或手動指定皆可

    [Header("MIDI 數值對應的最大亮度")]
    public float maxBrightness = 1f;

    void OnEnable()
    {
        midiKnob.Enable();
    }

    void OnDisable()
    {
        midiKnob.Disable();
    }

    void Update()
    {
        float value = midiKnob.ReadValue<float>(); // 0 ~ 1

        float brightness = Mathf.Clamp01(value) * maxBrightness;

        foreach (var light in streetLights)
        {
            if (light != null)
                light.SetBrightness(brightness);
        }
    }

    // 開場自動抓場景裡所有燈
    void Start()
    {
        streetLights = FindObjectsOfType<StreetLightUnit>();
    }
}
