using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float speed = 100;
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -20);

    void FixedUpdate()
    {
        if(target == null) return;

        if(Application.isPlaying){
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.fixedDeltaTime * speed);
        }
        

        else{
            transform.position = target.position + offset;
        }
    }
}
