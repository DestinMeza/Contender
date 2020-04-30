using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    public float visbilityDistance = 200;
    public MeshRenderer lazer;

    void Update(){
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude > visbilityDistance){
            lazer.enabled = false;
        }
        else{
            lazer.enabled = true;
        }
    }
}
