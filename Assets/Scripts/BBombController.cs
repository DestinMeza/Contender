using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBombController : MonoBehaviour
{

    public GameObject explosionSphere;

    public Color color1;
    public Color color2;
    public float lifeTime = 4;
    public float duration = 0.5f;
    public float speed = 80;
    Animator anim;
    Rigidbody rb;
    void Awake(){
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable(){
        StartCoroutine(LifeTime());
    }
    public void SetDir(Vector3 dir, float lifeTime)
    {
        transform.forward = dir.normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(dir.normalized * speed, ForceMode.VelocityChange);
    }
    public void SetDir(Vector3 dir)
    {
        SetDir(dir, lifeTime);
    }

    public void Explode(){
        StartCoroutine(ExplodeEffect());
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Solid"){
            Explode();
            StopCoroutine(LifeTime());
        }
    }
    IEnumerator LifeTime(){
        yield return new WaitForSeconds(lifeTime);
        Explode();
    }
    IEnumerator ExplodeEffect(){
        anim.Play("BombExplosion");
        for(float t = 0; t < duration; t += Time.deltaTime){
            explosionSphere.GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(color1, color2, t));
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
