using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipControler : MonoBehaviour
{
    void Start(){
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Explode;
    }
    void Explode(HealthController health){
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        gameObject.SetActive(false);
    }
}
