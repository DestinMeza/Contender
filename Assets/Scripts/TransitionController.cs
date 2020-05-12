using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TransitionController : MonoBehaviour
{
    public delegate void OnTransition(FlyingModes transition);
    public static event OnTransition onTransition = delegate {};

    public FlyingModes transitionState = FlyingModes.Rail;
    FlyingModes flyingModes = FlyingModes.Rail;
    void Start(){
        flyingModes = FlyingModes.Rail;
    }
    void OnTriggerEnter(Collider col){
        PlayerController player = col.GetComponentInParent<PlayerController>();
        if(player != null){
            if(flyingModes < FlyingModes.TransitionLock){
                flyingModes = FlyingModes.TransitionLock;
            }
            if(flyingModes == FlyingModes.TransitionLock){
                flyingModes = transitionState;
            }
            onTransition(flyingModes);
            gameObject.SetActive(false);
        }
    }
}
