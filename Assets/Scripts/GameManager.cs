using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    GameStart,
    GamePlaying,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static GameManager game;
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
        gameState = GameState.GameStart;
        score = 0;
    }

    void Update(){
        if(gameState == GameState.GameStart) Setup();
        if(gameState == GameState.GamePlaying)GameplayUpdate();
        if(gameState == GameState.GameOver)GameOverUpdate();
    }
    void Setup(){
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
        if(!PlayerControler.player.gameObject.activeSelf){
            gameState = GameState.GameOver;
        }
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
