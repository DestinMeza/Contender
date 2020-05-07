using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    public float visbilityDistance = 200;
    public GameObject lazer;

    void Update(){
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude > visbilityDistance){
            lazer.SetActive(false);
        }
        else{
            lazer.SetActive(true);
        }
    }
}
