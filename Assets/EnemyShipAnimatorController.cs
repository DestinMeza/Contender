using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAnimatorController : MonoBehaviour
{
    public enum AnimationState{
        Idel,
        StraightAhead
    }
    public AnimationState animationState = AnimationState.Idel;
    public Animator anim;

    void OnEnable(){
        anim.SetInteger("AnimationState", (int)animationState);
    }

    void Idel(){
        animationState = AnimationState.Idel;
    }
}
