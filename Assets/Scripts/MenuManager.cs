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
    public enum ScenesByBuild{
        LoadingScreen,
        MainMenu,
        Tutorial,
        Mission1,
        Mission2,
        Stats,
    }
    MenuScreens menuScreens = MenuScreens.MainMenu;
    Animator anim;

    void Awake(){
        Cursor.visible = true;
        anim = GetComponent<Animator>();
    }
    void Start(){
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)MenuManager.ScenesByBuild.MainMenu));
    }
    void Update(){
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.MainMenu){
            Application.Quit();
        }
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.PlayMenu){
            anim.Play("MainState");
            menuScreens = MenuScreens.MainMenu;
        }
    }
    public void PlayGame(){
        anim.Play("PlayState");
        menuScreens = MenuScreens.PlayMenu;
    }
    public void Tutorial(){
        StartCoroutine(SceneLoader((int)ScenesByBuild.Tutorial, (int)ScenesByBuild.MainMenu));
    }
    public void Quit(){
        Application.Quit();
    }

    public void Mission1(){
        StartCoroutine(SceneLoader((int)ScenesByBuild.Mission1, (int)ScenesByBuild.MainMenu));
    }
    public void Mission2(){
        StartCoroutine(SceneLoader((int)ScenesByBuild.Mission2, (int)ScenesByBuild.MainMenu));
    }

    IEnumerator SceneLoader(int sceneToLoad, int sceneOrigin){
        AudioManager.Play("LifeUp");
        yield return new WaitForSeconds(0.2f);
        LoadingScreenController.instance.LoadLevel(sceneToLoad, sceneOrigin);
    }
}
