using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    public delegate void OnDeath(HealthController health);
    public static OnDeath onDeath = delegate {};

    public int maxHealth = 3;
    int health;

    void OnEnable(){
        health = maxHealth;
    }

    public bool isAlive(){
        return health > 0;
    }
    public int GetHealth(){
        return health;
    }
    public void TakeDamage(int damage){
        health -= damage;
        Debug.Log(gameObject.name + "Took Damage");
        if(health <= 0){
            health = 0;
            onDeath(this);
        }
    }
}
