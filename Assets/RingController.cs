using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    void OnTriggerExit(Collider col){
        if(col.gameObject.GetComponentInParent<PlayerControler>()){
            gameObject.SetActive(false);
        }
    }
}
