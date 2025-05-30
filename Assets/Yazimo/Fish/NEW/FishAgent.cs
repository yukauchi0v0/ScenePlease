using UnityEngine;
using System.Collections.Generic;

public class FishAgent : MonoBehaviour
{
    public FishManager manager;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float neighborDistance = 5f;
    public float avoidDistance = 2f;
    public float rotationSpeed = 2f;

    [Header("Natural Motion Settings")]
    public Vector2 randomSpeedRange = new Vector2(1f, 2.5f);
    public Vector2 randomScaleRange = new Vector2(0.3f, 0.6f);
    public float verticalWaveAmplitude = 0.5f;
    public float verticalWaveFrequency = 1.5f;

    private float mySpeed;
    private float baseY;
    private Vector3 velocity;

    void Start()
    {
        // 隨機速度
        mySpeed = Random.Range(randomSpeedRange.x, randomSpeedRange.y);

        // 隨機縮放
        float s = Random.Range(randomScaleRange.x, randomScaleRange.y);
        transform.localScale = Vector3.one * s;

        // 記錄初始 Y 座標
        baseY = transform.position.y;
    }

    void Update()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        int neighborCount = 0;

        foreach (FishAgent other in manager.fishAgents)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);

            if (dist < neighborDistance)
            {
                cohesion += other.transform.position;
                alignment += other.transform.forward;
                neighborCount++;

                if (dist < avoidDistance)
                    separation += (transform.position - other.transform.position) / dist;
            }
        }

        if (neighborCount > 0)
        {
            cohesion = (cohesion / neighborCount - transform.position).normalized;
            alignment = (alignment / neighborCount).normalized;
        }

        Vector3 direction = cohesion + separation + alignment;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 前進
        transform.Translate(Vector3.forward * mySpeed * Time.deltaTime);

        // 垂直擺動
        float yOffset = Mathf.Sin(Time.time * verticalWaveFrequency + GetInstanceID()) * verticalWaveAmplitude;
        Vector3 pos = transform.position;
        pos.y = baseY + yOffset;
        transform.position = pos;
    }
}
