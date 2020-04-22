using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GameState{
    GameStart,
    GamePlaying,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static GameManager game;
    public GameObject stageSet;
    public Text scoreText;
    public TurretController[] turrets;
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
        turrets = stageSet.GetComponentsInChildren<TurretController>();
        score = 0;
        gameState = GameState.GameStart;
        
    }

    void Update(){
        if(gameState == GameState.GameStart) Setup();
        if(gameState == GameState.GamePlaying)GameplayUpdate();
        if(gameState == GameState.GameOver)GameOverUpdate();
    }
    void Setup(){
        
        for(int i = 0; i < turrets.Length; i++){
            if(!turrets[i].gameObject.activeSelf) turrets[i].gameObject.SetActive(true);
        }
        score = 0;
        PlayerControler.player.gameObject.SetActive(true);
        PlayerControler.player.transform.position = initalPos;
        PlayerControler.player.transform.rotation = Quaternion.identity;
        PlayerControler.player.crash = false;
        PlayerControler.player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        PlayerControler.player.GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
        PlayerControler.player.GetComponent<Rigidbody>().useGravity = false;
        gameState = GameState.GamePlaying;
    }
    void GameplayUpdate(){
        scoreText.text = string.Format("Rings :{0}", score);
    }

    void GameOverUpdate(){
        SpawnManager.DisableAll();
        score = 0;
        gameState = GameState.GameStart;
    }
    public void IncrementScore(){
        score++;
    }
}
