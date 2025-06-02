using UnityEngine;
using UnityEngine.InputSystem;

public class BGMusicVolumeController : MonoBehaviour
{
    [Header("控制的音樂來源（可留空，自動尋找）")]
    public AudioSource bgmSource;

    [Header("Input System MIDI 旋鈕 Action")]
    public InputAction musicKnob;

    [Header("音量倍率 (可選)")]
    [Range(0f, 2f)] public float volumeMultiplier = 1f;

    [Header("預設啟動音量")]
    [Range(0f, 1f)] public float defaultStartVolume = 0.5f;

    [Header("更新頻率控制")]
    public float updateInterval = 0.05f;

    [Header("數值變動閾值")]
    public float threshold = 0.01f;

    [Header("平滑過渡速度")]
    public float lerpSpeed = 5f;

    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private float updateTimer = 0f;
    private bool midiStarted = false;

    void OnEnable() => musicKnob.Enable();
    void OnDisable() => musicKnob.Disable();

    void Start()
    {
        if (bgmSource == null)
        {
            Camera cam = GameObject.FindObjectOfType<Camera>();
            if (cam != null)
            {
                bgmSource = cam.GetComponent<AudioSource>();
                if (bgmSource != null)
                    Debug.Log("找到了 AudioSource，並已連接！");
                else
                    Debug.LogWarning("找到了 Camera，但沒有 AudioSource！");
            }
            else
            {
                Debug.LogWarning("場景中找不到任何 Camera！");
            }
        }

        if (bgmSource != null)
        {
            targetVolume = defaultStartVolume;
            currentVolume = defaultStartVolume;
            bgmSource.volume = defaultStartVolume;
        }
    }

    void Update()
    {
        if (bgmSource == null || musicKnob == null) return;

        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;

            float knobValue = musicKnob.ReadValue<float>();
            float newVolume = Mathf.Clamp01(knobValue * volumeMultiplier);

            if (newVolume > 0.001f && Mathf.Abs(newVolume - targetVolume) > threshold)
            {
                targetVolume = newVolume;
                midiStarted = true;
            }
        }

        currentVolume = Mathf.Lerp(currentVolume, targetVolume, lerpSpeed * Time.deltaTime);
        bgmSource.volume = currentVolume;
    }
}
