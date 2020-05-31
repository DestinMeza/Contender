using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurretController : MonoBehaviour
{
    public Transform firePos1;
    public Transform firePos2;
    public Transform turretHead;
    public Transform target;
    public float rotationalDamp = 1;
    public float maxFiringDis = 200;
    public float fireInterval = 2;
    public float launchDelay = 1;
    bool firedFirst;
    float lastShot;
    float secondLaunchDelay;
    HealthController health;

    void Awake(){
        health = GetComponent<HealthController>();
    }

    void OnEnable(){
        health.onDeath += Explode;
    }

    void OnDisable(){
        health.onDeath -= Explode;
    }

    void Update()
    {
        Vector3 diff = target.position - turretHead.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.x = 0;
        rotation.z = 0;
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, rotation, rotationalDamp * Time.deltaTime);
        if(diff.magnitude < maxFiringDis){
            Fire(target);
        }
    }

    void Fire(Transform target){
        if(Time.time - lastShot > fireInterval && !firedFirst){
            MissileController missle = SpawnManager.Spawn("MissileRoot", firePos1.position).GetComponent<MissileController>();
            missle.SetTarget(target);
            secondLaunchDelay = Time.time;
            firedFirst = true;
            AudioManager.Play("MissileLaunch", 1, 1, false, transform.position, 0.8f);
        }
        else if(Time.time - secondLaunchDelay > launchDelay && firedFirst){
            MissileController missle = SpawnManager.Spawn("MissileRoot", firePos2.position).GetComponent<MissileController>();
            missle.SetTarget(target);
            lastShot = Time.time;
            firedFirst = false;
            AudioManager.Play("MissileLaunch", 1, 1, false, transform.position, 0.8f);
        }
    }

    void Explode(HealthController health){
        ParticleManager.particleMan.Play("ExplosionSmallObject", transform.position);
        AudioManager.Play("SmallObjectExplosion", 1, 1, false, transform.position, 0.8f);
        gameObject.SetActive(false);
    }
}
