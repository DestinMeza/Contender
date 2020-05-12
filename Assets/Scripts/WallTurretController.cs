using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTurretController : MonoBehaviour
{
    public float triggerDis = 250;
    Animator anim;
    void Awake(){
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 diff = PlayerController.player.transform.position - transform.position;
        if(diff.magnitude > triggerDis){
            anim.SetTrigger("playerIsFar");
            anim.ResetTrigger("playerIsNear");
        }
        else{
            anim.SetTrigger("playerIsNear");
            anim.ResetTrigger("playerIsFar");
        }
        
    }
}
