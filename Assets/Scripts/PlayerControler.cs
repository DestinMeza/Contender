using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    public delegate void OnCrash(PlayerControler player);
    public static OnCrash onCrash = delegate {};
    public delegate void OnDeath(PlayerControler player);
    public static OnDeath onDeath = delegate {};
    public static PlayerControler player;
    public Vector3 speed = new Vector3(30,-20, 50);
    public float maxSpeedChange = 10;
    public string bulletPrefab;
    public Transform firePos1;
    public Transform firePos2;
    public bool crash;
    public float crashDuration = 3;
    public float crashTime;

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
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Crash;
        crashTime = Time.time;
        crash = false;
    }

    void OnEnable(){
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

        if(Input.GetButtonDown("Submit")){
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
        bool shift = Input.GetKey(KeyCode.LeftShift);

        if(shift){
            x *= 2;
        }

        targetVelocity = new Vector3(x, y, 0).normalized + Vector3.forward;
        targetVelocity = Vector3.Scale(targetVelocity, speed);

        anim.SetFloat("xVel", x);
        anim.SetFloat("yVel", -y);
    }

    void Fire(){

        GameObject bullet1 = SpawnManager.Spawn(bulletPrefab, firePos1.position);
        bullet1.GetComponentInParent<BulletController>().startTime = Time.time;
        bullet1.GetComponentInParent<BulletController>().SetDir(firePos1.forward);

        GameObject bullet2 = SpawnManager.Spawn(bulletPrefab, firePos2.position);
        bullet2.GetComponentInParent<BulletController>().startTime = Time.time;
        bullet2.GetComponentInParent<BulletController>().SetDir(firePos2.forward);
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
            GameManager.game.IncrementScore();
        }
    }
    void OnCollisionEnter(Collision col){
        if(crash) {
            onDeath(this);
            gameObject.SetActive(false);
        }
        Crash();
    }
}
