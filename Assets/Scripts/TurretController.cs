using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform turretFirePos;
    public Transform turret;
    public GameObject enemyBulletPrefab;
    public float firingDis = 140;
    public float firingOffsetZ = 11;
    public float firingOffsetY = 2;
    public int maxBullets = 5;
    public float fireInterval = 1;
    float lastShot;
    BulletController[] bullets;

    void Start(){
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Explode;
        lastShot = Time.time;
        bullets = new BulletController[maxBullets];
        for(int i = 0; i < maxBullets; i++){
            GameObject bullet = Instantiate(enemyBulletPrefab);
            bullets[i] = bullet.GetComponent<BulletController>();
            bullet.gameObject.SetActive(false);
        }
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
            for(int i = 0; i < bullets.Length; i++){
                if(!bullets[i].gameObject.activeSelf){
                    bullets[i].gameObject.SetActive(true);
                    bullets[i].SetDir(dir);
                    bullets[i].transform.position = turretFirePos.transform.position;
                    bullets[i].startTime = Time.time;
                    lastShot = Time.time;
                    return;
                }
            }    
        }
    }
}
