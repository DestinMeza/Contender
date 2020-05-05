using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public GameObject playerUi;
    FlyingModes flyingModes;

    void Start(){
        TransitionController.onTransition += TransitionLock;
    }
    void Update(){
        if(flyingModes == FlyingModes.TransitionLock){
            playerUi.SetActive(false);
        }
        else{
            playerUi.SetActive(true);
        }
    }
    void TransitionLock(FlyingModes transition){
        flyingModes = transition;
    }
}
