﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthController : MonoBehaviour
{
    public delegate void OnSpawned(bool spawned);
    public static event OnSpawned onSpawned = delegate{};
    public delegate void OnTakingDamage(int bossHp, int bossMaxHP);
    public static event OnTakingDamage onTakingDamage = delegate{};
    HealthController[] healthControllers;
    int _bossMaxHP;
    public int bossMaxHp{get {return _bossMaxHP; }}
    int bossHP;
    void Awake(){
        healthControllers = GetComponentsInChildren<HealthController>();
    }
    void OnEnable(){
        onSpawned(true);
        for(int i = 0; i < healthControllers.Length; i++){
            bossHP += healthControllers[i].health;
            healthControllers[i].onHealthDecrease += VitalHit;
            healthControllers[i].onDeath += Death;
        }
        _bossMaxHP = bossHP;
    }
    void Disable(){
        onSpawned(false);
        for(int i = 0; i < healthControllers.Length; i++){
            healthControllers[i].onHealthDecrease -= VitalHit;
            healthControllers[i].onDeath -= Death;
        }
    }
    void VitalHit(){
        bossHP--;
        onTakingDamage(bossHP, bossMaxHp);
    }

    void Death(HealthController health){
        AudioManager.Play("LargeObjectExplosion",1,1,false,transform.position,0.9f);
        ParticleManager.particleMan.Play("ExplosionLargeObject", transform.position);
        gameObject.SetActive(false);
    }

}