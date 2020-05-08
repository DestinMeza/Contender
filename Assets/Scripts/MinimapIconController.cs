using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    Transform parentPos;
    Material[] objectBaseColors;
    MeshRenderer mesh;
    Camera minimapCam;
    void Awake(){
        parentPos = GetComponentInParent<Transform>();
        mesh = GetComponent<MeshRenderer>();
        objectBaseColors = mesh.materials;
    }
    void Start(){
        minimapCam = PlayerControler.player.minimapCam;
    }
    void Update()
    {   if(GetComponentsInParent<PlayerControler>() == null){
            float frac = (transform.position.magnitude - minimapCam.farClipPlane) / (minimapCam.transform.position.magnitude - minimapCam.farClipPlane);
            for(int i = 0; i < objectBaseColors.Length; i++){
                mesh.materials[i].color = 
                Color.Lerp(Color.gray, objectBaseColors[i].color, Mathf.Clamp01(frac));
            }
        }
        transform.forward = parentPos.forward;
    }
}
