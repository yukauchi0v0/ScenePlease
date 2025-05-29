using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("左到右（車頭朝右）車輛 Prefab")]
    public List<GameObject> carPrefabsLeftToRight;

    [Header("右到左（車頭朝左）車輛 Prefab")]
    public List<GameObject> carPrefabsRightToLeft;

    [Header("是否使用隨機車速")]
    public bool useRandomSpeed = true;

    [Header("固定速度 / 最小速度（隨機模式）")]
    public float minSpeed = 2.5f;

    [Header("最大速度（隨機模式）")]
    public float maxSpeed = 4.5f;

    public List<CarLane> allLanes = new List<CarLane>();
    private Camera mainCam;

    private PlayerControls controls; // ← 新增這個

    void Awake()
    {
        controls = new PlayerControls();

        // 當 SpawnCar 被按下時執行
        controls.Player.SpawnCar.performed += ctx =>
        {
            TrySpawnCar();
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        mainCam = Camera.main;

        if (allLanes.Count == 0)
        {
            allLanes.AddRange(FindObjectsOfType<CarLane>());
        }
    }

    void Update()
    {
        // 保留原本 Space 鍵
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TrySpawnCar();
        }
    }

    void TrySpawnCar()
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

        List<GameObject> prefabList = lane.isRightToLeft ? carPrefabsRightToLeft : carPrefabsLeftToRight;
        if (prefabList.Count == 0) return;

        GameObject selectedPrefab = prefabList[Random.Range(0, prefabList.Count)];
        GameObject car = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        float carSpeed = useRandomSpeed ? Random.Range(minSpeed, maxSpeed) : minSpeed;
        car.GetComponent<CarController>().Init(endPos, carSpeed);
    }
}
