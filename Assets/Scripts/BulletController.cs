using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string nameOfParticle;
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
        if(col.gameObject.tag == "Solid"){
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            gameObject.SetActive(false);
        }
    }
}
