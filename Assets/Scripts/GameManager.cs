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
    public delegate void OnLifeChange(int lives);
    public static OnLifeChange onLifeChange = delegate{};
    public static FlyingModes flyingModes = FlyingModes.TransitionLock;
    public int score = 0;
    public int ringScore = 0;
    public int hitScore = 0;
    public int lives = 3;
    public static GameManager game;
    public List <GameObject> enemyObjects;
    public GameObject[] triggerObjects;
    public GameObject AllRangeModeSpawner;
    public GameObject gameOverSign;
    public GameObject playerUI;
    public PlayerControler player;
    public Text ringScoreText;
    public Text scoreText;
    public Text hitScoreText;
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
        Cursor.visible = false;
        PlayerControler.onDeath += GameOver;
        HealthController.onIncreaseScore += IncrementScore;
        TransitionController.onTransition += TransitionStateChange;
        score = 0;
        ringScore = 0;
        hitScore = 0;
        lives = 3;
        onLifeChange(lives);
        for (int i = 0; i < triggerObjects.Length; i++){
            GameObject[] enemies = triggerObjects[i].GetComponent<TriggerAreaController>().enemys;
            foreach(GameObject g in enemies){
                enemyObjects.Add(g);
            }
        }
        gameState = GameState.GameStart;
        StartCoroutine(TrackPlayerPos());
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
        gameOverSign.SetActive(false);
        gameState = GameState.GamePlaying;
    }

    void RespawnPlayer(){
        gameOverSign.SetActive(false);
        player.GetComponent<HealthController>().IncreaseHeath(4);
        playerUI.gameObject.SetActive(true);
        player.transform.position = initalPos;
        player.transform.rotation = Quaternion.identity;
        player.crash = false;
        player.GetComponent<Animator>().SetBool("crashing", false);
        player.gameObject.SetActive(true);
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().useGravity = false;
        
    }

    IEnumerator TrackPlayerPos(){
        while(enabled){
            initalPos = player.transform.position;
            yield return new WaitForSeconds(10);
        }
    }
    void GameplayUpdate(){
        if(!player.gameObject.activeSelf) player.gameObject.SetActive(true);
        scoreText.text = string.Format("score: {0}", score);
        hitScoreText.text = string.Format("hit: {0}", hitScore);
        ringScoreText.text = string.Format("Rings : {0}", ringScore);
    }

    void GameOver(PlayerControler player){
        this.player = player;
        if(lives < 0){
            gameState = GameState.GameOver;
        }
        else{
            lives--;
            onLifeChange(lives);
            RespawnPlayer();
        }
    }

    void GameOverUpdate(){
        StopCoroutine(TrackPlayerPos());
        gameOverSign.SetActive(true);
        SpawnManager.DisableAll();
        playerUI.gameObject.SetActive(false);
        if(Input.GetButtonDown("Submit")){
            Start();
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
