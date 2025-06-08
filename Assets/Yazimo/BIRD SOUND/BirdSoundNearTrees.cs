using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BirdSoundNearTrees : MonoBehaviour
{
    [Header("基本設定")]
    public string treeTag = "Tree";
    public float triggerDistance = 5f;

    [Header("音效參數")]
    public AudioClip[] birdClips;
    [Range(0f, 1f)] public float maxVolume = 0.4f;
    public float fadeDuration = 1f;
    public float minInterval = 2f;
    public float maxInterval = 5f;

    [Header("Input System MIDI 滑桿控制")]
    public InputAction midiKnob; // 在 Inspector 綁定 <MidiDevice>/control2/value

    private AudioSource audioSource;
    private bool isNearTree = false;
    private Coroutine playCoroutine;

    void Awake()
    {
        midiKnob.Enable(); // 啟用 InputAction
    }

    void OnDestroy()
    {
        midiKnob.Disable(); // 關閉 InputAction
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

        // 判斷角色是否靠近樹
        isNearTree = false;
        GameObject[] trees = GameObject.FindGameObjectsWithTag(treeTag);

        foreach (GameObject tree in trees)
        {
            float dist = Vector3.Distance(transform.position, tree.transform.position);
            if (dist <= triggerDistance)
            {
                isNearTree = true;
                break;
            }
        }

        // 決定是否播放鳥叫聲
        if (isNearTree && playCoroutine == null && isMidiAllowed)
        {
            playCoroutine = StartCoroutine(PlayBirdLoop());
        }
        else if ((!isNearTree || !isMidiAllowed) && playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
            StartCoroutine(FadeOut(audioSource));
        }
    }

    IEnumerator PlayBirdLoop()
    {
        while (isNearTree && midiKnob.ReadValue<float>() < 0.01f)
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
