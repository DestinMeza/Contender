using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager particleMan;

    public ParticleSystem[] prefabs;

    List<ParticleSystem>[] particlePools;
    void Awake(){
        if(particleMan == null){
            particleMan = this;
            DontDestroyOnLoad(this);
            SetupParticles();
            
        }
        else{
            Debug.Log(gameObject + " was Destroyed");
            Destroy(gameObject);
        }
    }

    void SetupParticles(){
        particlePools = new List<ParticleSystem>[prefabs.Length];
        for(int i = 0; i < prefabs.Length; i++){
            particlePools[i] = new List<ParticleSystem>();
        }
    }

    public void Play(string particleName, Vector3 pos){

        for(int i = 0; i < particleMan.prefabs.Length; i++){
            if (particleName == particleMan.prefabs[i].name){
                
                ParticleSystem p = particleMan.particlePools[i].Find(CanUse);
                if(p == false){
                    p = Instantiate(particleMan.prefabs[i]) as ParticleSystem;
                    particleMan.particlePools[i].Add(p);
                }
                p.transform.position = pos;
                p.transform.SetParent(gameObject.transform);
                p.Play();
                return;
            }
        }
    }
    static bool CanUse(ParticleSystem p){
        return p.isStopped;
    }

    public void DisableAll(){
        for(int i = 0; i < particleMan.prefabs.Length; i++){
            for(int j = 0; j < particleMan.particlePools[i].Count; j++){
                particleMan.particlePools[i][j].Clear();
            }
        }
    }
}
