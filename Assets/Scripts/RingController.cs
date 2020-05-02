using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    public float visbilityDistance = 100;
    public MeshRenderer ring;
    bool triggered = false;
    void Update(){
        Vector3 dis = PlayerControler.player.transform.position - transform.position;
        if(dis.magnitude > visbilityDistance){
            ring.enabled = false;
        }
        else{
            ring.enabled = true;
        }
        if(PlayerControler.player.transform.position.z > transform.position.z + 10 && !triggered){
            GameManager.game.ResetScore();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider col){
        if(col.gameObject.GetComponentInParent<PlayerControler>()){
            ring.enabled = false;
            triggered = true;
        }
    }
}
