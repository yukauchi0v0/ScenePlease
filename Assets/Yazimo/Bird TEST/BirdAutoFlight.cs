using UnityEngine;

public class BirdAutoFlight : MonoBehaviour
{
    [Header("活動範圍")]
    public Transform centerPoint;       // 中心點
    public float roamRadius = 8f;

    [Header("滑翔設定")]
    public float moveSpeed = 2f;
    public float turnSpeed = 2f;
    public float directionChangeInterval = 4f;

    [Header("上下浮動")]
    public float floatAmplitude = 0.4f;
    public float floatFrequency = 1.2f;

    private Vector3 targetPoint;
    private float changeTimer;
    private Vector3 baseY;
    private Vector3 initialPosition;

    void Start()
    {
        baseY = transform.position;
        initialPosition = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        changeTimer -= Time.deltaTime;
        if (changeTimer <= 0f || Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            PickNewTarget();
        }

        // 上下浮動
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // 移動方向（帶有 y 浮動）
        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 velocity = direction * moveSpeed;

        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, baseY.y + yOffset, transform.position.z);

        // 平滑轉向
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }
    }

    void PickNewTarget()
    {
        if (centerPoint == null)
            centerPoint = this.transform;

        Vector2 rand = Random.insideUnitCircle * roamRadius;
        Vector3 newTarget = centerPoint.position + new Vector3(rand.x, 0, rand.y);
        targetPoint = newTarget;

        changeTimer = directionChangeInterval;
    }

    // ✅ 外部呼叫：重置位置並重新開始巡航
    public void ResetToStartPoint()
    {
        transform.position = initialPosition;
        baseY = initialPosition;
        PickNewTarget();
    }
}
