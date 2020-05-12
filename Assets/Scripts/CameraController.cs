using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float smoothTime = 0.5f;
    public bool playerCrashing;
    public bool looping;
    public Transform target;
    public Transform heading;
    public Vector3 railOffset = new Vector3(0, 1, -15);
    public Vector3 allRangeOffset = new Vector3(0, 1, -20);
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
        PlayerController.onCrash += Crash;
        PlayerController.onLoop += DoingALoop;
    }
    void FixedUpdate(){
        if(target == null) return;

        if(Application.isPlaying){
            Vector3 targetPos = target.transform.position;
            Vector3 pos = transform.position;
            if(looping){
                transform.LookAt(targetPos);
                transform.position = heading.forward + allRangeOffset;
            } 
            else if(!playerCrashing){
                if(PlayerController.flyingModes == FlyingModes.Rail){
                    pos.y = Mathf.Clamp(transform.position.y, 0, 58);
                    pos.x = Mathf.Clamp(transform.position.x, -30, 30);
                    transform.position = Vector3.SmoothDamp(
                        pos,
                        targetPos + railOffset,
                        ref velocity,
                        smoothTime
                    );
                    transform.eulerAngles = transform.forward;
                }
                if(PlayerController.flyingModes == FlyingModes.AllRange){
                    Vector3 targetOrientation = target.position + target.right * allRangeOffset.x + target.up * allRangeOffset.y + target.forward * allRangeOffset.z;
                    transform.position = Vector3.SmoothDamp(
                        pos,
                        targetOrientation,
                        ref velocity,
                        smoothTime -0.2f
                    );
                    Quaternion look = Quaternion.LookRotation(target.position-transform.position, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, 0.5f);
                }
                if(PlayerController.flyingModes == FlyingModes.TransitionLock){
                    Vector3 camLocTran = targetPos;
                    camLocTran.z += -8;
                    camLocTran.x += 7;
                    camLocTran.y += 2;

                    transform.position = camLocTran;
                    transform.LookAt(PlayerController.player.transform.position);
                }
            }
            else{
                
                Vector3 camLocCrash = targetPos;
                camLocCrash.z += 40;
                camLocCrash.y += 6;
                transform.position = camLocCrash;
                transform.LookAt(PlayerController.player.transform.position);
            }
        }
    
        else{
            transform.position = target.position + railOffset;
        }
    }

    void Crash(PlayerController player){
        playerCrashing = player.crash;
    }

    void DoingALoop(bool looping){
        this.looping = looping;
    }
}
