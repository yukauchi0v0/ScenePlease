using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BirdSoundNearBirds : MonoBehaviour
{
    [Header("基本設定")]
    public string birdTag = "Bird";
    public float triggerDistance = 5f;

    [Header("音效參數")]
    public AudioClip[] birdClips;
    [Range(0f, 1f)] public float maxVolume = 0.4f;
    public float fadeDuration = 1f;
    public float minInterval = 2f;
    public float maxInterval = 5f;

    [Header("Input System MIDI 滑桿控制")]
    public InputAction midiKnob; // Inspector 綁定 <MidiDevice>/control*/value（滑桿）

    private AudioSource audioSource;
    private bool isNearBird = false;
    private Coroutine playCoroutine;

    void Awake()
    {
        midiKnob.Enable();
    }

    void OnDestroy()
    {
        midiKnob.Disable();
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;  // 2D 音效
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
        audioSource.loop = false;
    }

    void Update()
    {
        float knobValue = midiKnob.ReadValue<float>();
        bool isMidiAllowed = knobValue < 0.01f;

        // 檢查是否靠近任何鳥
        isNearBird = false;
        GameObject[] birds = GameObject.FindGameObjectsWithTag(birdTag);

        foreach (GameObject bird in birds)
        {
            float dist = Vector3.Distance(transform.position, bird.transform.position);
            if (dist <= triggerDistance)
            {
                isNearBird = true;
                break;
            }
        }

        if (isNearBird && playCoroutine == null && isMidiAllowed)
        {
            playCoroutine = StartCoroutine(PlayBirdSoundLoop());
        }
        else if ((!isNearBird || !isMidiAllowed) && playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
            StartCoroutine(FadeOut(audioSource));
        }
    }

    IEnumerator PlayBirdSoundLoop()
    {
        while (isNearBird && midiKnob.ReadValue<float>() < 0.01f)
        {
            AudioClip clip = birdClips[Random.Range(0, birdClips.Length)];
            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeIn(audioSource));
            yield return new WaitForSeconds(clip.length);
            yield return StartCoroutine(FadeOut(audioSource));

            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator FadeIn(AudioSource source)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(0f, maxVolume, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = maxVolume;
    }

    IEnumerator FadeOut(AudioSource source)
    {
        float timer = 0f;
        float startVolume = source.volume;
        while (timer < fadeDuration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.volume = 0f;
    }
}
