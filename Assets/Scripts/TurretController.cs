using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform turretFirePos;
    public Transform turret;
    public string enemyBulletPrefab;
    public float firingDis = 140;
    public float firingOffsetZ = 11;
    public float firingOffsetY = 2;
    public float fireInterval = 1;
    float lastShot;
    void Start(){
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Explode;
        lastShot = Time.time;
    }
    void Update()
    {
        Vector3 playerPos = PlayerControler.player.transform.position;
        playerPos.z += firingOffsetZ;
        playerPos.y -= firingOffsetY;
        Vector3 diff = playerPos - turret.transform.position;
        turret.forward = diff.normalized;
        if(diff.magnitude < firingDis && Vector3.Dot(PlayerControler.player.transform.forward, turret.transform.forward) < 0.3f){
            Fire(diff);
        }
    }

    void Explode(HealthController health){
        gameObject.SetActive(false);
    }

    void Fire(Vector3 dir){
        if(Time.time - lastShot > fireInterval){
            lastShot = Time.time;
            GameObject bullet = SpawnManager.Spawn(enemyBulletPrefab, turretFirePos.position);
            bullet.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet.GetComponentInParent<BulletController>().SetDir(turretFirePos.forward);
        }
    }
}
