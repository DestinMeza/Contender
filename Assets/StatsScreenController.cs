using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatsScreenController : MonoBehaviour
{
    
    Text[] scores;
    void Awake()
    {
        scores = GetComponentsInChildren<Text>();
    }

    void Start(){
        for(int i = 0; i < scores.Length; i++){
            string pref = string.Format("Player{0}", i);
            scores[i].text = PlayerPrefs.GetString(pref, "");
            if(scores[i].text != ""){
                pref = string.Format("Score{0}", i);
                scores[i].text += string.Format(": " + PlayerPrefs.GetString(pref, ""));
            }
        }
        foreach(Text score in scores){
            if(score.text == "")score.gameObject.SetActive(false);
        }
    }
    public void MainMenu(){
        LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)MenuManager.ScenesByBuild.Stats);
    }
}
