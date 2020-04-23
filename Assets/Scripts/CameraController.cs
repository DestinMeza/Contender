using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float speed = 100;
    public float smoothTime = 0.5f;
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -20);
    Vector3 velocity = Vector3.zero;

    void FixedUpdate(){
        if(target == null) return;

        if(Application.isPlaying){
            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position;
            transform.position = Vector3.SmoothDamp(
                pos,
                new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, targetPos.z + offset.z),
                ref velocity,
                smoothTime
                );
        }
    
        else{
            transform.position = target.position + offset;
        }
    }
}
