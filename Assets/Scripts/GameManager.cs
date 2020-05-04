using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum GameState{
    GameStart,
    GamePlaying,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static GameManager game;
    public GameObject enemiesParent;
    public GameObject ringsParent;
    public GameObject gameOverSign;
    public GameObject playerUI;
    public Text scoreText;
    public RingController[] rings;
    public HealthController[] enemies;
    public GameState gameState = GameState.GameStart; 
    public Vector3 initalPos = new Vector3(0, 5, 0);
    void Awake(){
        if(game == null){
            game = this;
        }
        else{
            Destroy(gameObject);
        }
    }
    void Start()
    {   
        PlayerControler.onDeath += GameOver;
        enemies = enemiesParent.GetComponentsInChildren<HealthController>();
        rings = ringsParent.GetComponentsInChildren<RingController>();
        score = 0;
        gameState = GameState.GameStart;
    }

    void Update(){
        if(Input.GetButtonDown("Cancel")){
            SceneManager.LoadScene("MainMenu");
        }
        if(gameState == GameState.GameStart) Setup();
        if(gameState == GameState.GamePlaying)GameplayUpdate();
        if(gameState == GameState.GameOver)GameOverUpdate();
    }
    void Setup(){
        
        foreach(HealthController enemy in enemies){
            if(!enemy.gameObject.activeSelf) enemy.gameObject.SetActive(true);
        }
        foreach(RingController ring in rings){
            if(!ring.gameObject.activeSelf) ring.gameObject.SetActive(true);
        }
        gameOverSign.SetActive(false);
        score = 0;
        CameraController.cameraMain.transform.position = CameraController.cameraMain.target.position;
        PlayerControler.player.gameObject.SetActive(true);
        ResetPlayerPos();
        playerUI.gameObject.SetActive(true);
        gameState = GameState.GamePlaying;
    }

    void ResetPlayerPos(){
        PlayerControler.player.transform.position = initalPos;
        PlayerControler.player.transform.rotation = Quaternion.identity;
        PlayerControler.player.crash = false;
        PlayerControler.player.GetComponent<Animator>().SetBool("crashing", false);
        PlayerControler.player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        PlayerControler.player.GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
        PlayerControler.player.GetComponent<Rigidbody>().useGravity = false;
    }
    void GameplayUpdate(){
        scoreText.text = string.Format("Rings : {0}", score);
    }

    void GameOver(PlayerControler player){
        gameState = GameState.GameOver;
    }

    void GameOverUpdate(){
        gameOverSign.SetActive(true);
        SpawnManager.DisableAll();
        score = 0;
        playerUI.gameObject.SetActive(false);
        if(Input.GetButtonDown("Submit")){
            gameState = GameState.GameStart;
        }
        if(Input.GetButtonDown("Cancel")){
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void IncrementScore(){
        score++;
    }

    public void ResetScore(){
        score = 0;
    }
}
