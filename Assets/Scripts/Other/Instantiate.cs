using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    public Transform[] spawnPoints;
    int spawnNumber;

    private void Awake()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        Vector2 offset = Random.insideUnitCircle * 50f;
        Vector3 position = new Vector3(transform.position.x + offset.x, transform.position.y, transform.position.z);
        MasterManager.NetworkInstantiate(_prefab, spawnPoint.position, spawnPoint.rotation);
      
    }
}
