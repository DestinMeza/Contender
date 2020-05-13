using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public LayerMask ground;
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

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 10, ground)){
            rb.AddForce(transform.up * acceleration, ForceMode.VelocityChange);
        }
        Vector3 diff = target.position - transform.position;
        transform.forward = rb.velocity;
        rb.AddForce(diff * acceleration, ForceMode.Acceleration);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, maxSpeed *-1.5f, maxSpeed*1.5f),
            Mathf.Clamp(rb.velocity.y, maxSpeed *-1.5f, maxSpeed*1.5f),
            Mathf.Clamp(rb.velocity.z, maxSpeed *-1.5f, maxSpeed*1.5f)
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
