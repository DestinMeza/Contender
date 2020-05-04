using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombCountController : MonoBehaviour
{
    Image[] bombIcons;
    public GameObject countParent;
    public GameObject iconParent;
    public Text countText;

    void Awake(){
        bombIcons = iconParent.GetComponentsInChildren<Image>();
    }
    void Start(){
        PlayerControler.onFireBomb += SetBombCount;
        SetBombCount(PlayerControler.player.bombAmmo);
    }

    public void SetBombCount (int count){
        if(count > bombIcons.Length){
            countParent.SetActive(true);
            iconParent.SetActive(false);
            countText.text = count.ToString();
        }
        else{
            countParent.SetActive(false);
            iconParent.SetActive(true);
            for(int i = 0; i < bombIcons.Length; i++){
                bombIcons[i].enabled = (i < count);
            }
        }
    }
}
