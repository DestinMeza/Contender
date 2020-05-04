using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TransitionController : MonoBehaviour
{
    public delegate void OnTransition(bool isTransition);
    public static event OnTransition onTransition = delegate {};
    bool isTransition;
    void OnTriggerEnter(Collider col){
        PlayerControler player = col.GetComponentInParent<PlayerControler>();
        if(player != null){
            if(isTransition) isTransition = false;
            else isTransition = true;
            onTransition(isTransition);
        }
    }
}
