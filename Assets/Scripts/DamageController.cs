﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    public string hitSound = "laserHit";
    public string hitParticles = "Explosion";
    int defaultDmg;
    public int damage = 1;

    void Awake(){
        defaultDmg = damage;
    }
    void OnEnable(){
        damage = defaultDmg;
    }
    void OnCollisionEnter(Collision col){
        Hit(col.gameObject);
    }
    void OnTriggerEnter(Collider col){
        Hit(col.gameObject);
        if(gameObject.name == "LazerOrientation") Debug.Log(col.name);
    }
    void Hit(GameObject g){
        HealthController h = g.GetComponentInParent<HealthController>();
        if(h == null) return;
        h.TakeDamage(damage);    
    }
}
