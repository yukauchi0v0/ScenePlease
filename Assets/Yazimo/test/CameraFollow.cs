using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float followSpeed = 5f;

    [Header("Y 軸追蹤倍率")]
    public float yFollowMultiplier = 1f;

    [Header("補償設定（鳥用）")]
    public bool useCompensation = false;
    public float compensationStrength = 0.5f; // 補償程度
    public float compensationSpeed = 5f;      // 補償平滑度

    private bool canFollow = false;
    private bool isTransitioning = false;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentOffset;
    private Vector3 lastTargetPos;

    public void EnableFollow(bool smooth = true)
    {
        canFollow = true;
        currentOffset = offset;
        if (target != null) lastTargetPos = target.position;

        if (smooth && target != null)
        {
            isTransitioning = true;
        }
        else
        {
            transform.position = target.position + offset;
            isTransitioning = false;
        }
    }

    void LateUpdate()
    {
        if (!canFollow || target == null) return;

        Vector3 desiredOffset = offset;

        // 🟡 補償邏輯（鳥控制時才開）
        if (useCompensation)
        {
            Vector3 targetDelta = target.position - lastTargetPos;
            Vector3 forwardComp = targetDelta.normalized * Mathf.Clamp(targetDelta.magnitude, 0f, 1f) * compensationStrength;
            desiredOffset -= forwardComp; // 相對拉後鏡頭
        }

        Vector3 desiredPosition = target.position + desiredOffset;

        if (isTransitioning)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.6f);
            if (Vector3.Distance(transform.position, desiredPosition) < 0.05f)
                isTransitioning = false;
        }
        else
        {
            Vector3 current = transform.position;
            float lerpSpeed = Time.deltaTime * followSpeed;
            transform.position = new Vector3(
                Mathf.Lerp(current.x, desiredPosition.x, lerpSpeed),
                Mathf.Lerp(current.y, desiredPosition.y, lerpSpeed * yFollowMultiplier),
                Mathf.Lerp(current.z, desiredPosition.z, lerpSpeed)
            );
        }

        lastTargetPos = target.position;
    }
}
