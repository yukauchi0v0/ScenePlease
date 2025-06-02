using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;               // 拖入你那一隻魚的 Prefab
    public FishPathGenerator pathGenerator;     // 指定路徑
    public int fishCount = 10;                  // 魚的數量
    public float spawnRadius = 3f;              // 在路徑附近生成的範圍

    void Start()
    {
        // 取得魚群中心位置（如果 dynamicCenter 存在，就用它的位置，否則用 Vector3.zero）
        Vector3 centerPos = pathGenerator.dynamicCenter != null ? pathGenerator.dynamicCenter.position : Vector3.zero;

        for (int i = 0; i < fishCount; i++)
        {
            Vector3 spawnPos = centerPos + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = centerPos.y; // 保持在一樣的高度（可微調）

            GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
            
            FishFollower follower = fish.GetComponent<FishFollower>();
            if (follower == null) follower = fish.AddComponent<FishFollower>();
            follower.pathGenerator = pathGenerator;
        }
    }
}
