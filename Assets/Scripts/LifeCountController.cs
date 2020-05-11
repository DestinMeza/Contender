using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeCountController : MonoBehaviour
{
    Image[] lifeIcons;
    public GameObject countParent;
    public GameObject iconParent;
    public Text countText;
    void Awake(){
        lifeIcons = iconParent.GetComponentsInChildren<Image>();
    }
    void OnEnable(){
        GameManager.onLifeChange += SetlifeCount;
        SetlifeCount(GameManager.game.lives);
    }

    void OnDisable(){
        GameManager.onLifeChange -= SetlifeCount;
    }
    public void SetlifeCount (int count){
        if(count > lifeIcons.Length){
            countParent.SetActive(true);
            iconParent.SetActive(false);
            countText.text = count.ToString();
        }
        else{
            countParent.SetActive(false);
            iconParent.SetActive(true);
            for(int i = 0; i < lifeIcons.Length; i++){
                lifeIcons[i].enabled = (i < count);
            }
        }
    }
}
