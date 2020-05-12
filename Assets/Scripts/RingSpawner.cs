using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    public string prefabName = "RingRoot";
    public float spawnOffset = 75;
    public static RingSpawner ringSpawner;

    public void Awake(){
        if(ringSpawner == null){
            ringSpawner = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void SpawnRing(){
        Vector3 spawnLoc = new Vector3(Random.Range(-25,25),Random.Range(11,20), PlayerController.player.transform.position.z + spawnOffset);
        SpawnManager.Spawn(prefabName, spawnLoc);
    }
}
