using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBulletController : MonoBehaviour
{
    public string nameOfParticle;
    public float speed = 100;
    public float bulletLifetime = 5;
    public float lifeTime = 5;
    public float minimalTrackDist = 2.0f;
    bool exploding = false;
    Transform enemyPos;
    Rigidbody rb;
    Animator anim;
    void Awake(){
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInParent<Animator>();
    }

    void OnEnable(){
        exploding = false;
        anim.Play("ChargedBullet");
        StartCoroutine(LifeTime());
    }
    public void SetDir(Transform enemy, float lifeTime)
    {
        if(enemy == null){
            enemyPos = null;
            transform.forward = PlayerControler.player.firePosMain.forward;
            rb.velocity = transform.forward * speed/2;
        }
        else{
            this.lifeTime = lifeTime;
            enemyPos = enemy;
            transform.forward = PlayerControler.player.firePosMain.forward;
            rb.velocity = transform.forward * speed/2;
        }
        
    }
    public void Update(){
        
        if(exploding){
            rb.velocity = Vector3.zero;
            return;
        } 
        if(enemyPos == null){
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }
        else{
            Vector3 dir = enemyPos.position - transform.position;
            rb.AddForce(dir.normalized * speed, ForceMode.Impulse);
            if(dir.magnitude < minimalTrackDist){
                StopCoroutine(LifeTime());
                rb.velocity = Vector3.zero;
                exploding = true;
                anim.Play("ChargedExplosion");
                ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            }
        }
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
        );
        
    }
    public void SetDir(Transform enemy)
    {
        SetDir(enemy, lifeTime);
    }
    
    IEnumerator LifeTime(){
        yield return new WaitForSeconds(lifeTime);
        exploding = true;
        anim.Play("ChargedExplosion");
        ParticleManager.particleMan.Play(nameOfParticle, transform.position);
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Solid"){
            StopCoroutine(LifeTime());
            rb.velocity = Vector3.zero;
            exploding = true;
            anim.Play("ChargedExplosion");
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
        }
    }
    void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Solid"){
            StopCoroutine(LifeTime());
            rb.velocity = Vector3.zero;
            exploding = true;
            anim.Play("ChargedExplosion");
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
        }
    }
    public void Deactivate(){
        enemyPos = null;
        gameObject.SetActive(false);
    }
}
