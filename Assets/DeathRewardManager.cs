using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRewardManager : MonoBehaviour
{
    public string[] pickupNames;
    void OnEnable(){
        GetComponent<HealthController>().onDeath += SpawnPickup;
    }

    void SpawnPickup(HealthController health){
        string spawnName = pickupNames[Random.Range(0, pickupNames.Length)];
        SpawnManager.Spawn(spawnName, transform.position);
    }
}
