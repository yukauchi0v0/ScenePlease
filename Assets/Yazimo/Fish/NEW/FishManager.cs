using UnityEngine;
using System.Collections.Generic;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public int fishCount = 50;
    public Vector3 spawnAreaSize = new Vector3(30, 10, 30);
    public List<FishAgent> fishAgents = new List<FishAgent>();

    void Start()
    {
        for (int i = 0; i < fishCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnAreaSize.x, spawnAreaSize.x),
                Random.Range(-spawnAreaSize.y, spawnAreaSize.y),
                Random.Range(-spawnAreaSize.z, spawnAreaSize.z)
            );

            GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity, transform);
            fishAgents.Add(fish.GetComponent<FishAgent>());
            fish.GetComponent<FishAgent>().manager = this;
        }
    }
}
