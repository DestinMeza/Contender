using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBulletController : MonoBehaviour
{
    public string nameOfParticle;
    public float speed = 100;
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
            rb.velocity = Vector3.zero;
        }
        else{
            this.lifeTime = lifeTime;
            transform.forward = PlayerControler.player.transform.forward;
            enemyPos = enemy;
            rb.velocity = Vector3.zero;
        }
        
    }
    public void Update(){
        
        if(enemyPos == null){
            rb.AddForce(transform.forward * speed, ForceMode.Force);
        }
        else{
            Vector3 dir = enemyPos.position - transform.position;
            rb.AddForce(dir.normalized * speed, ForceMode.Force);
        }
        if(enemyPos != null && Vector3.Dot(enemyPos.forward, transform.forward) > 0.7 ){
            rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-2, speed*2),
            Mathf.Clamp(rb.velocity.y, speed*-2, speed*2),
            Mathf.Clamp(rb.velocity.z, speed*-2, speed*2)
            );
        }
        else{
            rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1, speed),
            Mathf.Clamp(rb.velocity.y, speed*-1, speed),
            Mathf.Clamp(rb.velocity.z, speed*-1, speed)
            );
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
