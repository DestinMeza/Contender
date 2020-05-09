using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDroidController : MonoBehaviour
{
    // Start is called before the first frame update
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
        gameObject.SetActive(false);
    }
}
