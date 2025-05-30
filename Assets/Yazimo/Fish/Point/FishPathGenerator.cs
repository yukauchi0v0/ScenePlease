using UnityEngine;

public class FishPathGenerator : MonoBehaviour
{
    public float speed = 1f;
    public float radius = 5f;
    public float height = 4f;
    public float funnelTightness = 0.5f;

    public bool idleSpin = false;   // 是否只在中心附近打轉

    private float t = 0f;
    [Header("控制目標")]
[SerializeField] public Transform dynamicCenter;


    public Vector3 GetNextPosition()
    {
        t += Time.deltaTime * speed;

        float funnelFactor = Mathf.Lerp(1f, funnelTightness, Mathf.Sin(t * 0.5f) * 0.5f + 0.5f);
        float x = Mathf.Cos(t) * radius * funnelFactor;
        float z = Mathf.Sin(t) * radius * funnelFactor;
        float y = Mathf.Sin(t * 0.5f) * height * 0.5f;

        Vector3 centerPos = dynamicCenter != null ? dynamicCenter.position : Vector3.zero;

        return centerPos + (idleSpin ? new Vector3(x, y, z) : Vector3.zero);
    }
}
