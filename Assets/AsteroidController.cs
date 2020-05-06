using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{    
    public string asteroidPrefabName;
    HealthController health;
    Rigidbody rb;
    void Awake(){
        health = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        health.onDeath += Explode;
    }
    void OnCollisionEnter(Collision col){
        if(col.gameObject.GetComponentInParent<AsteroidController>()){
            Rigidbody otherAsteroid = col.gameObject.GetComponentInParent<Rigidbody>();
            if(otherAsteroid.velocity.magnitude > rb.velocity.magnitude){
                rb.velocity = otherAsteroid.velocity.normalized - rb.velocity.normalized;
            }
        }
    }
    void Explode(HealthController health){
        GameObject smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        gameObject.SetActive(false);
    }
}
