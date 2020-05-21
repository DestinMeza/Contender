using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
    public delegate void OnTriggered(Collider col);
    public event OnTriggered onTriggered = delegate {}; 
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerController>()){
            onTriggered(col);
        }
    }
}
