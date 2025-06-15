using UnityEngine;

public class BirdAutoFlight : MonoBehaviour
{
    [Header("圓形飛行設定")]
    public Transform centerPoint;         // 中心點，可在 Inspector 設定或程式指定
    public float radius = 5f;             // 飛行半徑
    public float angularSpeed = 30f;      // 每秒旋轉角度（度）

    [Header("上下浮動")]
    public float floatAmplitude = 0.4f;
    public float floatFrequency = 1.2f;

    private float currentAngle = 0f;
    private Vector3 baseY;

    void Start()
    {
        if (centerPoint == null)
            centerPoint = this.transform;  // 如果沒指定中心點，就以自己為中心

        baseY = transform.position;
        currentAngle = 0f;
    }

    void Update()
    {
        if (!enabled) return; // 當控制器切換成玩家操作時，暫停自動飛行

        currentAngle += angularSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // 計算新位置（X-Z 平面上繞圈）
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * radius;

        // 加入上下浮動
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 targetPosition = centerPoint.position + offset + new Vector3(0, yOffset, 0);

        // 移動與旋轉朝向
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 2f * Time.deltaTime);
        transform.position += direction * Time.deltaTime * angularSpeed * Mathf.Deg2Rad * radius;
    }

    /// <summary>
    /// 回到起始點，並重設角度為 0 度
    /// </summary>
    public void ResetToStartPoint()
    {
        currentAngle = 0f;
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * radius;
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = centerPoint.position + offset + new Vector3(0, yOffset, 0);
    }
}
