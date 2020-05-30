using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController instance;
    public Image progressBar;
    public GameObject loadingScreen;
    public GameObject loadingCam;
    float totalSceneProgress;
    public void Awake(){
        instance = this;
        LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)MenuManager.ScenesByBuild.LoadingScreen);
        loadingScreen.SetActive(false);
    }
    public void LoadLevel (int sceneIndex, int sceneOrigin){
        loadingCam.SetActive(true);
        if(AudioManager.audioMan != null)AudioManager.Play("LifeUp");
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLoadingScreen(sceneIndex, sceneOrigin));
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    IEnumerator LoadLoadingScreen(int sceneIndex, int sceneOrigin){
        
        if(sceneOrigin != (int)MenuManager.ScenesByBuild.LoadingScreen)scenesLoading.Add(SceneManager.UnloadSceneAsync(sceneOrigin));
        scenesLoading.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
        for(int i = 0; i < scenesLoading.Count; i++){
            while(!scenesLoading[i].isDone){
                totalSceneProgress = 0;
                foreach(AsyncOperation operation in scenesLoading){
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress/scenesLoading.Count);

                progressBar.fillAmount = totalSceneProgress;
                
                yield return null;
            }
        }
        loadingCam.SetActive(false);
        loadingScreen.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        scenesLoading.RemoveRange(0, scenesLoading.Count);
        
    }
}
