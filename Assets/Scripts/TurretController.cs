using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform turretFirePos;
    public Transform turret;
    public string enemyBulletPrefab;
    public float firingDis = 140;
    public float fireInterval = 1;
    public float rotationalDamp = 2;
    public string deathParticles = "ExplosionSmallObject";
    public float bulletSpeed = 150;
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

        Vector3 playerVel = PlayerController.player.rb.velocity;
        Vector3 playerPos = PlayerController.player.hitbox.position;
        Vector3 diff = playerPos - turretFirePos.position;
        
        float dist = diff.magnitude;
        float t = dist / (bulletSpeed - playerVel.magnitude);
        playerPos = playerPos + playerVel * t;
        diff = playerPos - turretFirePos.position;

        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.z = 0;
        rotation.x = 0;
        turret.rotation = Quaternion.Slerp(turret.rotation, rotation, rotationalDamp * Time.deltaTime);

        if(diff.magnitude < firingDis && Vector3.Dot(PlayerController.player.transform.forward, turret.transform.forward) < 0.3f){
            Fire(diff);
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
            AudioManager.Play("BlasterSound",1,1,false,transform.position,0.9f);
        }
    }
}
