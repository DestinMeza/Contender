﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDroidController : MonoBehaviour
{
    // Start is called before the first frame update
    HealthController health;

    void Awake(){
        health = GetComponent<HealthController>();
    }
    void Start()
    {
        health.onDeath += Explode;
    }

    void Explode(HealthController health){
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        gameObject.SetActive(false);
    }
}