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
    public delegate void OnTriggerStageTwo();
    public static event OnTriggerStageTwo onTriggerStageTwo = delegate{};
    HealthController[] healthControllers;
    int _bossMaxHP;
    public int bossMaxHp{get {return _bossMaxHP; }}
    int bossHP;
    Animator anim;
    bool deathTrigger;
    bool explosionTrigger;
    CameraController cam;
    void Awake(){
        healthControllers = GetComponentsInChildren<HealthController>();
        anim = GetComponent<Animator>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    void Update(){
        for(int i = 0; i < healthControllers.Length; i++){
            if(healthControllers[i].health <= 0){
                if(healthControllers[i].gameObject.name == "HitSpotTail" && !explosionTrigger){
                    ParticleManager.particleMan.Play("ExplosionLargeObject", healthControllers[i].gameObject.transform.position);
                    AudioManager.Play("LargeObjectExplosion");
                    onTriggerStageTwo();
                    healthControllers[i].gameObject.SetActive(false);
                    explosionTrigger = true;
                }
            }
        }
        
        if(bossHP <= 0 && !deathTrigger){
            DeathAnimation();
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

    void DeathAnimation(){
        cam.target = this.gameObject.transform;
        cam.heading = this.gameObject.transform;
        deathTrigger = true;
        anim.Play("DragonMechDeath");
        
    }

    void Death(){
        AudioManager.Play("LargeObjectExplosion",1,1,false,transform.position,0.9f);
        ParticleManager.particleMan.Play("ExplosionLargeObject", transform.position);
        onBossDeath();
        cam.target = PlayerController.player.transform;
        cam.heading = PlayerController.player.heading;
        gameObject.SetActive(false);
    }

}
