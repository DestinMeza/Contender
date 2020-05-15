using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    public string bossName;
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerController>()){
            SpawnManager.Spawn(bossName, transform.position);
        }
    }
}
