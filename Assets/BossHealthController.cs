using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthController : MonoBehaviour
{
    public delegate void OnSpawned(bool spawned);
    public static event OnSpawned onSpawned = delegate{};
    public delegate void OnBossDeath();
    public static event OnBossDeath onBossDeath = delegate{};
    public delegate void OnTakingDamage(int bossHp, int bossMaxHP);
    public static event OnTakingDamage onTakingDamage = delegate{};
    HealthController[] healthControllers;
    int _bossMaxHP;
    public int bossMaxHp{get {return _bossMaxHP; }}
    int bossHP;
    void Awake(){
        healthControllers = GetComponentsInChildren<HealthController>();
    }

    void Update(){
        for(int i = 0; i < healthControllers.Length; i++){
            if(healthControllers[i].health <= 0){
                healthControllers[i].gameObject.SetActive(false);
            }
        }
        
        if(bossHP <= 0){
            Death();
        }
    }
    void OnEnable(){
        onSpawned(true);
        for(int i = 0; i < healthControllers.Length; i++){
            bossHP += healthControllers[i].health;
            healthControllers[i].onHealthDecrease += VitalHit;
        }
        _bossMaxHP = bossHP;
    }
    void Disable(){
        onSpawned(false);
        for(int i = 0; i < healthControllers.Length; i++){
            healthControllers[i].onHealthDecrease -= VitalHit;
        }
    }
    void VitalHit(){
        bossHP--;
        onTakingDamage(bossHP, bossMaxHp);
    }

    void Death(){
        AudioManager.Play("LargeObjectExplosion",1,1,false,transform.position,0.9f);
        ParticleManager.particleMan.Play("ExplosionLargeObject", transform.position);
        onBossDeath();
        gameObject.SetActive(false);
    }

}
