﻿using System.Collections;
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
    public float glidingDuration;
    public float rotationalDamp = 1;
    public float speed = 60;
    float strafingDist = 600;
    public Transform head;
    float lastGlideTime = 3;
    Rigidbody rb;

    void Awake(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        if(bossState == BossState.Flying)Flying();
        if(bossState == BossState.Turning)Turning();
        if(bossState == BossState.Gliding)Gliding();
        ClampPos();
    }

    void Gliding(){
        if(Time.time - lastGlideTime > glidingDuration){
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
            rb.velocity = rb.velocity = new Vector3(
                Mathf.Clamp(rb.velocity.x, speed*-2, speed*2),
                Mathf.Clamp(rb.velocity.y, speed*-2, speed*2),
                Mathf.Clamp(rb.velocity.z, speed*-2, speed*2)
            );
        }
        else{
            bossState = BossState.Flying;
        }
    }

    void Turning(){
        Vector3 diff = PlayerController.player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.z = 0;
        rotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        anim.SetFloat("horizontalTurn",transform.localRotation.y / rotation.y);
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
        rotation.x = 0;
        anim.SetFloat("horizontalTurn", rotation.y);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        anim.SetFloat("horizontalTurn",transform.localRotation.y / rotation.y);
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        rb.velocity = rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
            Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
        );

        Vector3 headDiff = PlayerController.player.transform.position - transform.position;
        anim.SetInteger("AnimState", (int)bossState);
        if(Vector3.Dot(transform.forward, PlayerController.player.transform.forward) < 0.1f){
            head.up = headDiff.normalized;
        }
        else if(diff.magnitude < strafingDist){
            bossState = BossState.Gliding;
            anim.SetInteger("AnimState", (int)bossState);
            lastGlideTime = Time.time;
        }
        else{
            bossState = BossState.Turning;
        }
    }

    void ClampPos(){
        transform.position = new Vector3 (transform.position.x, Mathf.Clamp(transform.position.y, 200, 300), transform.position.z);
    }
}
