using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerTrigger triggerArea;
    Animator anim;
    void Awake(){
        anim = GetComponent<Animator>();
    }
    void OnEnable(){
        triggerArea.onTriggered += BeginFall;
    }
    void OnDisable(){
        triggerArea.onTriggered -= BeginFall;
    }
    void Fell(){
        anim.ResetTrigger("Fall");
    }

    void BeginFall(Collider col){
        anim.SetTrigger("Fall");
    }

}
