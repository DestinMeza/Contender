using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    enum BossState{
        Flying,
        Gliding, 
        Turning
    }
    BossState bossState = BossState.Flying;
    Animator anim;
    public LayerMask obsticles;
    public float rotationalDamp = 1;
    public float speed = 60;
    public Transform head;
    Rigidbody rb;

    void Awake(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        
        if(bossState == BossState.Flying)Flying();
        if(bossState == BossState.Turning)Turning();
        if(bossState == BossState.Gliding)Gliding();
        CheckObsticles();
        ClampPos();
    }

    void Gliding(){

    }

    void Turning(){
        Vector3 diff = PlayerController.player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.z = 0;
        rotation.x = Mathf.Clamp(rotation.x, -5, 5);
        anim.SetFloat("horizontalTurn", rotation.x);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        rb.velocity = rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-0.5f, speed*0.5f),
            Mathf.Clamp(rb.velocity.y, speed*-0.5f, speed*0.5f),
            Mathf.Clamp(rb.velocity.z, speed*-0.5f, speed*0.5f)
        );

        anim.SetInteger("AnimState", (int)bossState);
        if(Vector3.Dot(transform.forward, PlayerController.player.transform.forward) > 0){
            bossState = BossState.Flying;
        }
    }

    void Flying(){
        Vector3 diff = PlayerController.player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.z = 0;
        rotation.x = Mathf.Clamp(rotation.x, -5, 5);
        anim.SetFloat("horizontalTurn", rotation.x);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        rb.velocity = rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
        );

        Vector3 headDiff = PlayerController.player.transform.position - transform.position;
        anim.SetInteger("AnimState", (int)bossState);
        if(Vector3.Dot(transform.forward, PlayerController.player.transform.forward) < 0.3){
            head.up = headDiff.normalized;
        }
        else{
            bossState = BossState.Turning;
        }
    }

    void CheckObsticles(){
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit , 1000, obsticles, QueryTriggerInteraction.Collide)){
            rb.AddForce(transform.up, ForceMode.VelocityChange);
        }
    }

    void ClampPos(){
        transform.position = new Vector3 (transform.position.x, Mathf.Clamp(transform.position.y, 20, 300), transform.position.z) ;
    }
}
