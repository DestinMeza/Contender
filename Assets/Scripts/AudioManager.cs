using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public static AudioManager audioMan;
    public AudioClip[] clips;
    public int maxSources = 10; 
    AudioSource[] sfxPool;
    int currentSound = 0;
    void Awake(){
        if (audioMan == null){ 

            audioMan = this;
            DontDestroyOnLoad(gameObject);
            SetupSFX();
            
        }
        else{
            Debug.Log(gameObject + " was Destroyed");
            Destroy(gameObject);
        }
    }
    void SetupSFX(){
        sfxPool = new AudioSource[maxSources];
        for(int i = 0; i < maxSources; i++){
            GameObject g = new GameObject("SFX." + i);
            g.transform.SetParent(transform);
            AudioSource source = g.AddComponent<AudioSource>();
            sfxPool[i] = source;
        }
    }
    public static void Play(string clipName){

        for(int i = 0; i < audioMan.clips.Length; i++){
            if (clipName == audioMan.clips[i].name){
                int index = audioMan.currentSound;
                AudioSource source = audioMan.sfxPool[index];
                source.clip = audioMan.clips[i];
                source.Play();
                audioMan.currentSound = (index + 1) % audioMan.maxSources;
                return;
            }
        }
        
    }
    public static void Play(string clipName, float pitch, float volume, bool loop){

        for(int i = 0; i < audioMan.clips.Length; i++){
            if (clipName == audioMan.clips[i].name){
                int index = audioMan.currentSound;
                AudioSource source = audioMan.sfxPool[index];
                source.loop = loop;
                source.pitch = pitch;
                source.volume = volume;
                source.clip = audioMan.clips[i];
                source.Play();
                audioMan.currentSound = (index + 1) % audioMan.maxSources;
                return;
            }
        }
    }
    public static void DisableAll(){
        for(int i = 0; i < audioMan.sfxPool.Length; i++){
            audioMan.sfxPool[i].Pause();
        }
    }
}
