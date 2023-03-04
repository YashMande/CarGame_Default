using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab0;
    [SerializeField]
    private GameObject _prefab1;
    [SerializeField]
    private GameObject _prefab2;

    public Transform[] spawnPoints;
    int spawnNumber;
    GameManager gm;
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        Vector2 offset = Random.insideUnitCircle * 50f;
        Vector3 position = new Vector3(transform.position.x + offset.x, transform.position.y, transform.position.z);
        if (gm.playerSelected == 0)
        {
            MasterManager.NetworkInstantiate(_prefab0, spawnPoint.position, spawnPoint.rotation);
        }
        else if (gm.playerSelected == 1)
        {
            MasterManager.NetworkInstantiate(_prefab1, spawnPoint.position, spawnPoint.rotation);
        }
        else if (gm.playerSelected ==2)
        {
            MasterManager.NetworkInstantiate(_prefab2, spawnPoint.position, spawnPoint.rotation);
        }
     
      
    }
}
