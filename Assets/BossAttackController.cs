using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    public Transform firePos;
    public Transform lazerParent;
    public Transform lazerOrientation;
    public ParticleSystem lazerCharge;
    public int numberPerVolley = 3;
    public float firingPause = 8;
    public float fireInterval = 0.2f;
    public float lazerFiringDistance = 500;
    public float stretchToPointTime = 4;
    public float lazerFireDuration = 3;
    Animator anim;
    int volleyIndex;
    float lastShot;
    float reloadingTime;
    float reloadingLazerTime;
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
        if((target.position - transform.position).magnitude < lazerFiringDistance && Time.time - reloadingLazerTime > firingPause){
            LazerCharge();
        }
    }

    void Fire(Transform target){
        if(Time.time - lastShot > fireInterval){
            MissileController missle = SpawnManager.Spawn("BossMissileRoot", firePos.position).GetComponent<MissileController>();
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
            lazerParent.gameObject.SetActive(true);
            CapsuleCollider col = lazerOrientation.GetComponent<CapsuleCollider>();
            lazerOrientation.forward = PlayerController.player.transform.position - lazerParent.transform.position;
            Vector3 diff = PlayerController.player.transform.position - lazerParent.transform.position;
            float dis = diff.magnitude;
            lazerParent.localScale = new Vector3(1, 1, Mathf.Lerp(0, dis, t/stretchToPointTime));
            col.center = new Vector3(0, 0, lazerParent.localScale.z / 2);
            col.height = lazerParent.localScale.z;
            lazerFiringTime = Time.time;
            yield return new WaitForEndOfFrame();
        }
        while(lazerPlaying){
            if(Time.time - lazerFiringTime > lazerFireDuration){
                lazerParent.gameObject.SetActive(false);
                lazerPlaying = false;
                reloadingLazerTime = Time.time;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
