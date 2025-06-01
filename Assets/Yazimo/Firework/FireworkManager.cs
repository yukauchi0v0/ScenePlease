using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FireworkManager : MonoBehaviour
{
    [Header("煙火效果")]
    public GameObject fireworkPrefab;
    public Transform player;
    public float spawnRadius = 3f;

    [Header("Input System MIDI 旋鈕控制")]
    public InputAction midiKnob; // 綁定 <MidiDevice>/control*/value
    private float lastMidiValue = 0f;

    [Header("Input System 發射鍵")]
    public InputAction fireButton; // 綁定手柄或鍵盤按鍵

    [Header("音效")]
    public AudioClip[] fireworkSounds;
    public float soundDelay = 0.2f;
    private AudioSource audioSource;

    [Header("高度調整")]
    [Tooltip("施放高度偏移（加在原本的隨機 Y 高度上）")]
    public float heightOffset = 0f;

    void Awake()
    {
        midiKnob.Enable();
        fireButton.Enable();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnDestroy()
    {
        midiKnob.Disable();
        fireButton.Disable();
    }

    void Update()
    {
        // MIDI 滑桿控制
        float currentMidiValue = midiKnob.ReadValue<float>();
        if (lastMidiValue < 0.5f && currentMidiValue >= 0.5f)
        {
            SpawnFirework();
        }
        lastMidiValue = currentMidiValue;

        // 普通按鍵或手柄控制
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
            Random.Range(0.5f, 2f) + heightOffset,
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
