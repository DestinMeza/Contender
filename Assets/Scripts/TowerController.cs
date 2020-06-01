using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerTrigger triggerArea;
    public Transform tower;
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
        AudioManager.Play("LargeObjectExplosion",1,1,false,transform.position,0.7f);
        ParticleManager.particleMan.Play("Dust", tower.position);
        anim.ResetTrigger("Fall");
    }

    void BeginFall(Collider col){
        anim.SetTrigger("Fall");
    }

}
