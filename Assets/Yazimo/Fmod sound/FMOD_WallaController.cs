using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using FMOD.Studio;

public class FMOD_WallaController : MonoBehaviour
{
    [Header("Walla 控制參數")]
    public StudioEventEmitter wallaEmitter;
    public InputAction wallaKnob; // Input System 滑桿控制

    private float lastWallaValue = -1f;

    void OnEnable()
    {
        wallaKnob.Enable();
    }

    void OnDisable()
    {
        wallaKnob.Disable();
    }

    void Start()
    {
        if (wallaEmitter != null)
        {
            wallaEmitter.Play();

            // 讓 Walla 音軌觸發一次
            wallaEmitter.EventInstance.setParameterByName("Walla", 0.01f);
            Debug.Log("✅ WallaEmitter 播放並初始化參數");
        }
        else
        {
            Debug.LogWarning("🚫 Walla Emitter 尚未設定");
        }
    }

    void Update()
    {
        if (wallaEmitter == null || !wallaEmitter.EventInstance.isValid()) return;

        float current = wallaKnob.ReadValue<float>();

        if (Mathf.Abs(current - lastWallaValue) > 0.001f)
        {
            wallaEmitter.EventInstance.setParameterByName("Walla", current);
            lastWallaValue = current;

            Debug.Log($"✅ 設定 Walla 參數為：{current}");
        }
    }
}
