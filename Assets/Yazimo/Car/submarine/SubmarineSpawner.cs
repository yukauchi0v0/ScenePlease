using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SubmarineSpawner : MonoBehaviour
{
    [Header("左到右（潛艇頭朝右）Prefab")]
    public List<GameObject> subsLeftToRight;

    [Header("右到左（潛艇頭朝左）Prefab")]
    public List<GameObject> subsRightToLeft;

    [Header("是否使用隨機速度")]
    public bool useRandomSpeed = true;

    [Header("固定速度 / 最小速度（隨機模式）")]
    public float minSpeed = 1.5f;

    [Header("最大速度（隨機模式）")]
    public float maxSpeed = 3.0f;

    public List<CarLane> allLanes = new List<CarLane>();
    private Camera mainCam;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SpawnCar.performed += ctx => TrySpawnSubmarine();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        mainCam = Camera.main;

        if (allLanes.Count == 0)
        {
            foreach (var lane in FindObjectsOfType<CarLane>())
            {
                if (lane.CompareTag("SubmarineLane"))
                {
                    allLanes.Add(lane);
                }
            }
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TrySpawnSubmarine();
        }
    }

    void TrySpawnSubmarine()
    {
        var visibleLanes = allLanes.FindAll(lane => lane.enableSpawning);
        if (visibleLanes.Count == 0) return;

        CarLane lane = visibleLanes[Random.Range(0, visibleLanes.Count)];

        float laneY = lane.transform.position.y;
        float laneZ = lane.transform.position.z;
        float depth = laneZ - mainCam.transform.position.z;

        float spawnX, endX;
        float overSpawnMargin = 0.15f;
        float extraExitMargin = 1.5f;

        if (lane.isRightToLeft)
        {
            spawnX = mainCam.ViewportToWorldPoint(new Vector3(1f + overSpawnMargin, 0.5f, depth)).x;
            endX = mainCam.ViewportToWorldPoint(new Vector3(-extraExitMargin, 0.5f, depth)).x;
        }
        else
        {
            spawnX = mainCam.ViewportToWorldPoint(new Vector3(0f - overSpawnMargin, 0.5f, depth)).x;
            endX = mainCam.ViewportToWorldPoint(new Vector3(1f + extraExitMargin, 0.5f, depth)).x;
        }

        Vector3 spawnPos = new Vector3(spawnX, laneY, laneZ);
        Vector3 endPos = new Vector3(endX, laneY, laneZ);

        List<GameObject> prefabList = lane.isRightToLeft ? subsRightToLeft : subsLeftToRight;
        if (prefabList.Count == 0) return;

        GameObject sub = Instantiate(prefabList[Random.Range(0, prefabList.Count)], spawnPos, Quaternion.identity);
        float speed = useRandomSpeed ? Random.Range(minSpeed, maxSpeed) : minSpeed;
        sub.GetComponent<SubmarineController>().Init(endPos, speed);
    }
}
