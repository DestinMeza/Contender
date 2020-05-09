using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRangeModeSpawner : MonoBehaviour
{
    public Transform[] spawnLoc;
    public string shipTrainingName = "EnemyFighterRoot";
    public string shipChasingName = "EnemyChaserRoot";
    public int maxSpawnedAtATime = 40;
    public int maxChasingShips = 1;
    int currentNumberChasing = 0;
    int numShips = 0;
    public int totalDeadShips = 0;
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
        numShips--;
    }
    void ChasingShipDied(){
        currentNumberChasing--;
    }
    IEnumerator SpawnShips(){
        while(enabled){
            if(numShips < maxSpawnedAtATime){
                if(totalDeadShips >= 50 && maxChasingShips >= currentNumberChasing){
                    SpawnManager.Spawn(shipChasingName, spawnLoc[Random.Range(0, spawnLoc.Length-1)].position + Random.insideUnitSphere);
                    currentNumberChasing++;
                    numShips++;
                }
                else{
                    GameObject enemyShip = SpawnManager.Spawn(shipTrainingName, spawnLoc[Random.Range(0, spawnLoc.Length-1)].position + Random.onUnitSphere);
                    enemyShip.GetComponentInParent<EnemyShipControler>().SetDir(gameObject);
                    numShips++;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
