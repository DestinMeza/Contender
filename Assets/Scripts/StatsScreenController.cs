using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatsScreenController : MonoBehaviour
{
    
    Text[] scoresUI;
    void Awake()
    {
        scoresUI = GetComponentsInChildren<Text>();
    }

    void Start(){
        for(int i = 0; i < scoresUI.Length; i++){
            string pref = string.Format("Player{0}", i);
            scoresUI[i].text = PlayerPrefs.GetString(pref, "");
            if(scoresUI[i].text != ""){
                pref = string.Format("Score{0}", i);
                scoresUI[i].text += string.Format(" Score: " + PlayerPrefs.GetInt(pref, 0));
                pref = string.Format("Hit{0}", i);
                scoresUI[i].text += string.Format(" Hits: " + PlayerPrefs.GetInt(pref, 0));
            }
        }
        foreach(Text score in scoresUI){
            if(score.text == "")score.gameObject.SetActive(false);
        }

    }
    public void MainMenu(){
        LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)MenuManager.ScenesByBuild.Stats);
    }
}
