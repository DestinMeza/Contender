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
        PlayerControler.onBoost += BoostBarChange;
    }
    void BoostBarChange(){
        if(image != null){
            image.fillAmount = (float)PlayerControler.player.boostMeter/(float)PlayerControler.player.boostMeterMax;
            slider.value = image.fillAmount;
        }
    }
}
