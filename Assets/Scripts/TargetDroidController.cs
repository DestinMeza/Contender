using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDroidController : MonoBehaviour
{
    public delegate void OnDeathCalculation(Vector3 position);
    public static event OnDeathCalculation onDeathCalculation = delegate {};
    HealthController health;
    public string deathParticles = "ExplosionSmallObject";
    void Awake(){
        health = GetComponent<HealthController>();
    }
    void OnEnable()
    {
        health.onDeath += Explode;
    }

    void OnDisable(){
        health.onDeath -= Explode;
    }
    void Explode(HealthController health){
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        onDeathCalculation(transform.position);
        gameObject.SetActive(false);
    }
}
