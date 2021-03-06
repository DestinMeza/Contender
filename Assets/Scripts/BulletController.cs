﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string nameOfParticle = "BlasterSparks";
    public float speed = 250;
    public float bulletLifetime = 5;
    public float lifeTime = 5;
    Rigidbody rb;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        StartCoroutine(LifeTime());
    }
    public void SetDir(Vector3 dir, float lifeTime)
    {
        transform.forward = dir.normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(dir.normalized * speed, ForceMode.VelocityChange);
    }
    public void SetDir(Vector3 dir)
    {
        SetDir(dir, lifeTime);
    }
    
    IEnumerator LifeTime(){
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Solid")){
            AudioManager.Play("BlasterHit",1,1,false,transform.position,0.9f);
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision col){
        if(col.gameObject.CompareTag("Solid")){
            AudioManager.Play("ObjectHit",1,1,false,transform.position,0.9f);
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            gameObject.SetActive(false);
        }
    }
}
