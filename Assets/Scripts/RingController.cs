using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    public float visbilityDistance = 100;
    public MeshRenderer ring;

    void Update(){
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude > visbilityDistance){
            ring.enabled = false;
        }
        else{
            ring.enabled = true;
        }
    }

    void OnTriggerExit(Collider col){
        if(col.gameObject.GetComponentInParent<PlayerControler>()){
            gameObject.SetActive(false);
        }
    }
}
