using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    enum BossAttackState{
        Stage1,
        Stage2,
    }
    BossAttackState bossAttackState = BossAttackState.Stage1;
    public Transform firePos;
    public Transform lazerParent;
    public Transform lazerOrientation;
    public ParticleSystem lazerCharge;
    public int numberPerVolley = 8;
    public float firingPause = 8;
    public float fireInterval = 0.2f;
    public float lazerFiringDistance = 500;
    public float stretchToPointTime = 4;
    public float lazerFireDuration = 3;
    public float lazerFiringDamping1 = 10;
    public float lazerFiringDamping2 = 0.7f;
    public float lazerRotationalDamp = 0.8f;
    Animator anim;
    int volleyIndex;
    float lastShot;
    float reloadingTime;
    float reloadingLazerTime;
    float lazerFiringTime;
    bool lazerPlaying;
    Transform target;
    void Awake(){
        anim = GetComponent<Animator>();
        target = PlayerController.player.hitbox;
        BossHealthController.onTriggerStageTwo += Stage2;
    }
    void Update()
    {
        target = PlayerController.player.hitbox;
        if(bossAttackState == BossAttackState.Stage1){
            MissleAttack();
            LazerAttack(lazerFiringDamping2);
        }

        if(bossAttackState == BossAttackState.Stage2){
            LazerAttack(lazerFiringDamping2);
        }    
    }

    void Stage2(){
        bossAttackState = BossAttackState.Stage2;
    }

    void MissleAttack(){
        if(Time.time - reloadingTime > firingPause && volleyIndex < numberPerVolley){
            Fire(target);
        }
        else{
            volleyIndex = 0;
        }
    }

    void LazerAttack(float firingDamping){
        if((target.position - transform.position).magnitude < lazerFiringDistance && Time.time - reloadingLazerTime > firingPause * firingDamping){
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
            lazerOrientation.gameObject.SetActive(true);
            CapsuleCollider col = lazerOrientation.GetComponent<CapsuleCollider>();
            Vector3 diff = PlayerController.player.transform.position - lazerParent.transform.position;
            Quaternion rotation = Quaternion.LookRotation(diff);
            lazerOrientation.rotation = Quaternion.Slerp(lazerOrientation.rotation, rotation, lazerRotationalDamp);
            float dis = diff.magnitude;
            lazerParent.localScale = new Vector3(0.5f, 0.5f, Mathf.Lerp(0, dis * 0.75f, t/stretchToPointTime));
            col.center = new Vector3(0, 0, lazerParent.localScale.z / 2);
            col.height = lazerParent.localScale.z;
            lazerFiringTime = Time.time;
            yield return new WaitForEndOfFrame();
        }
        while(Time.time - lazerFiringTime > lazerFireDuration){
            Vector3 rand = Random.insideUnitSphere;
            lazerOrientation.forward = rand + PlayerController.player.transform.position - lazerParent.transform.position;
        }
        for(float t = 0; t <= stretchToPointTime; t += Time.deltaTime){
            lazerOrientation.gameObject.SetActive(true);
            CapsuleCollider col = lazerOrientation.GetComponent<CapsuleCollider>();
            Vector3 diff = PlayerController.player.transform.position - lazerParent.transform.position;
            Quaternion rotation = Quaternion.LookRotation(diff);
            float dis = diff.magnitude;
            lazerParent.localScale = new Vector3(0.5f, 0.5f, Mathf.Lerp(dis, 0, t/stretchToPointTime));
            col.center = new Vector3(0, 0, lazerParent.localScale.z / 2);
            col.height = lazerParent.localScale.z;
            yield return new WaitForEndOfFrame();
        }
        reloadingLazerTime = Time.time;
        lazerPlaying = false;
        yield return new WaitForEndOfFrame();
    }
}
