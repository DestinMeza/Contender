using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAsteroidController : MonoBehaviour
{
    HealthController health;
    void Awake(){
        health = GetComponent<HealthController>();
    }
    void OnEnable()
    {
        health.onDeath += Explode;
    }
    void Explode(HealthController health){
        gameObject.SetActive(false);
    }
}
