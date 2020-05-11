﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRangeModeSpawner : MonoBehaviour
{
    public Transform[] spawnLoc;
    public string shipTrainingName = "EnemyFighterRoot";
    public string shipChasingName = "EnemyChaserRoot";
    public int maxSpawnedAtATime = 40;
    public int maxChasingShips = 1;
    public int totalForOtherShips = 15;
    public int totalDeadShips = 0;
    public int currentNumberChasing = 0;
    public int numShips = 0;
    
    void Start(){
        StartCoroutine(SpawnShips());
        EnemyShipControler.onDeathCalculation += ShipDied;
        EnemyChaserController.onDeathCalulation += ChasingShipDied;
    }
    void OnDisable(){
        StopCoroutine(SpawnShips());
        EnemyShipControler.onDeathCalculation -= ShipDied;
        EnemyChaserController.onDeathCalulation -= ChasingShipDied;
    }
    void ShipDied(){
        totalDeadShips++;
        numShips--;
    }
    void ChasingShipDied(){
        totalDeadShips++;
        currentNumberChasing--;
    }
    IEnumerator SpawnShips(){
        while(enabled){
            for(int i = numShips; i < maxSpawnedAtATime; i++){
                Vector3 spawnPos = spawnLoc[Random.Range(0, spawnLoc.Length-1)].position;

                spawnPos += new Vector3(
                    Random.Range(spawnPos.x*-2, spawnPos.x*2), 
                    Random.Range(spawnPos.y*-2, spawnPos.y*2), 
                    Random.Range(spawnPos.y*-2, spawnPos.y*2)
                );

                if(totalDeadShips >= totalForOtherShips && maxChasingShips > currentNumberChasing){
                    SpawnManager.Spawn(shipChasingName, spawnPos);
                    currentNumberChasing++;
                    numShips++;
                }
                else{
                    
                    GameObject enemyShip = SpawnManager.Spawn(shipTrainingName, spawnPos);
                    enemyShip.GetComponentInParent<EnemyShipControler>().SetDir(gameObject);
                    numShips++;
                }
                yield return new WaitForSeconds(2);
            }
            yield return new WaitForSeconds(4);
        }
    }
}