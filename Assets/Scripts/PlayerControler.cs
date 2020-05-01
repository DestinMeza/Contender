using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    enum BlasterState{
        SingleFire,
        DoubleFire,
        MegaFire
    }
    BlasterState blasterState = BlasterState.SingleFire;
    public delegate void OnCrash(PlayerControler player);
    public static OnCrash onCrash = delegate {};
    public delegate void OnDeath(PlayerControler player);
    public static OnDeath onDeath = delegate {};
    public delegate void OnBoost();
    public static OnBoost onBoost = delegate {};
    public delegate void OnFireBomb(int count);
    public static OnFireBomb onFireBomb = delegate {};
    public static PlayerControler player;
    public Vector3 speed = new Vector3(30,-20, 50);
    Vector3 defaultSpeed;
    public float maxSpeedChange = 10;
    public string bulletPrefab;
    public Transform firePosMain;
    public Transform firePos1;
    public Transform firePos2;
    public float boostMeterMax = 1;
    public float boostMeter = 1;
    public bool crash;
    public float crashDuration = 3;
    public float crashTime;
    public float collisionShield = 0.5f;
    public int bombAmmo;
    float lastCollisionTime;
    bool breaking = false;
    HealthController health;
    BulletController[] bullets;
    Vector3 targetVelocity;
    Rigidbody rb;
    Animator anim;

    void Awake()
    {
        if(player == null){
            player = this;
        }
        else{
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start(){
        defaultSpeed = speed;
        health = GetComponentInParent<HealthController>();
        health.onDeath += Crash;
        crashTime = Time.time;
        crash = false;
    }

    void OnEnable(){
        boostMeter = boostMeterMax;
        blasterState = BlasterState.SingleFire;
        onCrash(this);
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
        Movement();

        if(Input.GetButtonDown("Fire2")){
            if(bombAmmo < 0) return;
            bombAmmo--;
            onFireBomb(bombAmmo);
            return;
        }

        if(Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)){
            Fire();
        }
        
    }
    void FixedUpdate(){
        if(crash) return;
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

    void ClampPosition(){
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void Movement(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float boostAxis = Input.GetAxis("Boost");
        float breakAxis = Input.GetAxis("Break");
        
        if(boostAxis > 0.3f || Input.GetKey(KeyCode.LeftShift) && breaking == false){
            if(boostMeter > 0 && breaking == false){
                boostMeter -= 0.5f * Time.deltaTime;
                speed = defaultSpeed * 1.5f;
            }
            else{
                speed = defaultSpeed;
            }
        }
        else{
            if(boostMeter < boostMeterMax)boostMeter += 0.25f * Time.deltaTime;
            speed = defaultSpeed;
        }
        if(breakAxis > 0.3f || Input.GetKey(KeyCode.B)){
            speed.z = defaultSpeed.z * 0.5f;
            breaking = true;
        }
        else{
            breaking = false;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Bank")){
            speed.x = defaultSpeed.x * 1.5f;
            x *= 2;
        }
        
        onBoost();
        targetVelocity = new Vector3(x, y, 0).normalized + Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speed);

        anim.SetFloat("xVel", x);
        anim.SetFloat("yVel", -y);
    }

    void Fire(){
        if(blasterState == BlasterState.SingleFire){
            AudioManager.Play("BlasterSound");
            GameObject bullet = SpawnManager.Spawn(bulletPrefab, firePosMain.position);
            bullet.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet.GetComponentInParent<BulletController>().SetDir(firePosMain.forward);
        }
        if(blasterState == BlasterState.DoubleFire){
            AudioManager.Play("BlasterSound");
            GameObject bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
            bullet1.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);
            GameObject bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
            bullet2.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
        }
        if(blasterState == BlasterState.DoubleFire){
            AudioManager.Play("BlasterSound");
            GameObject bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
            bullet1.GetComponentInParent<DamageController>().damage *= 2;
            bullet1.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);
            GameObject bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
            bullet2.GetComponentInParent<DamageController>().damage *= 2;
            bullet2.GetComponentInParent<BulletController>().startTime = Time.time;
            bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
        }
    }

    void Crash(HealthController health){
        if(crash) return;
        anim.Play("PlayerCrash");
        crash = true;
        anim.SetBool("crashing", crash);
        rb.AddForce(transform.forward.normalized, ForceMode.Impulse);
        rb.velocity = Vector3.forward * speed.z;
        rb.useGravity = true;
        crashTime = Time.time;
        onCrash(this);
    }

    void Crash(){
        if(crash) return;
        anim.Play("PlayerCrash");
        crash = true;
        anim.SetBool("crashing", crash);
        rb.AddForce(transform.forward.normalized, ForceMode.Impulse);
        rb.velocity = Vector3.forward * speed.z;
        rb.useGravity = true;
        crashTime = Time.time;
        onCrash(this);
    }

    void OnTriggerExit(Collider col){
        if(col.name == "RingCollider"){
            AudioManager.Play("RingSound");
            GameManager.game.IncrementScore();
        }
        if(col.tag == "BombPowerup"){
            bombAmmo++;
            onFireBomb(bombAmmo);
            col.gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision col){
        if(Time.time - lastCollisionTime > collisionShield){
            lastCollisionTime = Time.time;
            health.TakeDamage(1);
            if(!crash)anim.Play("PlayerHit");
        }
        Vector3 diff = Vector3.Cross(player.transform.position, col.transform.position);
        float dot = Vector3.Dot(player.transform.position, col.transform.position);
        if(diff.normalized.magnitude > dot){
            rb.AddForce(diff.normalized * speed.z, ForceMode.Impulse);
        }
        else{
            rb.AddForce(-diff.normalized * speed.z, ForceMode.Impulse);
        }
        if(crash) {
            onDeath(this);
            gameObject.SetActive(false);
        }
        if(health.health <= 0){
            Crash();
        }
    }
}
