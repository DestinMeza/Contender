using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerControler>()){
            col.gameObject.SetActive(false);
        }
    }
}
