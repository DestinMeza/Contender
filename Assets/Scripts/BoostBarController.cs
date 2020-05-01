﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoostBarController : MonoBehaviour
{
    public Image image;
    public Slider slider;

    void Start(){
        PlayerControler.onBoost += BoostBarChange;
    }
    void BoostBarChange(){
        image.fillAmount = (float)PlayerControler.player.boostMeter/(float)PlayerControler.player.boostMeterMax;
        slider.value = image.fillAmount;
    }
}