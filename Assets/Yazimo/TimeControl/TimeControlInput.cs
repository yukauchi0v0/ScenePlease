using UnityEngine;
using UnityEngine.InputSystem;

public class TimeControlInput : MonoBehaviour
{
    public float sliderValue;
    public float sliderToggleValue;
    public bool setDay;
    public bool setDusk;
    public bool setNight;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls(); // ✅ 一定要先初始化！

        controls.Player.ChangeTimeSlider.performed += ctx =>
        {
            sliderValue = ctx.ReadValue<float>();
            Debug.Log($"[MIDI] SliderValue: {sliderValue:F2}");
        };

        controls.Player.SliderToggle.performed += ctx =>
        {
            sliderToggleValue = ctx.ReadValue<float>();
            Debug.Log($"[MIDI] ToggleValue: {sliderToggleValue:F2}");
        };

        controls.Player.SetDay.performed += ctx =>
        {
            setDay = true;
            Debug.Log("[MIDI] SetDay pressed");
        };

        controls.Player.SetDusk.performed += ctx =>
        {
            setDusk = true;
            Debug.Log("[MIDI] SetDusk pressed");
        };

        controls.Player.SetNight.performed += ctx =>
        {
            setNight = true;
            Debug.Log("[MIDI] SetNight pressed");
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    public void ResetFlags()
    {
        setDay = false;
        setDusk = false;
        setNight = false;
    }
}
