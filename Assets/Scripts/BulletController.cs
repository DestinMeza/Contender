using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 250;
    public float bulletLifetime = 5;
    public float startTime = 0;
    Rigidbody rb;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        rb.velocity = Vector3.forward * speed;
    }

    void Update(){
        if(Time.time - startTime > bulletLifetime){
            gameObject.SetActive(false);
        }
    }
}
