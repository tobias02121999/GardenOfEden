using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string name;
        public GameObject prefab;
        public int poolSize;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionairy;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionairy = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionairy.Add(pool.name, objectPool);
        }
    }

    public GameObject SpawnFromPool(string poolName, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (!poolDictionairy.ContainsKey(poolName))
        {
            Debug.LogWarning("Pool with tag " + poolName + "doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionairy[poolName].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = spawnPosition;
        objectToSpawn.transform.rotation = spawnRotation;

        poolDictionairy[poolName].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
