using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class WallaLayeredController : MonoBehaviour
{
    [Header("=== Input System ===")]
    public InputAction inputAction;

    [Header("=== Main 音源 (0~1) ===")]
    public AudioSource mainLoopSource;

    [Header("=== Walla 聲音群 (50~100) ===")]
    public AudioClip[] wallaClips;
    public int maxSimultaneousWalla = 3;
    public float minInterval = 1f;
    public float maxInterval = 3f;

    [Header("Walla 聲音播放範圍音量")]
    public float wallaBaseVolume = 1f;

    [Range(0f, 1f)]
    public float wallaMaxVolume = 0.6f; // 二軌最大音量上限

    private float sliderValue = 0f;
    private List<AudioSource> wallaPlayers = new List<AudioSource>();
    private Coroutine wallaRoutine;

    void OnEnable()
    {
        inputAction.Enable();
    }

    void OnDisable()
    {
        inputAction.Disable();
    }

    void Start()
    {
        if (mainLoopSource != null)
            mainLoopSource.Play();

        wallaRoutine = StartCoroutine(WallaLoop());
    }

    void Update()
    {
        sliderValue = inputAction.ReadValue<float>(); // 0~1

        if (mainLoopSource != null)
            mainLoopSource.volume = sliderValue;
    }

    IEnumerator WallaLoop()
    {
        while (true)
        {
            float wallaFactor = Mathf.InverseLerp(0.5f, 1f, sliderValue);

            if (wallaFactor > 0.05f && wallaClips.Length > 0)
            {
                // 移除已播放完的來源
                wallaPlayers.RemoveAll(source => source == null || !source.isPlaying);

                if (wallaPlayers.Count < maxSimultaneousWalla)
                {
                    AudioSource newSource = gameObject.AddComponent<AudioSource>();
                    AudioClip clip = wallaClips[Random.Range(0, wallaClips.Length)];
                    newSource.clip = clip;

                    // 計算最終音量（不能超過 wallaMaxVolume）
                    float targetVolume = Mathf.Min(wallaFactor * wallaBaseVolume, wallaMaxVolume);
                    newSource.volume = targetVolume;

                    newSource.Play();
                    Destroy(newSource, clip.length + 0.1f);
                    wallaPlayers.Add(newSource);
                }
            }

            float interval = Random.Range(minInterval, maxInterval) * Mathf.Lerp(1.2f, 0.4f, sliderValue);
            yield return new WaitForSeconds(interval);
        }
    }
}
