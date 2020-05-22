using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    public Transform firePos;
    public int numberPerVolley = 8;
    public float firingPause = 8;
    public float fireInterval = 0.2f;
    int volleyIndex;
    float lastShot;
    float reloadingTime;
    void Update()
    {
        Transform target = PlayerController.player.hitbox;
        if(Time.time - reloadingTime > firingPause && volleyIndex < numberPerVolley){
            Fire(target);
        }
        else{
            volleyIndex = 0;
        }
    }
    void Fire(Transform target){
        if(Time.time - lastShot > fireInterval){
            MissileController missle = SpawnManager.Spawn("MissileRoot", firePos.position).GetComponent<MissileController>();
            missle.SetTarget(target);
            lastShot = Time.time;
            volleyIndex++;
        }
        if(volleyIndex >= numberPerVolley){
            reloadingTime = Time.time;
        }
    }
}
