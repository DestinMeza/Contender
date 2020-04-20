using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public Vector3 speed = new Vector3(30,-20, 100);
    public float maxSpeedChange = 10;
    Vector3 targetVelocity;
    Rigidbody rb;
    Animator anim;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update(){

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool shift = Input.GetKey(KeyCode.LeftShift);
        if(shift) x *= 2;


        targetVelocity = new Vector3(x, y, 0).normalized + Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speed);
        
        anim.SetFloat("xVel", x);
        anim.SetFloat("yVel", -y);
    }

    void FixedUpdate(){

        Vector3 velocityChange = targetVelocity - rb.velocity;
        Vector3 xyChange = velocityChange;
        xyChange.z = 0;
        if(xyChange.sqrMagnitude > maxSpeedChange * maxSpeedChange){
            xyChange = xyChange.normalized * maxSpeedChange;
            velocityChange.x = xyChange.x;
            velocityChange.y = xyChange.y;
        }
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
