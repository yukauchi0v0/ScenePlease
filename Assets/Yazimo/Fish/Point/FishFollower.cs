using UnityEngine;

public class FishFollower : MonoBehaviour
{
    public FishPathGenerator pathGenerator;
    public float baseSpeed = 2f;
    public float turnSpeed = 2f;
    public float offsetRadius = 2f;

    private Vector3 individualOffset;
    private float delayTimer;
    private float delayDuration;
    private Vector3 lastTargetPosition;

    void Start()
    {
        individualOffset = Random.insideUnitSphere * offsetRadius;
        individualOffset.y *= 0.3f;

        delayDuration = Random.Range(0.5f, 3.5f);  // 每隻魚的延遲不同
        delayTimer = Random.Range(0f, delayDuration);

        lastTargetPosition = transform.position;
    }

    void Update()
    {
        if (pathGenerator == null) return;

        delayTimer += Time.deltaTime;
        if (delayTimer >= delayDuration)
        {
            Vector3 pathPos = pathGenerator.GetNextPosition();
            lastTargetPosition = pathPos + individualOffset;
            delayTimer = 0f;
        }

        Vector3 direction = (lastTargetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        float moveSpeed = baseSpeed * Random.Range(0.9f, 1.1f); // 每條魚速度也有微小隨機
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
