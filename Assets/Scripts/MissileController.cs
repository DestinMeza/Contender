using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public LayerMask ground;
    Animator anim;
    Transform target;
    public float rotationalDamp = 1000;
    public float acceleration = 60;
    public float maxSpeed = 200;
    public float lifetime = 10;
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
            rb.AddForce(transform.up, ForceMode.Impulse);
        }

        Vector3 diff = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        rb.AddForce(transform.forward * acceleration, ForceMode.VelocityChange);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, maxSpeed *-1, maxSpeed*1),
            Mathf.Clamp(rb.velocity.y, maxSpeed *-1, maxSpeed*1),
            Mathf.Clamp(rb.velocity.z, maxSpeed *-1, maxSpeed*1)
        );
    }
    IEnumerator Lifetime(){
        yield return new WaitForSeconds(lifetime);
        Explode(health);
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.CompareTag("Solid")){
            Explode(health);
        }
    }

    void Explode(HealthController health){
        ParticleManager.particleMan.Play("ExplosionSmallObject", transform.position);
        gameObject.SetActive(false);
    }
}
