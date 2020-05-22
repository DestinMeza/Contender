using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAreaController : MonoBehaviour
{
    public GameObject[] enemys;
    public Vector2 boundsX;
    public Vector2 boundsY;

    public bool useDefaultX = true;
    public bool useDefaultY = true;
    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerController>()){
            foreach (GameObject obj in enemys){
                obj.SetActive(true);
            }
            if(!useDefaultX) Camera.main.GetComponent<CameraController>().maxX = boundsX;
            if(!useDefaultY) Camera.main.GetComponent<CameraController>().maxY = boundsY;
        }
    }
    void OnTriggerExit(Collider col){
        if(col.GetComponentInParent<PlayerController>()){
            foreach (GameObject obj in enemys){
                obj.SetActive(false);
            }
            
        }
    }
}
