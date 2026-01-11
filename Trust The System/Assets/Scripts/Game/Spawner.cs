using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs, spawnPoints;
    [SerializeField] private bool canSpawnObjects = true;
    [SerializeField] private bool endlessSpawn = false;
    [Min(0), SerializeField] private int objectLimit = 1;
    //[Min(0), SerializeField] private int pauseSpawnLimit = 1;
    [SerializeField] private bool distanceLimit = false;
    [Min(0), SerializeField] private float spawnDistance = 1;
    [Min(0), SerializeField] private float spawnInterval = 1;
    
    //Internal Variables
    private float spawnTimer;
    private int objectsSpawned;
    //private int objectsInScene = 0;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (canSpawnObjects)
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position - transform.position;

        bool isBelowObjectLimit = (objectsSpawned < objectLimit && !endlessSpawn) || endlessSpawn;
        bool isReadyToSpawn = spawnTimer >= spawnInterval && isBelowObjectLimit;
        bool isPlayerNear = playerPos.sqrMagnitude < spawnDistance * spawnDistance && distanceLimit;

        if (isReadyToSpawn && (isPlayerNear || !distanceLimit))
        {
            int spawn = Random.Range(0, spawnPoints.Length), obj = Random.Range(0, prefabs.Length);
            Instantiate(prefabs[obj], spawnPoints[spawn].transform.position, Quaternion.identity);
            objectsSpawned++;
            spawnTimer = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (distanceLimit)
        {
            Gizmos.DrawWireSphere(transform.position, spawnDistance);
        }
    }

    public float GetSpawnDistance()
    {
        return spawnDistance;
    }
}
