using UnityEngine;
using UnityEngine.InputSystem;

public class LightManager : MonoBehaviour
{
    public InputAction midiKnob;

    [Header("路燈")]
    public StreetLightUnit[] streetLights;

    [Header("水晶燈")]
    public CrystalLightUnit[] crystalLights;

    public float maxBrightness = 1f;

    void OnEnable()
    {
        midiKnob.Enable();
    }

    void OnDisable()
    {
        midiKnob.Disable();
    }

    void Start()
    {
        // 自動找場景中的所有物件
        streetLights = FindObjectsOfType<StreetLightUnit>();
        crystalLights = FindObjectsOfType<CrystalLightUnit>();
    }

    void Update()
    {
        float value = Mathf.Clamp01(midiKnob.ReadValue<float>()) * maxBrightness;

        foreach (var light in streetLights)
        {
            if (light != null) light.SetBrightness(value);
        }

        foreach (var crystal in crystalLights)
        {
            if (crystal != null) crystal.SetBrightness(value);
        }
    }
}
