using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static PlayerControler player;
    public Vector3 speed = new Vector3(30,-20, 100);
    public float maxSpeedChange = 10;
    public GameObject bulletPrefab;
    public int maxBullets = 14;
    public Transform firePos1;
    public Transform firePos2;
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
        bullets = new BulletController[maxBullets];
        for(int i = 0; i < maxBullets; i++){
            GameObject bullet = Instantiate(bulletPrefab);
            bullets[i] = bullet.GetComponent<BulletController>();
            bullet.gameObject.SetActive(false);
        }
    }

    void Update(){
        Movement();
        if(Input.GetButtonDown("Submit")){
            Fire();
        }
    }
    void FixedUpdate(){

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

        if(shift && Mathf.Abs(x) < 2){
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
                break;
            }
        }
        for(int i = 0; i < bullets.Length; i++){
            if(!bullets[i].gameObject.activeSelf){
                bullets[i].gameObject.SetActive(true);
                bullets[i].startTime = Time.time;
                bullets[i].gameObject.transform.position = firePos2.position;
                return;
            }
        }
    }

    void OnTriggerExit(Collider col){
        if(col.name == "RingCollider"){
            GameManager.game.IncrementScore();
        }
    }
}
