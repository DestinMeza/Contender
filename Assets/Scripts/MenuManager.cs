using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    public EventSystem eventSystem;
    public GameObject playGame;
    public GameObject mission1;
    public GameObject placeHolderText;
    public Text inputField;
    string[] players = new string[10];
    void Awake(){
        Cursor.visible = true;
        anim = GetComponent<Animator>();
    }
    void Start(){
        placeHolderText.GetComponent<Text>().text = PlayerPrefs.GetString("CurrentPlayer", "");
        if(PlayerPrefs.GetString("CurrentPlayer", "") == "") placeHolderText.GetComponent<Text>().text = "Player";
        for(int i = 0; i < players.Length; i++){
            string playerIndex = string.Format("Player{0}", i);
            if(PlayerPrefs.GetString(playerIndex) != ""){
                players[i] = PlayerPrefs.GetString(playerIndex);
            }
            else{
                break;
            }
        }
    }
    void Update(){
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.MainMenu){
            PlayerPrefs.DeleteKey("CurrentPlayer");
            Application.Quit();
        }
        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.PlayMenu){
            anim.Play("MainState");
            menuScreens = MenuScreens.MainMenu;
        }
        if(eventSystem.currentSelectedGameObject == null){
            GameObject fixSelection = menuScreens!= MenuScreens.MainMenu ? mission1 : playGame;
            eventSystem.SetSelectedGameObject(fixSelection);
        }
    }
    public void PlayGame(){
        anim.Play("PlayState");
        menuScreens = MenuScreens.PlayMenu;
    }
    public void Tutorial(){
        SceneLoader((int)ScenesByBuild.Tutorial, (int)ScenesByBuild.MainMenu);
    }
    public void Stats(){
        SceneLoader((int)ScenesByBuild.Stats, (int)ScenesByBuild.MainMenu);
    }
    public void Quit(){
        Application.Quit();
    }
    
    public void Mission1(){
        SceneLoader((int)ScenesByBuild.Mission1, (int)ScenesByBuild.MainMenu);
    }
    public void Mission2(){
        SceneLoader((int)ScenesByBuild.Mission2, (int)ScenesByBuild.MainMenu);
    }

    public void StartFieldInput(){
        placeHolderText.gameObject.SetActive(false);
    }
    public void EndFieldInput(){
        if(inputField.text == "") placeHolderText.gameObject.SetActive(true);
        else{
            placeHolderText.gameObject.SetActive(false);
            for(int i = 0; i < players.Length; i++){
                string savedPlayer = players[i];
                if(inputField.text == savedPlayer){
                    SetCurrentPlayer(players[i]);
                    break;
                }
                if(players[i] == null){
                    string playerIndex = string.Format("Player{0}", i);
                    PlayerPrefs.SetString(playerIndex, inputField.text);
                    SetCurrentPlayer(inputField.text);
                    AddPlayerToList(inputField.text, i);
                    break;
                }
                if(i == players.Length-1){
                    string bottomIndex = string.Format("Player{0}", players.Length-1);
                    PlayerPrefs.SetString(bottomIndex, inputField.text);
                    SetCurrentPlayer(inputField.text);
                    AddPlayerToList(inputField.text, i);
                }
                
            }
            
        }
        foreach(string s in players){
            Debug.Log(s);
        }
    }

    void SetCurrentPlayer(string currentPlayer){
        PlayerPrefs.SetString("CurrentPlayer", currentPlayer);
        Debug.Log("CurrentPlayer: " + PlayerPrefs.GetString("CurrentPlayer", "No Player"));
    }

    void AddPlayerToList(string currentPlayer, int index){
        players[index] = currentPlayer;
    }

    void PlayGameScreen(){
        eventSystem.SetSelectedGameObject(mission1);
    }

    void MainMenuScreen(){
        eventSystem.SetSelectedGameObject(playGame);
    }

    void SceneLoader(int sceneToLoad, int sceneOrigin){
        LoadingScreenController.instance.LoadLevel(sceneToLoad, sceneOrigin);
    }
}
