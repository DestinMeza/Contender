using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoostBarController : MonoBehaviour
{
    Image image;
    public Slider slider;

    void Awake(){
        image = GetComponent<Image>();
    }

    void Start(){
        PlayerController.onBoost += BoostBarChange;
    }
    void BoostBarChange(){
        if(image != null){
            image.fillAmount = (float)PlayerController.player.boostMeter/(float)PlayerController.player.boostMeterMax;
            slider.value = image.fillAmount;
        }
    }
}
