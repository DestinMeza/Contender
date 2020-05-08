using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaserController : MonoBehaviour
{
    public float rotationalDamp = 0.5f;
    public float speed = 30;
    public float maxFollowDistance = 30;
    public Transform target;
    public FlyingModes flyingModes;
    Rigidbody rb;
    
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        TransitionController.onTransition += ModeCheck;
    }
    void OnDisable(){
        TransitionController.onTransition -= ModeCheck;
    }
    void Update()
    {
        if(flyingModes == FlyingModes.AllRange){
            Turn();
            MoveAllRange();
        }
        else if(flyingModes == FlyingModes.Rail){
            MoveRail();
        }
    }

    void Turn(){
        Vector3 diff = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
    }
    void MoveAllRange(){
        Vector3 diff = target.position - transform.position;
        Vector3 steeringDir = transform.forward;
        if(diff.magnitude < maxFollowDistance){
            rb.AddForce(steeringDir.normalized * speed/2 * -1, ForceMode.Acceleration);
        }
        else{
            rb.AddForce(steeringDir.normalized * speed, ForceMode.Acceleration);
            rb.velocity = new Vector3(
                Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
            );
        }
    }

    void MoveRail(){
        Vector3 diff = target.position - transform.position;
        transform.forward = diff.normalized;
        Vector3 steeringDir = transform.forward;
        if(diff.magnitude < maxFollowDistance){
            rb.AddForce(steeringDir.normalized * speed/2 * -1, ForceMode.VelocityChange);
        }
        else{
            rb.AddForce(steeringDir.normalized * speed, ForceMode.VelocityChange);
            rb.velocity = new Vector3(
                Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
            );
        }
    }

    void OnCollisionEnter(Collision col){
        rb.AddForce(transform.forward.normalized * -Vector3.Dot(transform.forward, col.transform.position), ForceMode.Impulse);
    }
    void ModeCheck(FlyingModes currentState){
        flyingModes = currentState;
    }
}
