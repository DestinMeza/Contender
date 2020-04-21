using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static PlayerControler player;
    public Vector3 speed = new Vector3(30,-20, 50);
    public float maxSpeedChange = 10;
    public GameObject bulletPrefab;
    public int maxBullets = 14;
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
        crashTime = Time.time;
        crash = false;
        bullets = new BulletController[maxBullets];
        for(int i = 0; i < maxBullets; i++){
            GameObject bullet = Instantiate(bulletPrefab);
            bullets[i] = bullet.GetComponent<BulletController>();
            bullet.gameObject.SetActive(false);
        }
    }

    void Update(){
        if(crash)
        {
            if(Time.time - crashTime > crashDuration){
                gameObject.SetActive(false);
            }
            return;
        }
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
        for(int i = 0; i < bullets.Length; i++){
            if(!bullets[i].gameObject.activeSelf){
                bullets[i].gameObject.SetActive(true);
                bullets[i].startTime = Time.time;
                bullets[i].gameObject.transform.position = firePos1.position;
                bullets[i].SetDir(Vector3.forward);
                break;
            }
        }
        for(int i = 0; i < bullets.Length; i++){
            if(!bullets[i].gameObject.activeSelf){
                bullets[i].gameObject.SetActive(true);
                bullets[i].startTime = Time.time;
                bullets[i].gameObject.transform.position = firePos2.position;
                bullets[i].SetDir(Vector3.forward);
                return;
            }
        }
    }

    void OnTriggerExit(Collider col){
        if(col.name == "RingCollider"){
            GameManager.game.IncrementScore();
        }
    }
    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "EBullet"){
            Debug.Log("Hit!");
        }
    }
    void OnCollisionEnter(Collision col){
        if(crash) return;
        rb.useGravity = true;
        Debug.Log(col.collider.name);
        crashTime = Time.time;
        crash = true;
        
    }
}
