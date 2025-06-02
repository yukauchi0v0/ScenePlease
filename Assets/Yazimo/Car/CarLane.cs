using UnityEngine;

public class CarLane : MonoBehaviour
{
    [Tooltip("車輛是否從右邊進入（否則從左邊）")]
    public bool isRightToLeft = false;

    [Tooltip("該車道深度（可用來調整縮放或聲音）")]
    public float laneDepth = 0f;

    [HideInInspector]
    public bool enableSpawning = true;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        float camY = mainCam.transform.position.y;
        float laneY = transform.position.y;
        float viewHeightRange = 6f;

        // 如果車道高度在視野上下6單位內，允許生成
        enableSpawning = Mathf.Abs(camY - laneY) < viewHeightRange;
    }
}
