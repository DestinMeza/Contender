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

public enum FlyingModes{
    Rail,
    AllRange,
    TransitionLock
}
public class GameManager : MonoBehaviour
{
    public static FlyingModes flyingModes = FlyingModes.TransitionLock;
    public int score = 0;
    public int ringScore = 0;
    public int hitScore = 0;
    public static GameManager game;
    public GameObject AllRangeModeSpawner;
    public GameObject enemiesParent;
    public GameObject ringsParent;
    public GameObject gameOverSign;
    public GameObject playerUI;
    public Text ringScoreText;
    public Text scoreText;
    public Text hitScoreText;
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
        HealthController.onIncreaseScore += IncrementScore;
        enemies = enemiesParent.GetComponentsInChildren<HealthController>();
        rings = ringsParent.GetComponentsInChildren<RingController>();
        score = 0;
        ringScore = 0;
        hitScore = 0;
        gameState = GameState.GameStart;
        TransitionController.onTransition += TransitionStateChange;
    }

    void TransitionStateChange(FlyingModes currentState){
        flyingModes = currentState;
    }

    void Update(){
        if(Input.GetButtonDown("Cancel")){
            SceneManager.LoadScene("MainMenu");
        }
        if(gameState == GameState.GameStart) Setup();
        if(gameState == GameState.GamePlaying)GameplayUpdate();
        if(gameState == GameState.GameOver)GameOverUpdate();
        if(flyingModes == FlyingModes.AllRange){
            AllRangeModeSpawner.SetActive(true);
        }
        else{
            AllRangeModeSpawner.SetActive(false);
        }
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
        ringScore = 0;
        hitScore = 0;
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
        scoreText.text = string.Format("score: {0}", score);
        hitScoreText.text = string.Format("hit: {0}", hitScore);
        ringScoreText.text = string.Format("Rings : {0}", ringScore);
    }

    void GameOver(PlayerControler player){
        gameState = GameState.GameOver;
    }

    void GameOverUpdate(){
        gameOverSign.SetActive(true);
        SpawnManager.DisableAll();
        score = 0;
        ringScore = 0;
        playerUI.gameObject.SetActive(false);
        if(Input.GetButtonDown("Submit")){
            gameState = GameState.GameStart;
        }
        if(Input.GetButtonDown("Cancel")){
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void IncrementRingScore(){
        ringScore++;
        score += ringScore * 5;
        
    }

    void IncrementScore(int score){
        this.score += score;
        hitScore++;
    }

    public void ResetRingScore(){
        ringScore = 0;
    }
}
