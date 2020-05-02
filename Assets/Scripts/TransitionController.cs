using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TransitionController : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        PlayerControler player = col.GetComponentInParent<PlayerControler>();
        if(player != null){
            player.TransitionLock();
        }
    }
}
