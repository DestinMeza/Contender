using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    Animator anim;
    Transform target;
    float acceleration = 7;
    float maxSpeed = 50;
    float lifetime = 2;
    Rigidbody rb;
    HealthController health;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        health = GetComponent<HealthController>();
    }
    void OnEnable(){
        GetComponentInParent<HealthController>().onDeath += Explode;
        StartCoroutine(Lifetime());
    }
    void OnDisable(){
        StopCoroutine(Lifetime());
    }
    public void SetTarget(Transform target){
        this.target = target;
    }
    void Update()
    {
        Vector3 diff = target.position - transform.position;
        transform.forward = rb.velocity;
        rb.AddForce(diff * acceleration, ForceMode.Acceleration);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, maxSpeed *-1, maxSpeed),
            Mathf.Clamp(rb.velocity.y, maxSpeed *-1, maxSpeed),
            Mathf.Clamp(rb.velocity.z, maxSpeed *-1, maxSpeed)
        );
    }
    IEnumerator Lifetime(){
        yield return new WaitForSeconds(lifetime);
        Explode(health);
    }

    void Explode(HealthController health){
        gameObject.SetActive(false);
    }
}
