using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechController : MonoBehaviour
{
    public Transform headOfMech;
    public Transform firingPos1;
    public Transform firingPos2;
    public Vector3 firingOffset = new Vector3(0,2,15);
    public string enemyBulletPrefab;
    public float moveForce = 5;
    public float firingDistance = 200;
    public float fireInterval = 0.5f;
    public float reloadDuration = 2;
    int fireIndex = 1;
    float lastShot;
    float reloadTime;
    Rigidbody rb;
    HealthController health;
    void Awake(){
        rb = GetComponent<Rigidbody>();
        health = GetComponent<HealthController>();
    }
    void Start(){
        health.onDeath += Explode;
    }
    void Update(){
        Vector3 diff = PlayerControler.player.transform.position - transform.position;
        headOfMech.forward = Vector3.Dot(PlayerControler.player.transform.forward, transform.forward) < 0.3f ? diff.normalized : transform.forward;
        headOfMech.eulerAngles += new Vector3(-90,0,0);
        if(diff.magnitude < firingDistance && Vector3.Dot(diff.normalized, transform.forward) > 0){
            Fire(diff + firingOffset);
        }
    }

    void Fire(Vector3 dir){
        if(Time.time - lastShot > fireInterval && Time.time - reloadTime > reloadDuration){
            if(fireIndex > 3){
                fireIndex = 0;
                reloadTime = Time.time;
                return;
            }
            lastShot = Time.time;
            GameObject bullet1 = SpawnManager.Spawn(enemyBulletPrefab, firingPos1.position);
            bullet1.GetComponentInParent<BulletController>().SetDir(dir.normalized);
            GameObject bullet2 = SpawnManager.Spawn(enemyBulletPrefab, firingPos2.position);
            bullet2.GetComponentInParent<BulletController>().SetDir(dir.normalized);
            fireIndex++;
        }
    }
    void Explode(HealthController health){
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        gameObject.SetActive(false);
    }
    public void MoveForward(){
        rb.AddForce(transform.forward * moveForce, ForceMode.Impulse);
    }
}
