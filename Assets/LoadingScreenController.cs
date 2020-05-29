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
    float totalSceneProgress;
    public void Awake(){
        instance = this;
        SceneManager.LoadSceneAsync((int)MenuManager.ScenesByBuild.MainMenu, LoadSceneMode.Additive);
        loadingScreen.SetActive(false);
    }
    public void LoadLevel (int sceneIndex, int sceneOrigin){
        loadingScreen.SetActive(true);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt((int)MenuManager.ScenesByBuild.LoadingScreen));
        AudioManager.Play("LifeUp");
        StartCoroutine(LoadLoadingScreen(sceneIndex, sceneOrigin));
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    IEnumerator LoadLoadingScreen(int sceneIndex, int sceneOrigin){
        
        scenesLoading.Add(SceneManager.UnloadSceneAsync(sceneOrigin));
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
            scenesLoading.Remove(scenesLoading[i]);
        }
        loadingScreen.SetActive(false);
    }
}
