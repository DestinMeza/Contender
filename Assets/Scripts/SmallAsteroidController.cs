using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAsteroidController : MonoBehaviour
{
    HealthController health;
    Rigidbody rb;
    void Awake(){
        health = GetComponent<HealthController>();
        rb = GetComponentInParent<Rigidbody>();
    }
    void OnEnable()
    {
        health.onDeath += Explode;
    }
    void Explode(HealthController health){
        gameObject.SetActive(false);
    }

    void OnTriggerExit(Collider col){
        if(col.name == "AllRangeModeBounds"){
            rb.velocity *= -1;
            transform.forward *= -1;
        }
    }
}
