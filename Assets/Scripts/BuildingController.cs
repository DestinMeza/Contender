using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    Rigidbody body;
    void OnCollisionEnter(Collision col){
        if(col.gameObject.GetComponentInParent<PlayerControler>()){ 
            body = col.gameObject.GetComponentInParent<Rigidbody>();
            Debug.Log("Hit");
            Vector3 diff = body.velocity - transform.position;
            float dot = Vector3.Dot(diff.normalized, Vector3.right.normalized);
            body.AddForce(diff * dot * 50, ForceMode.Impulse);  
        }
    }
}
