using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 targetPoint;
    private Camera mainCam;
    private Renderer rend;
    private bool initialized = false;

    private float lifetime = 0f;
    public float deleteProtectTime = 0.8f; // 保護時間

    [Header("漂浮效果")]
    public float floatAmplitude = 0.2f;     // 漂浮上下最大距離
    public float floatSpeed = 0.5f;         // 漂浮速度
    public float noiseScale = 1f;           // 自然變化感（可微調）

    private float noiseOffset;
    private float baseY; // 漂浮的基礎高度

    public void Init(Vector3 target, float customSpeed)
    {
        targetPoint = target;
        speed = customSpeed;
        initialized = true;
    }

    void Start()
    {
        mainCam = Camera.main;
        rend = GetComponentInChildren<Renderer>();
        baseY = transform.position.y;
        noiseOffset = Random.Range(0f, 100f); // 每艘潛艇獨特飄法
    }

    void Update()
    {
        if (!initialized || mainCam == null || rend == null) return;

        // 計算前進方向
        Vector3 movePos = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // 漂浮偏移
        float time = Time.time * floatSpeed + noiseOffset;
        float offsetY = (Mathf.PerlinNoise(time * noiseScale, 0f) * 2f - 1f) * floatAmplitude;

        // 合成移動與漂浮
        transform.position = new Vector3(movePos.x, baseY + offsetY, movePos.z);

        // 自動銷毀
        lifetime += Time.deltaTime;
        if (lifetime > deleteProtectTime && IsRendererCompletelyOffscreen())
        {
            Destroy(gameObject);
        }
    }

    bool IsRendererCompletelyOffscreen()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);
        return !GeometryUtility.TestPlanesAABB(planes, rend.bounds);
    }
}
