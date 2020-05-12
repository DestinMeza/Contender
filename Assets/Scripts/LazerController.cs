using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    public Transform[] explosionPositions;
    public string particleName = "ExplosionLazerPart";
    public float visbilityDistance = 200;
    HealthController health;
    Animator anim;
    bool exploding = false;
    bool active = false;
    void Awake(){
        health = GetComponent<HealthController>();
        anim = GetComponent<Animator>();
    }
    void OnEnable(){
        health.onHealthDecrease += FlashLazer;
        health.onDeath += Explode;
    }

    void OnDisable(){
        health.onHealthDecrease = FlashLazer;
        health.onDeath -= Explode;
    }

    void Update(){
        Vector3 dis = PlayerController.player.transform.position - transform.position;
        if(dis.magnitude < visbilityDistance){
            anim.SetTrigger("Activate");
            active = true;
        }
        else{
            anim.ResetTrigger("Activate");
        }
    }

    public void TurnOffLazer(){
        gameObject.SetActive(false);
    }

    void FlashLazer(){
        if(exploding) return;
        if(!active) return;
        AudioManager.Play("BlasterHit",1,1,false,transform.position,0.7f);
        anim.Play("LazerFlash");
    }
    void Explode(HealthController health){
        if(exploding) return;
        active = false;
        exploding = true;
        AudioManager.Play("LargeObjectExplosion",1,1,false,transform.position,0.7f);
        foreach(Transform t in explosionPositions){
            ParticleManager.particleMan.Play(particleName, t.position);
            anim.Play("LazerExplode");
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.GetComponentInParent<PlayerController>()) Explode(health);
    }
}
