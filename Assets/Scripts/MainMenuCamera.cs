using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public float fogDensity;
    public Color fogColor;
    void Start(){
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
    void Update(){
        transform.Rotate(transform.up, Time.deltaTime);
    }
}
