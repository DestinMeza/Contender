using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    public LayerMask obsticles;
    public float rotationalDamp = 1;
    public float speed = 60;
    public Transform head;
    Rigidbody rb;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        Vector3 diff = PlayerController.player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.z = 0;
        rotation.x = Mathf.Clamp(rotation.x, -5, 5);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        rb.velocity = rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
        );

        Vector3 headDiff = PlayerController.player.transform.position - transform.position;

        if(Vector3.Dot(transform.forward, PlayerController.player.transform.forward) < 0){
            head.up = headDiff.normalized;
        }
        else{
            head.up = transform.forward;
        }

        CheckObsticles();
    }

    void CheckObsticles(){
        Ray pos = new Ray(transform.position, Vector3.zero);
        RaycastHit hit;
        if(Physics.SphereCast(pos, 50, out hit, 50, obsticles, QueryTriggerInteraction.Collide)){
            if(hit.collider.gameObject.tag == "Solid"){
                Transform obsticle = hit.collider.GetComponentInParent<Transform>();
                Vector3 diff = obsticle.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(diff.normalized, transform.right.normalized));
                rotation.z = 0;
                rotation.x = Mathf.Clamp(rotation.x, -5, 5);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            }
        }
    }
}
