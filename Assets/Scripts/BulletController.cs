using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string nameOfParticle;
    public float speed = 250;
    public float bulletLifetime = 5;
    public float startTime = 0;
    Rigidbody rb;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    public void SetDir(Vector3 dir)
    {
        transform.forward = dir.normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(dir.normalized * speed, ForceMode.VelocityChange);
    }
    void Update(){
        if(Time.time - startTime > bulletLifetime){
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Solid"){
            ParticleManager.particleMan.Play(nameOfParticle, transform.position);
            gameObject.SetActive(false);
        }
    }
}
