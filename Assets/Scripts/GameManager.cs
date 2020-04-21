using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static GameManager game;
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
        score = 0;
    }

    void Update(){
    }
    public void IncrementScore(){
        score++;
    }
}
