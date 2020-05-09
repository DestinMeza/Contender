using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    public Transform[] explosionPositions;
    public string particleName = "ExplosionLargeObject";
    public float visbilityDistance = 200;
    public GameObject lazer;
    HealthController health;

    void Awake(){
        health = GetComponent<HealthController>();
    }
    void OnEnable(){
        health.onHealthChange += FlashLazer;
        health.onDeath += Explode;
    }

    void OnDisable(){
        health.onHealthChange -= FlashLazer;
        health.onDeath -= Explode;
    }

    void Update(){
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude > visbilityDistance){
            lazer.SetActive(false);
        }
        else{
            lazer.SetActive(true);
        }
    }

    void FlashLazer(){
        StopCoroutine(LazerFlash());
        StartCoroutine(LazerFlash());
    }
    IEnumerator LazerFlash(){
        lazer.SetActive(false);
        yield return new WaitForSeconds(1);
        lazer.SetActive(true);
    }
    void Explode(HealthController health){
        health.TakeDamage(health.health);
        foreach(Transform t in explosionPositions){
            ParticleManager.particleMan.Play(particleName, t.position);
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision col){
        Explode(health);
    }
}
