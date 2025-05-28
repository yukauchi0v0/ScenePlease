using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 targetPoint;
    private Camera mainCam;
    private Renderer rend;
    private bool initialized = false;

    private float lifetime = 0f;
    public float deleteProtectTime = 0.8f; // ⏱️ 生成後這段時間不刪除

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
    }

    void Update()
    {
        if (!initialized || mainCam == null || rend == null) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        lifetime += Time.deltaTime;

        // ❗只有在保護時間過後才判斷是否該刪除
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
