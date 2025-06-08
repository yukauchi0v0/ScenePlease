using UnityEngine;

public class AnimIntervalPlayer : MonoBehaviour
{
    public Animator animator;
    public string stateName = "character"; // 動畫狀態名稱
    public float interval = 2f; // 播放間隔時間
    public float playDuration = 1.5f; // 每次播放多久

    private float timer = 0f;
    private bool isPlaying = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (isPlaying)
        {
            if (timer >= playDuration)
            {
                animator.speed = 0f; // 暫停動畫
                timer = 0f;
                isPlaying = false;
            }
        }
        else
        {
            if (timer >= interval)
            {
                animator.Play(stateName, 0, 0f); // 重新播放動畫
                animator.speed = 1f; // 開啟動畫
                timer = 0f;
                isPlaying = true;
            }
        }
    }
}
