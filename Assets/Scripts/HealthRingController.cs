using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRingController : MonoBehaviour
{
    public int health = 5;
    void OnTriggerEnter(Collider col){
        PlayerControler player = col.GetComponentInParent<PlayerControler>();
        if(player != null){
            col.GetComponentInParent<HealthController>().IncreaseHeath(health);
            gameObject.SetActive(false);
        }
    }
}
