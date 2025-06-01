using UnityEngine;

public class BirdCameraFollow : MonoBehaviour
{
    public Transform target;            // 要跟隨的目標（鳥）
    public Vector3 offset = new Vector3(0, 2, -10); // 固定偏移
    public float smoothSpeed = 5f;      // 平滑速度

    void LateUpdate()
    {
        if (target == null) return;

        // 目標位置 + 固定偏移（不跟隨 rotation）
        Vector3 desiredPosition = target.position + offset;

        // 平滑跟隨
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
