using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    public delegate void OnIncreaseScore(int score);
    public static event OnIncreaseScore onIncreaseScore = delegate{};
    public delegate void OnDeath(HealthController health);
    public OnDeath onDeath = delegate {};

    public delegate void OnHealthChange();
    public OnHealthChange onHealthChange = delegate {};

    public int maxHealth = 3;
    public int health;
    public int scoreValue = 5;
    public bool projectScore = false;

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
            if(projectScore) onIncreaseScore(scoreValue);
        }
    }

    public void IncreaseHeath(int health){
        this.health += health;
        onHealthChange();
    }
}
