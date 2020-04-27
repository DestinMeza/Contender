using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float speed = 100;
    public float smoothTime = 0.5f;
    public bool playerCrashing;
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -5);
    Vector3 velocity = Vector3.zero;
    public static CameraController cameraMain;
    Camera cam;
    void Awake(){
        if(cameraMain == null){
            cameraMain = this;
            cam = Camera.main;
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start(){
        PlayerControler.onCrash += Crash;
    }
    void FixedUpdate(){
        if(target == null) return;

        if(Application.isPlaying){
            Vector3 targetPos = target.transform.position;
            Vector3 pos = transform.position;
            if(!playerCrashing){
                transform.position = Vector3.SmoothDamp(
                    pos,
                    new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, targetPos.z + offset.z),
                    ref velocity,
                    smoothTime
                );
                transform.eulerAngles = Vector3.forward;
            }
            else{
                Vector3 camLoc2 = transform.position;
                camLoc2 = targetPos;
                camLoc2.z += 20;
                transform.position = camLoc2;
                transform.LookAt(PlayerControler.player.transform.position);
            }
        }
    
        else{
            transform.position = target.position + offset;
        }
    }

    void Crash(PlayerControler player){
        playerCrashing = player.crash;
    }
}
