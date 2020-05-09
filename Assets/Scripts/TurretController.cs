using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform turretFirePos;
    public Transform turret;
    public string enemyBulletPrefab;
    public float firingDis = 140;
    public Vector3 firingOffset = new Vector3(0,2,11);
    public float fireInterval = 1;
    public string deathParticles = "ExplosionSmallObject";

    float lastShot;
    HealthController health;
    
    void Awake(){
        health = GetComponentInParent<HealthController>();
    }
    void OnEnable(){
        health.onDeath += Explode;
        lastShot = Time.time;
    }
    void OnDisable(){
        health.onDeath += Explode;
    }
    void Update()
    {
        TrackPlayer();
    }

    void TrackPlayer(){
        Vector3 playerPos = PlayerControler.player.transform.position;
        Vector3 diff = playerPos - turret.transform.position;
        turret.forward = diff.normalized;
        if(diff.magnitude < firingDis && Vector3.Dot(PlayerControler.player.transform.forward, turret.transform.forward) < 0.3f){
            Fire(diff + firingOffset);
        }
    }

    void Explode(HealthController health){
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        gameObject.SetActive(false);
    }

    void Fire(Vector3 dir){
        if(Time.time - lastShot > fireInterval){
            lastShot = Time.time;
            GameObject bullet = SpawnManager.Spawn(enemyBulletPrefab, turretFirePos.position);
            bullet.GetComponentInParent<BulletController>().SetDir(dir);
        }
    }
}
