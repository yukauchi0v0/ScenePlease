using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float followSpeed = 5f;

    private bool canFollow = false;
    private bool isTransitioning = false;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentOffset;

    /// <summary>
    /// 啟用跟隨攝影機，使用更平滑的滑動過渡
    /// </summary>
    public void EnableFollow(bool smooth = true)
    {
        canFollow = true;
        currentOffset = offset;

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

        Vector3 desiredPosition = target.position + offset;

        if (isTransitioning)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.6f);

            if (Vector3.Distance(transform.position, desiredPosition) < 0.05f)
                isTransitioning = false;  // 過渡完成
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        }
    }
}
