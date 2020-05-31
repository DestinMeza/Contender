using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum GameState{
    GameStart,
    GamePlaying,
    GameOver,
    Victory,
    Pause,
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
    public int[] scores = new int[10];
    public int score = 0;
    public int ringScore = 0;
    public int hitScore = 0;
    public static int lives = 3;
    public static GameManager game;
    public List <GameObject> enemyObjects;
    public GameObject[] triggerObjects;
    public GameObject AllRangeModeSpawner;
    public GameObject victoryUI;
    public GameObject gameOverSign;
    public GameObject bossUI;
    public GameObject playerUI;
    public GameObject menuUI;
    public GameObject continueButton;
    public EventSystem eventSystem;
    public PlayerController player;
    public Text ringScoreText;
    public Text scoreText;
    public Text hitScoreText;
    public Text hitScoreVictoryText;
    public GameState gameState = GameState.GameStart;
    public GameState previousState;
    public Vector3 initalPos = new Vector3(0, 5, 0);
    public AudioClip[] soundTracks;
    AudioSource songPlayer;
    bool bossFight = false;
    public MenuManager.ScenesByBuild scenesByBuild;
    void Awake(){
        if(game == null){
            game = this;
            songPlayer = GetComponent<AudioSource>();
        }
        else{
            Destroy(gameObject);
        }
    }
    void Start()
    {   
        playerUI.SetActive(false);
        victoryUI.SetActive(false);
        if(scenesByBuild != MenuManager.ScenesByBuild.Tutorial) ringScoreText.gameObject.SetActive(false);
        Cursor.visible = false;
        gameState = GameState.GameStart;
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
        menuUI.SetActive(false);
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
        
        if(gameState == GameState.Victory){
            VictoryUpdate();
            return;
        }
        if(gameState != GameState.Pause) previousState = gameState;
        if(Input.GetButtonDown("Cancel")){
            MenuToggle();
        }
        if(gameState == GameState.Pause && eventSystem.currentSelectedGameObject == null){
            MenuToggle();
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

    public void MenuToggle(){
        if(gameState != GameState.Pause){
            menuUI.SetActive(true);
            Cursor.visible = true;
            eventSystem.SetSelectedGameObject(continueButton);
            gameState = GameState.Pause;
            Time.timeScale = 0;
        }
        else{
            Cursor.visible = false;
            gameState = previousState;
            menuUI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void LoadMainMenu(){
        Time.timeScale = 1;
        LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)scenesByBuild);
    }

    IEnumerator TrackPlayerPos(){
        while(enabled){
            initalPos = player.transform.position;
            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator SwitchSong(){
        for(float t = 0; t <= 1; t += Time.maximumParticleDeltaTime){
            songPlayer.volume = Mathf.Lerp(0.05f, 0, t);
            yield return new WaitForEndOfFrame();
        }
        songPlayer.clip = songPlayer.clip == soundTracks[0] ? soundTracks[1] : soundTracks[0];
        songPlayer.Play();
        for(float t = 0; t <= 1; t += Time.maximumParticleDeltaTime){
            songPlayer.volume = Mathf.Lerp(0, 0.05f, t);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator HitFlash(){
        while(enabled){
            victoryUI.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            victoryUI.SetActive(true);
        }
    }

    void BossSpawned(bool isAlive){
        bossFight = isAlive;
        StartCoroutine(SwitchSong());
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
        if(Input.GetButtonDown("Cancel") || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit")){
            LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)scenesByBuild);
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
            LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.Mission1, (int)scenesByBuild);
        }
        if(Input.GetButtonDown("Cancel")){
            LoadingScreenController.instance.LoadLevel((int)MenuManager.ScenesByBuild.MainMenu, (int)scenesByBuild);
        }
    }
    public void IncrementRingScore(){
        ringScore++;
        score += ringScore * 5;
    }

    void Victory(){
        UpdateScore();
        StartCoroutine(SwitchSong());
        PlayerController.flyingModes = FlyingModes.TransitionLock;
        gameOverSign.GetComponent<Text>().text = "Victory!";
        gameOverSign.SetActive(true);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        string pref = string.Format("Hit {0}!", hitScore);
        hitScoreVictoryText.text = PlayerPrefs.GetString(pref);
        StartCoroutine(HitFlash());
        gameState = GameState.Victory;
    }

    void UpdateScore(){
        for(int i = 0; i < MenuManager.players.Length; i++){
            string pref = string.Format("Player{0}", i);
            string playerCheck= PlayerPrefs.GetString(pref, "");
            if(playerCheck == PlayerPrefs.GetString("CurrentPlayer")){
                pref = string.Format("Score{0}", i);
                int currentScore = PlayerPrefs.GetInt(pref, 0);
                PlayerPrefs.SetInt(pref, currentScore + score);
                pref = string.Format("Hit{0}", i);
                int currentHit = PlayerPrefs.GetInt(pref, 0);
                PlayerPrefs.SetInt(pref, currentHit + hitScore);
                pref = string.Format("CanPlayMission2{0}", i);
                PlayerPrefs.SetInt(pref, 1);
                break;
            }
        }
    }

    void IncrementScore(int score){
        this.score += score;
        hitScore++;
    }

    public void ResetRingScore(){
        ringScore = 0;
    }
}
