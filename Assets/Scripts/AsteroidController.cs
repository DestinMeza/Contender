using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{    
    public string asteroidPrefabName;
    public string deathParticles = "ExplosionLargeObject";
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
                rb.AddForce((otherAsteroid.velocity - rb.velocity).normalized, ForceMode.Force);
            }
        }
    }
    void OnTriggerExit(Collider col){
        if(col.name == "AllRangeModeBounds"){
            rb.velocity *= -1/2;
            transform.forward *= -1/2;
        }
    }
    void Explode(HealthController health){
        GameObject smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        smallAsteroid = SpawnManager.Spawn(asteroidPrefabName, transform.position + Random.insideUnitSphere);
        smallAsteroid.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere;
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        gameObject.SetActive(false);
    }
}
