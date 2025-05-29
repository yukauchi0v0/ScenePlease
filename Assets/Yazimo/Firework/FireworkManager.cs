using UnityEngine;
using UnityEngine.InputSystem;
using MidiJack;
using System.Collections;

public class FireworkManager : MonoBehaviour
{
    [Header("煙火效果")]
    public GameObject fireworkPrefab;
    public Transform player;
    public float spawnRadius = 3f;

    [Header("MIDI 設定")]
    public int knobIndex = 0; // MIDI 控制鍵 index

    [Header("音效")]
    public AudioClip[] fireworkSounds;
    public float soundDelay = 0.2f; // 音效延遲時間（秒）
    private AudioSource audioSource;

    [Header("Input System")]
    public InputAction fireButton; // 要在 Inspector 綁定或程式啟用

    private float lastMidiValue = 0f;

    void Awake()
    {
        fireButton.Enable();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnDestroy()
    {
        fireButton.Disable();
    }

    void Update()
    {
        // MIDI 控制
        float currentMidiValue = MidiMaster.GetKnob(knobIndex);
        if (lastMidiValue < 0.5f && currentMidiValue >= 0.5f)
        {
            SpawnFirework();
        }
        lastMidiValue = currentMidiValue;

        // Input System 控制
        if (fireButton.WasPressedThisFrame())
        {
            SpawnFirework();
        }
    }

    void SpawnFirework()
    {
        Vector3 behindDirection = -player.forward;
        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 2f),
            Random.Range(0.5f, 1.5f)
        );
        Vector3 spawnPos = player.position + behindDirection * spawnRadius + randomOffset;

        Vector3 fireDirection = Vector3.up + new Vector3(
            Random.Range(-0.3f, 0.3f),
            0,
            Random.Range(-0.3f, 0.3f)
        );

        Quaternion fireRotation = Quaternion.LookRotation(fireDirection.normalized);
        Instantiate(fireworkPrefab, spawnPos, fireRotation);

        if (fireworkSounds.Length > 0)
        {
            StartCoroutine(PlaySoundDelayed(soundDelay));
        }
    }

    IEnumerator PlaySoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioClip clip = fireworkSounds[Random.Range(0, fireworkSounds.Length)];
        audioSource.PlayOneShot(clip);
    }
}
