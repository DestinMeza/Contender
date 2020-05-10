using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{

    public Image[] healthBars;
    public HealthController player;

    void Start(){
        healthBars = GetComponentsInChildren<Image>();
    }

    void OnEnable(){
        player.onHealthDecrease += ChangeHeath;
        player.onHealthIncrease += ChangeHeath;
        ChangeHeath();
    }

    void OnDisable(){
        player.onHealthDecrease -= ChangeHeath;
        player.onHealthIncrease -= ChangeHeath;
    }

    void ChangeHeath(){
        for(int i = 0; i < healthBars.Length; i++){
            healthBars[i].enabled = (i < player.health);
        }
    }
}
