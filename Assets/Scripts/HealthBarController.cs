using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{

    public Image image;
    public HealthController player;

    int maxHealth;

    void Awake(){
        maxHealth = player.maxHealth;
    }

    void OnEnable(){
        player.onHealthChange += ChangeHeath;
        ChangeHeath();
    }

    void ChangeHeath(){
        image.fillAmount = (float)player.health/(float)maxHealth;
    }
}
