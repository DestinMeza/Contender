using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHPUIController : MonoBehaviour
{
    Image healthBarUI;
    float hp;
    float maxHp;
    void Awake(){
        healthBarUI = GetComponent<Image>();
    }

    void OnEnable(){
        BossHealthController.onTakingDamage += HealthChange;
        hp = BossHealthController.bossMaxHp;
        maxHp = BossHealthController.bossHP;
        StartCoroutine(HealthBarGrow());
    }
    void OnDisable(){
        BossHealthController.onTakingDamage -= HealthChange;
    }

    IEnumerator HealthBarGrow(){
        for(float t = 0; t <= 1.5f; t += Time.deltaTime){
            healthBarUI.fillAmount = Mathf.Lerp(0, (float)hp/(float)maxHp, t/1.5f);
            yield return new WaitForEndOfFrame();
        }
    }

    void HealthChange(int hp, int maxHp){
        this.maxHp = maxHp;
        this.hp = hp; 
        healthBarUI.fillAmount = (float)hp/(float)maxHp;
    }
}
