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
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude < visbilityDistance){
            anim.SetTrigger("Activate");
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
        anim.Play("LazerFlash");
    }
    void Explode(HealthController health){
        if(exploding) return;
        exploding = true;
        foreach(Transform t in explosionPositions){
            ParticleManager.particleMan.Play(particleName, t.position);
            anim.Play("LazerExplode");
        }
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.GetComponentInParent<PlayerControler>()) Explode(health);
    }
}
