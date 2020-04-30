using System.Collections;
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
    }
    void Hit(GameObject g){
        HealthController h = g.GetComponentInParent<HealthController>();
        Animator animator = g.GetComponentInParent<Animator>();
        if(h == null) return;
        if(animator != null) animator.Play("Hit");
        h.TakeDamage(damage);    
    }
}
