using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum GameState{
    GameStart,
    GamePlaying,
    GameOver,
    Victory,
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
    public string[] pickupNames;
    public int score = 0;
    public int ringScore = 0;
    public int hitScore = 0;
    public int lives = 3;
    public static GameManager game;
    public List <GameObject> enemyObjects;
    public GameObject[] triggerObjects;
    public GameObject AllRangeModeSpawner;
    public GameObject gameOverSign;
    public GameObject bossUI;
    public GameObject playerUI;
    public PlayerController player;
    public Text ringScoreText;
    public Text scoreText;
    public Text hitScoreText;
    public GameState gameState = GameState.GameStart; 
    public Vector3 initalPos = new Vector3(0, 5, 0);
    bool bossFight = false;
    public float timerDuration = 5;
    public MenuManager.ScenesByBuild scenesByBuild;
    float sceneExitTimer;
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
        playerUI.SetActive(false);
        Cursor.visible = false;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)scenesByBuild));
        PlayerController.onDeath += GameOver;
        HealthController.onIncreaseScore += IncrementScore;
        TransitionController.onTransition += TransitionStateChange;
        EnemyShipAnimatorController.onDeathCalculation += DeathReward;
        EnemyShipController.onDeathCalculation += DeathReward;
        EnemyChaserController.onDeathCalculation += DeathReward;
        MechController.onDeathCalculation += DeathReward;
        TargetDroidController.onDeathCalculation += DeathReward;
        BossHealthController.onSpawned += BossSpawned;
        BossHealthController.onBossDeath += Victory;
        bossUI.SetActive(bossFight);
        score = 0;
        ringScore = 0;
        hitScore = 0;
        lives = 3;
        for (int i = 0; i < triggerObjects.Length; i++){
            GameObject[] enemies = triggerObjects[i].GetComponent<TriggerAreaController>().enemys;
            foreach(GameObject g in enemies){
                enemyObjects.Add(g);
            }
        }
        foreach(GameObject g in enemyObjects){
            g.SetActive(false);
        }
        gameState = GameState.GameStart;
        StartCoroutine(TrackPlayerPos());
    }

    void TransitionStateChange(FlyingModes currentState){
        flyingModes = currentState;
    }

    void Update(){
        if(Input.GetButtonDown("Cancel")){
            LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, SceneManager.GetActiveScene().buildIndex);
        }
        if(gameState == GameState.Victory){
            VictoryUpdate();
            return;
        }
        if(gameState == GameState.GameStart) Setup();
        if(gameState == GameState.GamePlaying)GameplayUpdate();
        if(gameState == GameState.GameOver)GameOverUpdate();
        if(flyingModes == FlyingModes.AllRange){
            if(AllRangeModeSpawner.gameObject != null) AllRangeModeSpawner.SetActive(true);
        }
        else{
            if(AllRangeModeSpawner.gameObject != null) AllRangeModeSpawner.SetActive(false);
        }
    }
    void Setup(){
        gameOverSign.SetActive(false);
        gameState = GameState.GamePlaying;
    }

    void RespawnPlayer(){
        gameOverSign.SetActive(false);
        player.GetComponent<HealthController>().IncreaseHeath(4);
        player.transform.position = initalPos;
        player.transform.rotation = Quaternion.identity;
        player.crash = false;
        player.GetComponent<Animator>().SetBool("crashing", false);
        player.gameObject.SetActive(true);
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().useGravity = false;
        
    }

    void DeathReward(Vector3 deathPos){
        int randomValue = Random.Range(0, pickupNames.Length);
        string spawnName = pickupNames[randomValue];
        SpawnManager.Spawn(spawnName, deathPos);
        Debug.Log("Spawned : " + spawnName);
    }

    IEnumerator TrackPlayerPos(){
        while(enabled){
            initalPos = player.transform.position;
            yield return new WaitForSeconds(10);
        }
    }

    void BossSpawned(bool isAlive){
        bossFight = isAlive;
    }

    void GameplayUpdate(){
        if(!player.gameObject.activeSelf) player.gameObject.SetActive(true);
        if(flyingModes != FlyingModes.TransitionLock && !player.crash) playerUI.SetActive(true);
        if(bossFight) bossUI.SetActive(bossFight);
        scoreText.text = string.Format("score: {0}", score);
        hitScoreText.text = string.Format("hit: {0}", hitScore);
        ringScoreText.text = string.Format("Rings : {0}", ringScore);
    }

    void VictoryUpdate(){
        if(Time.time - sceneExitTimer > timerDuration){
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).ToString());
        }
    }

    void GameOver(PlayerController player){
        this.player = player;
        if(lives <= 0){
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if(Input.GetButtonDown("Cancel")){
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void IncrementRingScore(){
        ringScore++;
        score += ringScore * 5;
    }

    void Victory(){
        gameOverSign.GetComponent<Text>().text = "Victory!";
        gameOverSign.SetActive(true);
        gameState = GameState.Victory;
        sceneExitTimer = Time.time;
    }

    void IncrementScore(int score){
        this.score += score;
        hitScore++;
    }

    public void ResetRingScore(){
        ringScore = 0;
    }
}
