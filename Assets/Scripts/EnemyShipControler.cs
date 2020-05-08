using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipControler : MonoBehaviour
{
    float speed = 20;
    public LayerMask obsticles;
    Rigidbody rb;
    GameObject origin;
    bool leftTriggerArea = false;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    void Start(){
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Explode;
    }
    void Explode(HealthController health){
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        gameObject.SetActive(false);
    }
    void Update(){
        Move();
        CheckObsticles();
        if(leftTriggerArea){
            TurnTowardsOrigin();
        }
    }

    void Move(){
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
        );
    }

    void CheckObsticles(){
        Ray ray = new Ray(transform.position, rb.velocity.normalized);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100000, obsticles, QueryTriggerInteraction.Collide)){
            if(hit.collider.gameObject.tag == "Solid"){
                Transform obsticle = hit.collider.GetComponentInParent<Transform>();
                Vector3 diff = obsticle.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(diff, transform.right.normalized));
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            }
        }
    }

    void TurnTowardsOrigin(){
        Vector3 diff = origin.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }
    void OnTriggerExit(Collider col){
        if(col.name == "AllRangeModeBounds"){
            leftTriggerArea = true;
            origin = col.gameObject;
        }
    }
    void OnTriggerEnter(Collider col){
        if(col.name == "AllRangeModeBounds"){
            leftTriggerArea = false;
            origin = col.gameObject;
        }
    }
}
