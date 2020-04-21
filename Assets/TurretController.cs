using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform turretFirePos;
    public Transform turret;
    public GameObject enemyBulletPrefab;
    public int maxBullets = 5;
    public float fireInterval = 1;
    float lastShot;
    BulletController[] bullets;

    void Start(){
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
        Vector3 diff = PlayerControler.player.transform.position - turret.transform.position;
        turret.forward = diff;
        if(diff.magnitude < 50){
            Fire(diff);
        }
    }

    void Fire(Vector3 diff){
        if(Time.time - lastShot > fireInterval)
        for(int i = 0; i < bullets.Length; i++){
            if(!bullets[i].gameObject.activeSelf){
                bullets[i].gameObject.SetActive(true);
                bullets[i].transform.forward = diff;
                bullets[i].startTime = Time.time;
                bullets[i].gameObject.transform.position = turretFirePos.position;
                return;
            }
        }
    }
}
