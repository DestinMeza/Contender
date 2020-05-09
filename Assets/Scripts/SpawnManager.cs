using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager spawner;
    public GameObject[] prefabs;
    List<GameObject>[] pools;
    void Awake(){
        if (spawner == null){
            spawner = this;
            Setup();
        }
        else {
            Destroy(gameObject);
            Debug.Log(gameObject + " Destroyed");
        }
    }

    void Setup(){
        pools = new List<GameObject>[prefabs.Length];
        for(int i = 0; i < prefabs.Length; i++){
            pools[i] = new List<GameObject>();
        }
    }

    public static GameObject Spawn(string objName, Vector3 pos){
        if(objName == null) return null;
        for(int i = 0; i < spawner.prefabs.Length; i++){
            if(objName == spawner.prefabs[i].name){
                
                GameObject g = spawner.pools[i].Find(CanUse);
                if(g == null){
                    g = Instantiate(spawner.prefabs[i]) as GameObject;
                    spawner.pools[i].Add(g);
                }
                g.transform.position = pos;
                g.transform.SetParent(spawner.transform);
                g.SetActive(true);
                return g;
            }
        }
        return null;
    }

    static bool CanUse(GameObject g){
        return !g.activeSelf;
    }
    
    public static void DisableAll(){
        for(int i = 0; i < spawner.prefabs.Length; i++){
            for(int j = 0; j < spawner.pools[i].Count; j++){
                spawner.pools[i][j].gameObject.SetActive(false);
            }
        }
    }
}
