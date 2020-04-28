using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoostBarController : MonoBehaviour
{
    public Image image;

    void Start(){
        PlayerControler.onBoost += BoostBarChange;
    }
    void BoostBarChange(){
        image.fillAmount = (float)PlayerControler.player.boostMeter/(float)PlayerControler.player.boostMeterMax;
    }
}
