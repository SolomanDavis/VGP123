using System;
using UnityEditor;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefabs;
    [SerializeField] Transform[] itemSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        int noSpawnIndex = itemPrefabs.Length;

        for (int i = 0; i < itemSpawnPoints.Length; ++i)
        {
            // Randomly select prefab to spawn
            int prefabIndex = UnityEngine.Random.Range(0, itemPrefabs.Length + 1);

            if (prefabIndex == noSpawnIndex) continue;

            GameObject item = itemPrefabs[prefabIndex];

            // Instantiate randomly selected prefab at spawn point
            GameObject go = Instantiate(item, itemSpawnPoints[i].position, Quaternion.identity);

            Debug.Log("ZA - spawned item [" + go.name + "] at (" + itemSpawnPoints[i].position + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
