using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{

    public Image[] healthBars;
    public HealthController player;

    void Awake(){
        healthBars = GetComponentsInChildren<Image>();
    }

    void OnEnable(){
        player.onHealthChange += ChangeHeath;
        ChangeHeath();
    }

    void ChangeHeath(){
        for(int i = 0; i < healthBars.Length; i++){
            healthBars[i].enabled = (i < player.health);
        }
    }
}
