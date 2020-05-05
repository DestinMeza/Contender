using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechController : MonoBehaviour
{
    public float moveForce = 5; 
    Rigidbody rb;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    public void MoveForward(){
        rb.AddForce(transform.forward * moveForce, ForceMode.Impulse);
    }
}
