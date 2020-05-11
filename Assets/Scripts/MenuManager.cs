using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    void Update(){
        if(Input.GetButtonDown("Cancel")){
            Application.Quit();
        }
    }
    public void Tutorial(){
        StartCoroutine(SceneLoader("Tutorial"));
    }
    public void Quit(){
        Application.Quit();
    }

    IEnumerator SceneLoader(string sceneToLoad){
        AudioManager.Play("LifeUp");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
