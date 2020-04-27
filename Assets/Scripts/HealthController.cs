using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    public delegate void OnDeath(HealthController health);
    public OnDeath onDeath = delegate {};

    public delegate void OnHealthChange();
    public OnHealthChange onHealthChange = delegate {};

    public int maxHealth = 3;
    public int health;

    void OnEnable(){
        health = maxHealth;
        onHealthChange();
    }

    public bool isAlive(){
        return health > 0;
    }
    public int GetHealth(){
        return health;
    }
    public void TakeDamage(int damage){
        health -= damage;
        onHealthChange();
        Debug.Log(gameObject.name + "Took Damage");
        if(health <= 0){
            health = 0;
            onDeath(this);
        }
    }
}
