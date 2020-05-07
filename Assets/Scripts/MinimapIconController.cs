using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    Transform parentPos;
    void Awake(){
        parentPos = GetComponentInParent<Transform>();
    }
    void LateUpdate()
    {
        transform.forward = parentPos.forward;
    }
}
