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
    public GameObject inputField;
    public GameObject playGame;
    public GameObject mission1;
    public GameObject placeHolderText;
    public Text inputFieldText;
    public Text controllerNameInput;
    public static string[] players = new string[10];
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVQXYZ";
    char currentLetter;
    int alphabetIndexer;
    string currentName;
    bool level2Unlocked;
    bool usingController;
    float lastLetterChange;
    void Awake(){
        anim = GetComponent<Animator>();
    }
    void Start(){
        Cursor.visible = true;
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
        CheckMission2();
    }
    void Update(){

        if(Input.GetButtonDown("Cancel") && menuScreens == MenuScreens.MainMenu && eventSystem.currentSelectedGameObject != inputField){
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
        ControllerName();
    }

    void ControllerName(){
        if(eventSystem.currentSelectedGameObject == inputField && Input.GetKeyDown(KeyCode.Return)){
            GameObject fixSelection = menuScreens!= MenuScreens.MainMenu ? mission1 : playGame;
            eventSystem.SetSelectedGameObject(fixSelection);
        }
        if(eventSystem.currentSelectedGameObject == inputField && usingController){
            StartFieldInput();
            inputFieldText.gameObject.SetActive(false);
            controllerNameInput.gameObject.SetActive(true);

            if(Input.GetAxis("Vertical") > 0.3f && Time.time - lastLetterChange > 0.1f){
                alphabetIndexer++;
                if(alphabetIndexer > alphabet.Length -1) alphabetIndexer = 0;
                currentLetter = alphabet[alphabetIndexer];
                controllerNameInput.text = "";
                controllerNameInput.text += inputFieldText.text;
                controllerNameInput.text += currentLetter;
                lastLetterChange = Time.time;
            }
            else if(Input.GetAxis("Vertical") < -0.3f && Time.time - lastLetterChange > 0.1f){
                alphabetIndexer--;
                if(alphabetIndexer < 0) alphabetIndexer = alphabet.Length-1;
                currentLetter = alphabet[alphabetIndexer];
                controllerNameInput.text = "";
                controllerNameInput.text += inputFieldText.text;
                controllerNameInput.text += currentLetter;
                lastLetterChange = Time.time;
            }
            if(Input.GetButtonDown("Submit") && Time.time - lastLetterChange > 0.1f){
                currentName += currentLetter;
                inputFieldText.text = currentName;
                lastLetterChange = Time.time;
            }
            if(Input.GetButtonDown("Fire2") && Time.time - lastLetterChange > 0.1f){
                if(currentName.Length <= 1){
                    currentName = "";
                    inputFieldText.text = currentName;
                    controllerNameInput.text = inputFieldText.text;
                    lastLetterChange = Time.time;                    
                    return;
                }
                currentName = currentName.Remove(currentName.Length-1);
                inputFieldText.text = currentName;
                controllerNameInput.text = inputFieldText.text;
                lastLetterChange = Time.time;
            }
            if(Input.GetAxis("Horizontal") < -0.3f && usingController){
                InputField input = inputField.GetComponent<InputField>();
                input.text = controllerNameInput.text;
                inputFieldText.gameObject.SetActive(true);
                controllerNameInput.gameObject.SetActive(false);
                GameObject fixSelection = menuScreens!= MenuScreens.MainMenu ? mission1 : playGame;
                eventSystem.SetSelectedGameObject(fixSelection);
                usingController = false;
            }
        }
        else if(eventSystem.currentSelectedGameObject != inputField && Input.GetAxis("JoyHorizontal") > 0.3f && Input.GetAxis("JoyHorizontal") < 0.5f){
            eventSystem.SetSelectedGameObject(inputField);
            controllerNameInput.text = inputFieldText.text;
            currentName = controllerNameInput.text;
            currentLetter = alphabet[alphabetIndexer];
            controllerNameInput.text += currentLetter;
            lastLetterChange = Time.time;
            usingController = true;
        }
        else if(eventSystem.currentSelectedGameObject != inputField && Input.GetAxis("KeyHorizontal") > 0){
            eventSystem.SetSelectedGameObject(inputField);
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
        if(inputFieldText.text == ""){
            placeHolderText.gameObject.SetActive(true);
            SetCurrentPlayer(inputFieldText.text);
        } 
        else{
            placeHolderText.gameObject.SetActive(false);
            for(int i = 0; i < players.Length; i++){
                string savedPlayer = players[i];
                if(inputFieldText.text == savedPlayer){
                    SetCurrentPlayer(players[i]);
                    break;
                }
                if(players[i] == null){
                    string playerIndex = string.Format("Player{0}", i);
                    PlayerPrefs.SetString(playerIndex, inputFieldText.text);
                    SetCurrentPlayer(inputFieldText.text);
                    AddPlayerToList(inputFieldText.text, i);
                    break;
                }
                if(i == players.Length-1){
                    string bottomIndex = string.Format("Player{0}", players.Length-1);
                    PlayerPrefs.SetString(bottomIndex, inputFieldText.text);
                    SetCurrentPlayer(inputFieldText.text);
                    AddPlayerToList(inputFieldText.text, i);
                }
                
            }
            
        }
        CheckMission2();
        placeHolderText.GetComponent<Text>().text = PlayerPrefs.GetString("CurrentPlayer", "");
        if(PlayerPrefs.GetString("CurrentPlayer", "") == "") placeHolderText.GetComponent<Text>().text = "Player";
    }

    void CheckMission2(){
        for(int i = 0; i < players.Length; i++){
            if(players[i] == PlayerPrefs.GetString("CurrentPlayer")){
                string pref = string.Format("CanPlayMission2{0}", i);
                level2Unlocked = PlayerPrefs.GetInt(pref, i) == 1;
                break;
            }
        }
        anim.SetBool("L2", level2Unlocked);
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
