using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FlyingModes{
    Rail,
    AllRange,
    TransitionLock
}
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
    public LayerMask enemy;
    public ChargeFire chargeFire = ChargeFire.Release;
    public BlasterState blasterState = BlasterState.SingleFire;
    public static FlyingModes flyingModes = FlyingModes.TransitionLock;
    public delegate void OnBlasterChange(BlasterState blaster);
    public static OnBlasterChange onBlasterChange = delegate {};
    public delegate void OnCrash(PlayerControler player);
    public static OnCrash onCrash = delegate {};
    public delegate void OnDeath(PlayerControler player);
    public static OnDeath onDeath = delegate {};
    public delegate void OnBoost();
    public static OnBoost onBoost = delegate {};
    public delegate void OnFireBomb(int count);
    public static OnFireBomb onFireBomb = delegate {};
    public static PlayerControler player;
    public Vector3 speedRail = new Vector3(30,-20, 50);
    public Vector3 speedAllRange = new Vector3(50,-40, 50);
    Transform chargeShotPos;
    Vector3 defaultSpeedRail;
    Vector3 defaultSpeedAllRange;
    public float maxSpeedChange = 10;
    public string bulletPrefab;
    public string bombPrefab;
    public string chargedBulletPrefab;
    public Transform firePosMain;
    public Transform firePos1;
    public Transform firePos2;
    public bool crash;
    public float boostMeterMax = 1;
    public float boostMeter = 1;
    public float lockHoldDuration = 1;
    public float lockOnDistance = 500;
    float lockHoldStart = 0;
    public float crashDuration = 3;
    public float crashTime;
    public float collisionShield = 0.5f;
    public int bombAmmo;
    public GameObject crossHair;
    float lastCollisionTime;
    bool breaking = false;
    HealthController health;
    BulletController[] bullets;
    Vector3 targetVelocity;
    Rigidbody rb;
    Animator anim;
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
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        health = GetComponentInParent<HealthController>();
    }

    void Start(){
        onFireBomb(bombAmmo);
        defaultSpeedRail = speedRail;
        defaultSpeedAllRange = speedAllRange;
        crashTime = Time.time;
        crash = false;
        breaking = false;
    }

    void TransitionLock(FlyingModes transition){
        flyingModes = transition;
    }
    void OnEnable(){
        boostMeter = boostMeterMax;
        blasterState = BlasterState.SingleFire;
        onBlasterChange(blasterState);
        onCrash(this);
        health.onDeath += Crash;
        TransitionController.onTransition += TransitionLock;
    }
    void OnDisable(){
        health.onDeath -= Crash;
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
        ClampPosition();
        if(flyingModes == FlyingModes.TransitionLock){
            StraightenPlayer();
            return;
        }
        if(flyingModes == FlyingModes.Rail)RailMovement();
        if(flyingModes == FlyingModes.AllRange)AllRangeMovement();

        if(Input.GetButtonDown("Fire2")){
            FireBomb();
            onFireBomb(bombAmmo);
        }
        Fire();
    }

    void StraightenPlayer(){
        transform.forward = Vector3.forward;
        targetVelocity = Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speedRail);
    }
    void FixedUpdate(){
        if(crash) return;
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

        if(breakAxis > 0.3f || Input.GetKey(KeyCode.B)){
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

        if(breakAxis > 0.3f || Input.GetKey(KeyCode.B)){
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
                    AudioManager.Play("BlasterSound");
                    GameObject bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
                    bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);
                    GameObject bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
                    bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
                break;
                case BlasterState.MegaFire:
                    AudioManager.Play("BlasterSound");
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
        
        if(Time.time - lockHoldStart > lockHoldDuration && Input.GetButton("Fire1") && chargeFire < ChargeFire.Waiting){
            Vector3 pos = crossHair.transform.position - cam.transform.position;
            Ray lockOnRay = new Ray(cam.transform.position, pos * 10000);
            RaycastHit hit;
            if(Physics.Raycast(lockOnRay, out hit, Mathf.Infinity, enemy, QueryTriggerInteraction.Collide)){
                if(hit.collider.GetComponentInParent<HealthController>()){
                    Transform enemyTransform = hit.collider.GetComponentInParent<Transform>();
                    chargeShotPos = enemyTransform;
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
        if(chargeFire == ChargeFire.Searching && Input.GetButtonUp("Fire1")){
            FireChargeShot();
        }
        if(chargeFire == ChargeFire.Waiting && Input.GetButtonDown("Fire1")){
            FireChargeShot();
        }
        if(!Input.GetButton("Fire1") && chargeFire == ChargeFire.Charging){
            chargeFire = ChargeFire.Release;
            anim.SetInteger("ChargeFireState", (int)chargeFire);
        }

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
        chargeFire = ChargeFire.Release;
        anim.SetInteger("ChargeFireState", (int)chargeFire);
        AudioManager.Play("BlasterSound");
        GameObject chargedBullet = SpawnManager.Spawn(chargedBulletPrefab, firePosMain.position);
        chargedBullet.GetComponentInParent<ChargedBulletController>().SetDir(chargeShotPos);
        chargeShotPos = null;
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

    void OnTriggerExit(Collider col){
        if(col.name == "RingCollider"){
            AudioManager.Play("RingSound");
            GameManager.game.IncrementScore();
            col.GetComponentInParent<RingController>().gameObject.SetActive(false);
        }
        if(col.name == "AllRangeModeBounds"){
            rb.velocity *= -1;
            transform.forward *= -1;
        }
    }
    void OnTriggerEnter(Collider col){
        if(col.tag == "BombPowerup"){
            bombAmmo++;
            onFireBomb(bombAmmo);
            col.gameObject.SetActive(false);
        }
        if(col.tag == "BlasterPowerup"){
            if(blasterState < BlasterState.MegaFire) blasterState++;
            col.gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision col){
        if(health.health <= 0){
            Crash();
        }
        else if(Time.time - lastCollisionTime > collisionShield){
            lastCollisionTime = Time.time;
            health.TakeDamage(1);
            if(!crash)anim.Play("PlayerHit");
        }
        if(crash) {
            onDeath(this);
            gameObject.SetActive(false);
        }
        
        Vector3 diff = rb.velocity - col.transform.position;
        rb.AddForce(diff.normalized*-1 + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0), ForceMode.Impulse);
    }
}
