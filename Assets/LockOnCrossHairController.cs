using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnCrossHairController : MonoBehaviour
{
    public Vector3 lockOnOffset = new Vector3(0,0,60);
    public GameObject resetParent;
    public void LockOnCrossHairs(GameObject lockedOnEnemy){
        if(lockedOnEnemy == null){
            transform.parent = resetParent.transform;
            transform.forward = resetParent.transform.forward;
            transform.localPosition = Vector3.zero + lockOnOffset;
        }
        else{
            transform.parent = null;
            transform.position = lockedOnEnemy.transform.position;
        }
    }
}
