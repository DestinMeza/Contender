using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    void Update(){
        if(Input.GetButtonDown("Cancel")){
            Application.Quit();
        }
    }
    public void Tutorial(){
        SceneManager.LoadScene("Tutorial");
    }
    public void Quit(){
        Application.Quit();
    }
}
