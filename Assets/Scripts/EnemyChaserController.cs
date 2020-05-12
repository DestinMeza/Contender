using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaserController : MonoBehaviour
{
    public delegate void OnDeathCalculation(Vector3 position);
    public static event OnDeathCalculation onDeathCalculation = delegate{};
    public enum RailState {
        FollowPlayer,
        KitePlayer,
        Leave
    }
    public RailState railState;
    public float rotationalDamp = 0.5f;
    public float speed = 30;
    public float firingDelay = 1;
    public float maxFollowDistance = 30;
    public string deathParticles = "ExplosionSmallObject";
    public string enemyBulletPrefab = "EnemyBullet";
    public LayerMask obsticles;
    public Transform firingPos1;
    public Transform firingPos2;
    public Transform target;
    public Vector3 railKiteOffset = new Vector3(0,0,20);
    public FlyingModes flyingModes;
    Rigidbody rb;
    float lastShotTime = 0;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        flyingModes = GameManager.flyingModes;
        TransitionController.onTransition += ModeCheck;
    }
    void OnDisable(){
        TransitionController.onTransition -= ModeCheck;
    }
    void Start(){
        HealthController health = GetComponentInParent<HealthController>();
        health.onDeath += Explode;
    }
    void Update()
    {
        if(flyingModes == FlyingModes.AllRange){
            Turn();
            MoveAllRange();
            CheckObsticles();
        }
        else if(flyingModes == FlyingModes.Rail){
            MoveRail();
            CheckObsticles();
        }
    }

    void Turn(){
        Vector3 diff = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
    }
    void MoveAllRange(){
        Vector3 diff = target.position - transform.position;
        Vector3 steeringDir = transform.forward;
        if(diff.magnitude < maxFollowDistance){
            rb.AddForce(steeringDir.normalized * speed/2 * -1, ForceMode.Acceleration);
        }
        else{
            rb.AddForce(steeringDir.normalized * speed, ForceMode.Acceleration);
            rb.velocity = new Vector3(
                Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
                Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
            );
        }
    }

    void MoveRail(){
        Vector3 diff = target.position - transform.position;

        if(railState != RailState.Leave){
            transform.forward = diff.normalized;
            Vector3 steeringDir = transform.forward;
            if(railState == RailState.KitePlayer){
                steeringDir = transform.forward + railKiteOffset;
            }
            if(diff.magnitude < maxFollowDistance){
                if(Input.GetButtonDown("Bank") && Input.GetButtonDown("BarrelRoll") || Input.GetKeyDown(KeyCode.Q)){
                    railState = RailState.Leave;
                }
                rb.AddForce(steeringDir.normalized * speed/2 * -1, ForceMode.VelocityChange);
            }
            else{
                rb.AddForce(steeringDir.normalized * speed, ForceMode.VelocityChange);
                rb.velocity = new Vector3(
                    Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
                    Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
                    Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
                );
            }
        }
        else{
            rb.AddForce(((target.forward * speed) + transform.up/2).normalized, ForceMode.VelocityChange);
                rb.velocity = new Vector3(
                    Mathf.Clamp(rb.velocity.x, speed*-1.0f, speed*1.0f),
                    Mathf.Clamp(rb.velocity.y, speed*-1.0f, speed*1.0f),
                    Mathf.Clamp(rb.velocity.z, speed*-1.0f, speed*1.0f)
            );
        }
        
        
    }

    void CheckObsticles(){
        Ray ray = new Ray(transform.position, rb.velocity.normalized);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, obsticles, QueryTriggerInteraction.Collide)){
            PlayerController player = hit.collider.GetComponentInParent<PlayerController>();
            if(hit.collider.gameObject.tag == "Solid" && player == null){
                Fire();
                Transform obsticle = hit.collider.GetComponentInParent<Transform>();
                Vector3 diff = obsticle.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(diff, transform.right.normalized));
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            }
            else if(player != null){
                Fire();
            }
        }
    }

    void Fire(){
        if(Time.time - lastShotTime > firingDelay){
            AudioManager.Play("BlasterSound2",1,1,false,transform.position,0.9f);
            BulletController bullet = SpawnManager.Spawn(enemyBulletPrefab, firingPos1.position).GetComponent<BulletController>();
            bullet.SetDir(firingPos1.forward);
            bullet = SpawnManager.Spawn(enemyBulletPrefab, firingPos2.position).GetComponent<BulletController>();
            bullet.SetDir(firingPos2.forward);
            lastShotTime = Time.time;
        }
    }
    
    void Explode(HealthController health){
        ParticleManager.particleMan.Play(deathParticles, transform.position);
        AudioManager.Play("SmallObjectExplosion",1,1,false,transform.position,0.8f);
        onDeathCalculation(transform.position);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision col){
        GetComponent<HealthController>().TakeDamage(3);
    }
    void ModeCheck(FlyingModes currentState){
        flyingModes = currentState;
    }
}
