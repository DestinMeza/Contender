using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAnimatorController : MonoBehaviour
{
    public delegate void OnDeathCalculation();
    public static event OnDeathCalculation onDeathCalculation = delegate {};
    public enum AnimationState{
        Idel,
        StraightAhead,
        MissionAnim1
    }

    public string deathParticles = "ExplosionSmallObject";
    public AnimationState animationState = AnimationState.Idel;
    public Animator anim;

    void OnEnable(){
        GetComponent<HealthController>().onDeath += Explode;
        anim.SetInteger("AnimationState", (int)animationState);
    }

    void Idel(){
        animationState = AnimationState.Idel;
    }

    void Explode(HealthController health){
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        onDeathCalculation();
        gameObject.SetActive(false);
    }
}
