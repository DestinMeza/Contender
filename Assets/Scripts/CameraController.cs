﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public float smoothTime = 0.5f;
    public bool playerCrashing;
    public Transform target;
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
        PlayerControler.onCrash += Crash;
    }
    void FixedUpdate(){
        if(target == null) return;

        if(Application.isPlaying){
            Vector3 targetPos = target.transform.position;
            Vector3 pos = transform.position;
            if(!playerCrashing){
                if(PlayerControler.flyingModes == FlyingModes.Rail){
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
                if(PlayerControler.flyingModes == FlyingModes.AllRange){
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
                if(PlayerControler.flyingModes == FlyingModes.TransitionLock){
                    Vector3 camLocTran = targetPos;
                    camLocTran.z += -10;
                    camLocTran.x += 7;
                    camLocTran.y += 2;

                    transform.position = camLocTran;
                    transform.LookAt(PlayerControler.player.transform.position);
                }
            }
            else{
                
                Vector3 camLocCrash = targetPos;
                camLocCrash.z += 40;
                camLocCrash.y += 6;
                transform.position = camLocCrash;
                transform.LookAt(PlayerControler.player.transform.position);
            }
        }
    
        else{
            transform.position = target.position + railOffset;
        }
    }

    void Crash(PlayerControler player){
        playerCrashing = player.crash;
    }
}
