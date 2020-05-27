using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    enum MenuScreens{
        MainMenu,
        PlayMenu
    }
    MenuScreens menuScreens = MenuScreens.MainMenu;
    Animator anim;

    void Awake(){
        anim = GetComponent<Animator>();
    }
    void Update(){
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.MainMenu){
            Application.Quit();
        }
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.PlayMenu){
            anim.Play("MainState"); // Scrollanimation
            menuScreens = MenuScreens.MainMenu;
        }
    }
    public void PlayGame(){
        anim.Play("PlayState");
        menuScreens = MenuScreens.PlayMenu;
    }
    public void Tutorial(){
        StartCoroutine(SceneLoader("Tutorial"));
    }
    public void Quit(){
        Application.Quit();
    }

    public void Mission1(){
        SceneManager.LoadScene("Mission1");
    }
    public void Mission2(){
        SceneManager.LoadScene("Mission2");
    }

    IEnumerator SceneLoader(string sceneToLoad){
        AudioManager.Play("LifeUp");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
