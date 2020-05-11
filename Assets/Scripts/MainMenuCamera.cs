using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    void Update(){
        transform.Rotate(transform.up, Time.deltaTime);
    }
}
