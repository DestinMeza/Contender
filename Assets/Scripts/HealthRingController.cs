using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRingController : MonoBehaviour
{
    public int health = 5;
    public string SFX = "HealthGoldPickup";
    void OnTriggerEnter(Collider col){
        PlayerController player = col.GetComponentInParent<PlayerController>();
        if(player != null){
            col.GetComponentInParent<HealthController>().IncreaseHeath(health);
            AudioManager.Play(SFX);
            gameObject.SetActive(false);
        }
    }
}
