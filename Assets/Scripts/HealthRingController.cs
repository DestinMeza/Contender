using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRingController : MonoBehaviour
{
    public int health = 5;
    void OnTriggerEnter(Collider col){
        PlayerControler player = col.GetComponentInParent<PlayerControler>();
        if(player != null){
            player.GetComponentInParent<HealthController>().health += health;
        }
    }
}
