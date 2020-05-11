using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlasterState{
        SingleFire,
        DoubleFire,
        MegaFire

}

public class PlayerControler : MonoBehaviour
{
    public enum ChargeFire {
        Charging,
        Searching,
        Waiting,
        Release
    }
    public static FlyingModes flyingModes = FlyingModes.TransitionLock;
    public LayerMask enemy;
    public ChargeFire chargeFire = ChargeFire.Release;
    public BlasterState blasterState = BlasterState.SingleFire;
    public delegate void OnBarrelRoll(bool barrelRoll);
    public static event OnBarrelRoll onBarrelRoll = delegate{};
    public delegate void OnLoop(bool looping);
    public static event OnLoop onLoop = delegate{};
    public delegate void OnBlasterChange(BlasterState blaster);
    public static event OnBlasterChange onBlasterChange = delegate {};
    public delegate void OnCrash(PlayerControler player);
    public static event OnCrash onCrash = delegate {};
    public delegate void OnDeath(PlayerControler player);
    public static event OnDeath onDeath = delegate {};
    public delegate void OnBoost();
    public static event OnBoost onBoost = delegate {};
    public delegate void OnFireBomb(int count);
    public static event OnFireBomb onFireBomb = delegate {};
    public static PlayerControler player;
    public Vector3 speedRail = new Vector3(30,-20, 50);
    public Vector3 speedAllRange = new Vector3(50,-40, 50);
    public Vector3 lockOnOffset = new Vector3(0,0,60);
    Transform chargeShotPos;
    Vector3 defaultSpeedRail;
    Vector3 defaultSpeedAllRange;
    public float maxSpeedChange = 10;
    public string bulletPrefab;
    public string bombPrefab;
    public string chargedBulletPrefab;
    public ParticleSystem barrelRollParticle;
    public GameObject lockedOnEnemy;
    public GameObject crossHair;
    public LockOnCrossHairController lockOnCrossHairs;
    public Transform firePosMain;
    public Transform firePos1;
    public Transform firePos2;
    public bool crash;
    public float boostMeterMax = 1;
    public float boostMeter = 1;
    public float lockHoldDuration = 1;
    public float lockOnDistance = 500;
    float lockHoldStart;
    public float crashDuration = 3;
    public float crashTime;
    public float collisionShield = 0.5f;
    public int bombAmmo;
    float lastCollisionTime;
    bool looping = false;
    bool breaking = false;
    bool charged = false;
    bool barrelRoll = false;
    public Transform enemyTransform;
    HealthController health;
    BulletController[] bullets;
    Vector3 targetVelocity;
    Rigidbody rb;
    Animator anim;
    public Camera minimapCam;
    Camera cam;

    void Awake()
    {
        if(player == null){
            player = this;
        }
        else{
            Destroy(gameObject);
        }
        
        cam = Camera.main;
        minimapCam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        health = GetComponentInParent<HealthController>();
    }

    void Start(){
        defaultSpeedRail = speedRail;
        defaultSpeedAllRange = speedAllRange;
        crashTime = Time.time;
        crash = false;
        breaking = false;
        onFireBomb(bombAmmo);
    }

    void TransitionLock(FlyingModes transition){
        flyingModes = transition;
        chargeFire = ChargeFire.Release;
        anim.SetInteger("ChargeFireState", (int)chargeFire);
    }
    void OnEnable(){
        boostMeter = boostMeterMax;
        blasterState = BlasterState.SingleFire;
        onBlasterChange(blasterState);
        onCrash(this);
        onFireBomb(bombAmmo);
        chargeFire = ChargeFire.Release;
        anim.SetInteger("ChargeFireState", (int)chargeFire);
        health.onDeath += Crash;
        health.onHealthDecrease += Hit;
        TransitionController.onTransition += TransitionLock;
    }
    void OnDisable(){
        health.onDeath -= Crash;
        health.onHealthIncrease -= Hit;
        TransitionController.onTransition -= TransitionLock;
    }
    void Update(){
        if(crash)
        {
            if(Time.time - crashTime > crashDuration){
                onDeath(this);
                gameObject.SetActive(false);
            }
            return;
        }
        if(GameManager.game.gameState == GameState.GameStart){
            return;
        }
        onCrash(this);
        if(flyingModes == FlyingModes.TransitionLock){
            StraightenPlayer();
            return;
        }
        if(!barrelRoll){
            if(Input.GetButtonDown("Fire2")){
                FireBomb();
                onFireBomb(bombAmmo);
            }
            Fire();
        }
        
        if(looping) return;
        ClampPosition();
        if(flyingModes == FlyingModes.Rail)RailMovement();
        if(flyingModes == FlyingModes.AllRange)AllRangeMovement();

        if(Input.GetButtonDown("BarrelRoll") || Input.GetKeyDown(KeyCode.E)){
            barrelRollParticle.Play();
            AudioManager.Play("BarrelRoll");
            anim.Play("BarrelRoll");
            barrelRoll = true;
            onBarrelRoll(barrelRoll);
        }
        
        if(Input.GetButton("Bank") && Input.GetButton("BarrelRoll") || Input.GetKeyDown(KeyCode.Q)){
            looping = true;
            rb.velocity = Vector3.zero;
            rb.velocity = transform.forward * defaultSpeedRail.z/2;
            anim.Play("PlayerLoop");
        }
    }

    void StraightenPlayer(){
        anim.SetFloat("xVel", 0);
        anim.SetFloat("yVel", 0);
        transform.forward = Vector3.forward;
        targetVelocity = Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speedRail);
    }
    void FixedUpdate(){
        if(crash || looping) return;
        if(flyingModes == FlyingModes.Rail){
            Vector3 velocityChange = targetVelocity - rb.velocity;
            Vector3 xyChange = velocityChange;
            xyChange.z = 0;
            if(xyChange.sqrMagnitude > maxSpeedChange * maxSpeedChange){
                xyChange = xyChange.normalized * maxSpeedChange;
                velocityChange.x = xyChange.x;
                velocityChange.y = xyChange.y;
            }
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        else{
            Vector3 velocityChange = targetVelocity - rb.velocity;
            Vector3 dirChange = velocityChange;
            if(dirChange.sqrMagnitude > maxSpeedChange * maxSpeedChange){
                dirChange = dirChange.normalized * maxSpeedChange;
                velocityChange.x = dirChange.x;
                velocityChange.y = dirChange.y;
                velocityChange.z = dirChange.z;
            }
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    void ClampPosition(){
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void RailMovement(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float boostAxis = Input.GetAxis("Boost");
        float breakAxis = Input.GetAxis("Break");
        
        if(boostAxis > 0.3f || Input.GetKey(KeyCode.LeftShift) && breaking == false){
            if(boostMeter > 0 && breaking == false){
                boostMeter -= 0.5f * Time.deltaTime;
                speedRail = defaultSpeedRail * 1.5f;
            }
            else{
                speedRail = defaultSpeedRail;
            }
        }
        else{
            if(boostMeter < boostMeterMax)boostMeter += 0.25f * Time.deltaTime;
            speedRail = defaultSpeedRail;
        }
        onBoost();

        if(breakAxis > 0.3f || Input.GetKey(KeyCode.Space)){
            speedRail.z = defaultSpeedRail.z * 0.5f;
            breaking = true;
        }
        else{
            breaking = false;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Bank")){
            speedRail.x = defaultSpeedRail.x * 1.5f;
            x *= 2;
        }
        
        targetVelocity = new Vector3(x, y, 0).normalized + Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speedRail);

        anim.SetFloat("xVel", x);
        anim.SetFloat("yVel", -y);
    }

    void AllRangeMovement(){
        float x = Input.GetAxis("Horizontal");
        transform.forward = (transform.forward + transform.right * x * Time.deltaTime).normalized;
        float y = Input.GetAxis("Vertical");
        float boostAxis = Input.GetAxis("Boost");
        float breakAxis = Input.GetAxis("Break");
        
        if(boostAxis > 0.3f || Input.GetKey(KeyCode.LeftShift) && breaking == false){
            if(boostMeter > 0 && breaking == false){
                boostMeter -= 0.5f * Time.deltaTime;
                speedAllRange = defaultSpeedAllRange * 1.5f;
            }
            else{
                speedAllRange = defaultSpeedAllRange;
            }
        }
        else{
            if(boostMeter < boostMeterMax)boostMeter += 0.25f * Time.deltaTime;
            speedAllRange = defaultSpeedAllRange;
        }
        onBoost();

        if(breakAxis > 0.3f || Input.GetKey(KeyCode.Space)){
            speedAllRange.z = defaultSpeedAllRange.z * 0.5f;
            breaking = true;
        }
        else{
            breaking = false;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Bank")){
            speedAllRange.x = defaultSpeedAllRange.x * 1.5f;
            x *= 2;
        }

        Vector3 v = new Vector3(x, y, 0).normalized + Vector3.forward;
        v = Vector3.Scale(v, speedAllRange); 
        targetVelocity = transform.right * v.x + transform.up * v.y + transform.forward * v.z;

        anim.SetFloat("xVel", x);
        anim.SetFloat("yVel", -y);
    }

    void Fire(){

        if(chargeFire == ChargeFire.Release && Input.GetButtonDown("Fire1")){
            switch (blasterState){
                case BlasterState.SingleFire:
                    AudioManager.Play("BlasterSound");
                    GameObject bullet = SpawnManager.Spawn(bulletPrefab, firePosMain.position);
                    bullet.GetComponentInParent<BulletController>().SetDir(firePosMain.forward);
                    break;
                case BlasterState.DoubleFire:
                    AudioManager.Play("BlasterSound2");
                    GameObject bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
                    bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);
                    GameObject bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
                    bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
                break;
                case BlasterState.MegaFire:
                    AudioManager.Play("BlasterSound2");
                    bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
                    bullet1.GetComponentInParent<DamageController>().damage *= 2;
                    bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);
                    bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
                    bullet2.GetComponentInParent<DamageController>().damage *= 2;
                    bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
                break;
            }
            lockHoldStart = Time.time;
            chargeFire = ChargeFire.Charging;
            anim.SetInteger("ChargeFireState", (int)chargeFire);
        }
        
        if(charged && Input.GetButton("Fire1") && chargeFire < ChargeFire.Waiting){
            Vector3 pos = crossHair.transform.position - cam.transform.position;
            Ray lockOnRay = new Ray(cam.transform.position, pos.normalized * 10000);
            RaycastHit hit;
            if(Physics.Raycast(lockOnRay, out hit, Mathf.Infinity, enemy, QueryTriggerInteraction.Collide)){
                if(hit.collider.GetComponentInParent<HealthController>()){
                    enemyTransform = hit.collider.GetComponentInParent<Transform>();
                    chargeShotPos = enemyTransform;
                    lockedOnEnemy = enemyTransform.GetComponentInParent<HealthController>().gameObject;
                    AudioManager.Play("ChargedUpLockOnSound");
                    chargeFire = ChargeFire.Waiting;
                    anim.SetInteger("ChargeFireState", (int)chargeFire);
                }
                else{
                    chargeFire = ChargeFire.Searching;
                    anim.SetInteger("ChargeFireState", (int)chargeFire);
                }
            }
            else{
                chargeFire = ChargeFire.Searching;
                anim.SetInteger("ChargeFireState", (int)chargeFire);
            }
            
        }
        if(chargeFire == ChargeFire.Charging && Time.time - lockHoldStart > lockHoldDuration){
            charged = true;
        }
        if(chargeFire == ChargeFire.Searching && Input.GetButtonUp("Fire1")){
            FireChargeShot();
        }
        if(chargeFire == ChargeFire.Waiting && Input.GetButtonDown("Fire1")){
            FireChargeShot();
        }
        if(!Input.GetButton("Fire1") && chargeFire == ChargeFire.Charging){
            charged = false;
            chargeFire = ChargeFire.Release;
            anim.SetInteger("ChargeFireState", (int)chargeFire);
        }
        anim.SetInteger("ChargeFireState", (int)chargeFire);
        if(lockedOnEnemy != null){
            if(!lockedOnEnemy.gameObject.activeSelf){
                enemyTransform = null;
                lockedOnEnemy = null;
            }
        }
        if(lockOnCrossHairs!= null) lockOnCrossHairs.LockOnCrossHairs(lockedOnEnemy);
    }
    void EndBarrelRoll(){
        barrelRoll = false;
        onBarrelRoll(barrelRoll);
    }
    public void SearchingBeep(){
        AudioManager.Play("ChargeBombSearchingBeep");
    }
    public void ChargingSound(){
        AudioManager.Play("ChargeBombChargingBeep");
    }
    // void OnDrawGizmos(){
    //     Vector3 pos = crossHair.transform.position - cam.transform.position;
    //     Ray lockOnRay = new Ray(cam.transform.position, pos);
    //     RaycastHit hit;
        
    //     if(Physics.Raycast(lockOnRay, out hit, 10000, enemy, QueryTriggerInteraction.Collide)){
    //         Gizmos.DrawSphere(hit.point, 10);
    //         Gizmos.color = Color.red;
    //     }
    //     else{
    //         Gizmos.DrawRay(lockOnRay.origin, lockOnRay.direction * 10000);
    //         Gizmos.color = Color.white;
    //     }
    // }

    void FireChargeShot(){
        charged = false;
        chargeFire = ChargeFire.Release;
        anim.SetInteger("ChargeFireState", (int)chargeFire);
        AudioManager.Play("BlasterSound");
        GameObject chargedBullet = SpawnManager.Spawn(chargedBulletPrefab, firePosMain.position);
        chargedBullet.GetComponentInParent<ChargedBulletController>().SetDir(chargeShotPos);
        chargeShotPos = null;
        enemyTransform = null;
        lockedOnEnemy = null;
    }
    void FireBomb(){
        BBombController lastBomb = FindObjectOfType<BBombController>();
        if(lastBomb == null){
            if(bombAmmo < 1) return;
            bombAmmo--;
            GameObject bomb = SpawnManager.Spawn(bombPrefab, firePosMain.position);
            bomb.GetComponentInParent<BBombController>().SetDir(firePosMain.forward);
        }
        else{
            lastBomb.Explode();
        }
    }
    void Crash(HealthController health){
        if(crash) return;
        crossHair.SetActive(false);
        anim.Play("PlayerCrash");
        crash = true;
        anim.SetBool("crashing", crash);
        rb.AddForce(transform.forward.normalized, ForceMode.Impulse);
        rb.velocity = Vector3.forward * speedRail.z;
        rb.useGravity = true;
        crashTime = Time.time;
        onCrash(this);
    }

    void Crash(){
        if(crash) return;
        crossHair.SetActive(false);
        anim.Play("PlayerCrash");
        crash = true;
        anim.SetBool("crashing", crash);
        rb.AddForce(transform.forward.normalized, ForceMode.Impulse);
        rb.velocity = Vector3.forward * speedRail.z;
        rb.useGravity = true;
        crashTime = Time.time;
        onCrash(this);
    }
    public void LoopAllRange(){
        if(flyingModes == FlyingModes.AllRange)
        transform.forward *= -1;
        looping = false;
        onLoop(looping);
    }
    void Hit(){
        if(looping || crash) return;
        AudioManager.Play("BlasterHit");
        anim.Play("PlayerHit");
        anim.Play("HitFlash");
    }
    void OnTriggerExit(Collider col){
        if(col.name == "RingCollider"){
            AudioManager.Play("RingSound");
            GameManager.game.IncrementRingScore();
            col.GetComponentInParent<RingController>().gameObject.SetActive(false);
        }
        if(col.name == "AllRangeModeBounds"){
            looping = true;
            anim.Play("PlayerLoop");
        }
    }
    void OnTriggerEnter(Collider col){
        if(col.tag == "BombPowerup"){
            bombAmmo++;
            AudioManager.Play("BombPickUp");
            onFireBomb(bombAmmo);
            col.gameObject.SetActive(false);
        }
        else if(col.tag == "BlasterPowerup"){
            if(blasterState < BlasterState.MegaFire) blasterState++;
            AudioManager.Play("BlasterPowerUp");
            col.gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision col){
        if(!looping || !crash){
            rb.AddForce(targetVelocity.normalized + transform.up, ForceMode.Impulse);
        }
        
        if(health.health <= 0){
            Crash();
        }
        else if(Time.time - lastCollisionTime > collisionShield){
            lastCollisionTime = Time.time;
            health.TakeDamage(1);
            AudioManager.Play("ObjectHit",1,1,false,transform.position,0.9f);
        }
        if(crash) {
            onDeath(this);
            gameObject.SetActive(false);
        }
        
    }
}