using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaController : MonoBehaviour
{
    public GameObject[] enemys;
    
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerControler>()){
            foreach (GameObject obj in enemys){
                obj.SetActive(true);
            }
        }
    }
    void OnTriggerExit(Collider col){
        if(col.GetComponentInParent<PlayerControler>()){
            foreach (GameObject obj in enemys){
                obj.SetActive(false);
            }
        }
    }
}
