using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBulletController : MonoBehaviour
{
    public string nameOfParticle;
    public float speed = 60;
    public float bulletLifetime = 5;
    public float lifeTime = 5;
    Transform enemyPos;
    Rigidbody rb;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        StartCoroutine(LifeTime());
    }
    public void SetDir(Transform enemy, float lifeTime)
    {
        if(enemy == null){
            transform.forward = PlayerControler.player.transform.forward;
        }
        else{
            this.lifeTime = lifeTime;
            transform.forward = enemy.position;
            enemyPos = enemy;
            rb.velocity = Vector3.zero;
        }
        
    }
    public void Update(){
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, 0, 60),
            Mathf.Clamp(rb.velocity.y, 0, 60),
            Mathf.Clamp(rb.velocity.z, 0, 60)
        );
        if(enemyPos == null){
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }
        else{
            rb.AddForce((enemyPos.position - rb.velocity).normalized * speed, ForceMode.VelocityChange);
        }
    }
    public void SetDir(Transform enemy)
    {
        SetDir(enemy, lifeTime);
    }
    
    IEnumerator LifeTime(){
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Solid"){
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            enemyPos = null;
            gameObject.SetActive(false);
        }
    }
}
