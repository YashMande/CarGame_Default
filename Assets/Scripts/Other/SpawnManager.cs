using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public static SpawnManager Instance;

    public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

  public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }
}
