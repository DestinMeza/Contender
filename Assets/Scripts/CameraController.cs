using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float smoothTime = 0.5f;
    public float panDuration = 2;
    public bool playerCrashing;
    public bool looping;
    public bool alreadyPanning;
    public Transform target;
    public Transform heading;
    public Vector2 maxY = new Vector2(0, 58);
    public Vector2 maxX = new Vector2(-40, 40);
    public Vector3 railOffset = new Vector3(0, 1, -15);
    public Vector3 allRangeOffset = new Vector3(0, 1, -20);
    public Vector3 bossDeathOffset = new Vector3(-100, 0, 100);
    public Vector3[] panArray;
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
            if(!target.GetComponentInParent<PlayerController>()){
                if(target.GetComponent<BossMovementController>()){
                    Vector3 targetOrientation = target.GetComponent<BossMovementController>().head.position;
                    transform.LookAt(targetOrientation);
                }
            }
            if(looping){
                transform.LookAt(heading.position);
                Vector3 targetOrientation = heading.position + heading.right * railOffset.x + heading.up * railOffset.y + heading.forward * railOffset.z;
                transform.position = Vector3.SmoothDamp(
                        transform.position,
                        targetOrientation,
                        ref velocity,
                        smoothTime + 0.2f
                );
            } 
            else if(!playerCrashing || !looping){
                if(PlayerController.flyingModes == FlyingModes.Rail){
                    pos.y = Mathf.Clamp(transform.position.y, maxY.x, maxY.y);
                    pos.x = Mathf.Clamp(transform.position.x, maxX.x, maxX.y);
                    transform.position = Vector3.SmoothDamp(
                        pos,
                        targetPos + railOffset,
                        ref velocity,
                        smoothTime
                    );
                    transform.forward = PlayerController.player.transform.forward;
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
                    if(!alreadyPanning) StartCoroutine(CameraPan());   
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

    IEnumerator CameraPan(){
        alreadyPanning = true;
        for(int i = 0; i < panArray.Length; i++){
            for(float t = 0; t < panDuration; t += Time.maximumParticleDeltaTime){
                if(i == panArray.Length - 1 || PlayerController.flyingModes != FlyingModes.TransitionLock) goto End;
                if(Time.timeScale == 0){
                    while(Time.timeScale == 0) yield return null;
                }
                Vector3 offset = Vector3.Slerp(panArray[i], panArray[i+1], t/panDuration);
                transform.position = target.transform.position + offset;
                transform.LookAt(heading);
                yield return null;
            }
        }
        End:
        alreadyPanning = false;
    }

    void Crash(PlayerController player){
        playerCrashing = player.crash;
    }

    void DoingALoop(bool looping){
        this.looping = looping;
    }
}
