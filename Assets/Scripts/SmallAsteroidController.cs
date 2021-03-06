﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAsteroidController : MonoBehaviour
{
    public string deathParticles = "ExplosionSmallObject";
    HealthController health;
    Rigidbody rb;
    void Awake(){
        health = GetComponent<HealthController>();
        rb = GetComponentInParent<Rigidbody>();
    }
    void OnEnable()
    {
        health.onDeath += Explode;
    }
    void Explode(HealthController health){
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.7f);
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        gameObject.SetActive(false);
    }

    void OnTriggerExit(Collider col){
        if(col.name == "AllRangeModeBounds"){
            rb.velocity *= -1;
            transform.forward *= -1;
        }
    }
}
