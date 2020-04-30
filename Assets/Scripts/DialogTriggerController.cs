using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerController : MonoBehaviour
{
    public delegate void OnTriggerSet(int i, string name);
    public static OnTriggerSet onTriggerSet = delegate {};
    public int indexOfDialog = 0;
    public string characterName = "Jim Bob";

    void OnTriggerEnter(Collider col){
        if(col.GetComponentInParent<PlayerControler>()) onTriggerSet(indexOfDialog, characterName);
    }
}
