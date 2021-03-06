﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAnimatorController : MonoBehaviour
{
    public delegate void OnDeathCalculation(Vector3 position);
    public static event OnDeathCalculation onDeathCalculation = delegate {};
    public enum AnimationState{
        Idel,
        StraightAhead,
        MissionAnim1,
        MissionAnim2
    }

    public Transform shipPos;
    public Transform firingPos1;
    public AnimationState animationState = AnimationState.Idel;
    public Animator anim;
    public string enemyBulletPrefab = "EnemyBullet";
    public string deathParticles = "ExplosionSmallObject";
    void OnEnable(){
        GetComponentInChildren<HealthController>().onDeath += Explode;
        anim.SetInteger("AnimationState", (int)animationState);
    }

    void Idel(){
        animationState = AnimationState.Idel;
    }

    void FireTrackPlayer(){
        Vector3 diff = PlayerController.player.transform.position - shipPos.position;
        GameObject bullet = SpawnManager.Spawn(enemyBulletPrefab, firingPos1.position);
        if(Vector3.Dot(diff, shipPos.forward) > 0) bullet.GetComponentInParent<BulletController>().SetDir(diff.normalized);
        else{
            bullet.GetComponentInParent<BulletController>().SetDir(shipPos.forward.normalized);
        }
        AudioManager.Play("BlasterSound",1,1,false,shipPos.position,0.9f);
    }

    void Fire(){
        GameObject bullet = SpawnManager.Spawn(enemyBulletPrefab, firingPos1.position);
        bullet.GetComponentInParent<BulletController>().SetDir(shipPos.forward.normalized);
        AudioManager.Play("BlasterSound",1,1,false,shipPos.position,0.9f);
    }
    void Explode(HealthController health){
        ParticleManager.particleMan.Play(deathParticles, shipPos.position);
        AudioManager.Play("SmallObjectExplosion",1,1,false,shipPos.position,0.8f);
        gameObject.SetActive(false);
    }
}
