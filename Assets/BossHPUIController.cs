using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHPUIController : MonoBehaviour
{
    Image healthBarUI;
    public int currentHP;
    
    void Awake(){
        healthBarUI = GetComponent<Image>();
    }

    void OnEnable(){
        BossHealthController.onTakingDamage += HealthChange;
    }
    void OnDisable(){
        BossHealthController.onTakingDamage -= HealthChange;
    }

    void HealthChange(int hp, int maxHp){
        currentHP = hp;
        healthBarUI.fillAmount = currentHP/maxHp;
    }
}
