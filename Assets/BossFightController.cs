using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    public string bossName;
    bool spawned;
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerController>() && !spawned){
            SpawnManager.Spawn(bossName, transform.position);
            spawned = true;
        }
    }
}
