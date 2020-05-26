using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    public LayerMask solidObjects;
    public Transform firePos;
    public LineRenderer lazer;
    public ParticleSystem lazerCharge;
    public int numberPerVolley = 8;
    public float firingPause = 8;
    public float fireInterval = 0.2f;
    public float lazerFiringDistance = 500;
    public float stretchToPointTime = 2;
    public float lazerFireDuration = 3;
    Animator anim;
    int volleyIndex;
    float lastShot;
    float reloadingTime;
    float lazerFiringTime;
    bool lazerPlaying;
    void Awake(){
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Transform target = PlayerController.player.hitbox;
        if(Time.time - reloadingTime > firingPause && volleyIndex < numberPerVolley){
            Fire(target);
        }
        else{
            volleyIndex = 0;
        }
        if((target.position - transform.position).magnitude < lazerFiringDistance){
            LazerCharge();
        }
        else{
            lazerCharge.Stop();
        }
    }
    void Fire(Transform target){
        if(Time.time - lastShot > fireInterval){
            MissileController missle = SpawnManager.Spawn("MissileRoot", firePos.position).GetComponent<MissileController>();
            missle.SetTarget(target);
            lastShot = Time.time;
            volleyIndex++;
        }
        if(volleyIndex >= numberPerVolley){
            reloadingTime = Time.time;
        }
    }
    void LazerCharge(){
        if(lazerPlaying) return;
        lazerCharge.Play();
        anim.Play("LazerFire");
        lazerPlaying = true;
    }

    void StartLazerFire(){
        StopCoroutine(LazerFire());
        StartCoroutine(LazerFire());
    }
    IEnumerator LazerFire(){
        for(float t = 0; t <= stretchToPointTime; t += Time.deltaTime){
            lazer.gameObject.SetActive(true);
            Transform playerTarget = PlayerController.player.hitbox;
            int previousSegment = lazer.positionCount -2;
            Vector3 previousPos = lazer.GetPosition(previousSegment);
            Vector3 lazerFinalPos = Vector3.Lerp(previousPos, playerTarget.position, t/stretchToPointTime);
            lazer.SetPosition(lazer.positionCount-1, lazerFinalPos);
            Ray ray = new Ray(lazer.GetPosition(0), lazer.GetPosition(lazer.positionCount-1));
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, lazer.GetPosition(lazer.positionCount-1).magnitude, solidObjects, QueryTriggerInteraction.Collide)){
                if(hit.collider.GetComponentInParent<HealthController>()){
                    hit.collider.GetComponentInParent<HealthController>().health -= 1;
                }
            }
            lazerFiringTime = Time.time;
            yield return new WaitForEndOfFrame();
        }
        if(Time.time - lazerFiringTime > lazerFireDuration){
            lazer.gameObject.SetActive(false);
            lazerPlaying = false;
        }
    }
}
